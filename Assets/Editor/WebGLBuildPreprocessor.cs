#if UNITY_EDITOR

using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

public class WebGLBuildPreprocessor : IPreprocessBuildWithReport
{
    public int callbackOrder => 0;

    public void OnPreprocessBuild(BuildReport report)
    {
        if (report.summary.platform != BuildTarget.WebGL)
            return;

        string args = PlayerSettings.WebGL.emscriptenArgs ?? "";

        // Clamp NaN/Inf float-to-int casts
        if (!args.Contains("BINARYEN_TRAP_MODE"))
        {
            args += " -s BINARYEN_TRAP_MODE='clamp'";
        }

        // Allow WebAssembly heap memory to dynamically grow
        // when AutoGeometry/Physics allocates additional buffers.
        if (!args.Contains("ALLOW_MEMORY_GROWTH"))
        {
            args += " -s ALLOW_MEMORY_GROWTH=1";
        }

        PlayerSettings.WebGL.emscriptenArgs = args.Trim();

        Debug.Log(
            "[WebGL Build] Emscripten args applied: "
            + PlayerSettings.WebGL.emscriptenArgs
        );
    }
}

#endif