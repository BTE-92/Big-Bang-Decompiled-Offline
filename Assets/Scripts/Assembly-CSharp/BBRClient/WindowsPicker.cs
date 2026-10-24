using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;

// "Import level" on Windows: the standard "Open file" dialog. It runs on its own thread and, like the Android picker,
// copies the chosen file to <cache>/picked.zip and writes <cache>/picked.done ("ok", "cancel" or "error").
public static class WindowsPicker
{
#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    private struct OpenFileName
    {
        public int lStructSize;
        public IntPtr hwndOwner;
        public IntPtr hInstance;
        public IntPtr lpstrFilter;
        public IntPtr lpstrCustomFilter;
        public int nMaxCustFilter;
        public int nFilterIndex;
        public IntPtr lpstrFile;
        public int nMaxFile;
        public IntPtr lpstrFileTitle;
        public int nMaxFileTitle;
        public IntPtr lpstrInitialDir;
        public IntPtr lpstrTitle;
        public int Flags;
        public short nFileOffset;
        public short nFileExtension;
        public IntPtr lpstrDefExt;
        public IntPtr lCustData;
        public IntPtr lpfnHook;
        public IntPtr lpTemplateName;
        public IntPtr pvReserved;
        public int dwReserved;
        public int FlagsEx;
    }

    [DllImport("comdlg32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    private static extern bool GetOpenFileNameW(ref OpenFileName ofn);

    [DllImport("user32.dll")]
    private static extern IntPtr GetActiveWindow();

    private const int OFN_PATHMUSTEXIST = 0x800;
    private const int OFN_FILEMUSTEXIST = 0x1000;
    private const int OFN_NOCHANGEDIR = 0x8;
    private const int OFN_EXPLORER = 0x80000;
    private const int Buffer = 2048;

    public static bool Launch(string cacheDir)
    {
        try
        {
            foreach (string f in new string[] { "picked.zip", "picked.tmp", "picked.name", "picked.done" })
            {
                string p = Path.Combine(cacheDir, f);
                if (File.Exists(p)) File.Delete(p);
            }
            IntPtr owner = GetActiveWindow();
            Thread t = new Thread(delegate() { Run(cacheDir, owner); });
            t.IsBackground = true;
            t.Start();
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    private static void Run(string cacheDir, IntPtr owner)
    {
        string status = "error";
        IntPtr file = IntPtr.Zero, filter = IntPtr.Zero, title = IntPtr.Zero;
        try
        {
            file = Marshal.AllocHGlobal(Buffer * 2);
            for (int i = 0; i < Buffer * 2; i++) Marshal.WriteByte(file, i, 0);
            filter = Marshal.StringToHGlobalUni("Level files (*.zip)\0*.zip\0All files (*.*)\0*.*\0");
            title = Marshal.StringToHGlobalUni("Import level");
            OpenFileName ofn = new OpenFileName();
            ofn.lStructSize = Marshal.SizeOf(typeof(OpenFileName));
            ofn.hwndOwner = owner;
            ofn.lpstrFilter = filter;
            ofn.nFilterIndex = 1;
            ofn.lpstrFile = file;
            ofn.nMaxFile = Buffer;
            ofn.lpstrTitle = title;
            ofn.Flags = OFN_EXPLORER | OFN_FILEMUSTEXIST | OFN_PATHMUSTEXIST | OFN_NOCHANGEDIR;
            if (GetOpenFileNameW(ref ofn))
            {
                string path = Marshal.PtrToStringUni(file);
                FileInfo info = new FileInfo(path);
                if (info.Exists && info.Length > 0 && info.Length <= 32L * 1024L * 1024L)
                {
                    File.Copy(path, Path.Combine(cacheDir, "picked.tmp"), true);
                    string zip = Path.Combine(cacheDir, "picked.zip");
                    if (File.Exists(zip)) File.Delete(zip);
                    File.Move(Path.Combine(cacheDir, "picked.tmp"), zip);
                    File.WriteAllText(Path.Combine(cacheDir, "picked.name"), Path.GetFileName(path));
                    status = "ok";
                }
            }
            else
            {
                status = "cancel";
            }
        }
        catch (Exception)
        {
            status = "error";
        }
        finally
        {
            if (file != IntPtr.Zero) Marshal.FreeHGlobal(file);
            if (filter != IntPtr.Zero) Marshal.FreeHGlobal(filter);
            if (title != IntPtr.Zero) Marshal.FreeHGlobal(title);
        }
        try { File.WriteAllText(Path.Combine(cacheDir, "picked.done"), status); } catch (Exception) { }
    }
#else
    public static bool Launch(string cacheDir)
    {
        return false;
    }
#endif
}
