using System.IO;
using UnityEditor;
using UnityEngine;

// Renders one picture per hat prefab (used by the hat chooser). Run once, with graphics (no -nographics):
//   Unity.exe -batchmode -projectPath <p> -executeMethod HatIconRenderer.Run -quit
public static class HatIconRenderer
{
    private const string Root = "Assets/Resources/playsomething/units/characters/alienprops/";

    // same order as HatChoice (index 1..9), i.e. as AlienCharacter.SetRandomHat
    private static readonly string[] Prefabs = new string[]
    {
        "baseballhat/AlienBaseballHatPrefab.prefab",
        "offroadcarhelmet/AlienHelmetPrefab.prefab",
        "barbarianhelmet/BarbarianHelmetPrefab.prefab",
        "cowboyhat/CowboyHatPrefab.prefab",
        "horseheadmask/HorseHeadMaskPrefab.prefab",
        "chickenhat/ChickenHatPrefab.prefab",
        "mushroomhat/MushroomHatPrefab.prefab",
        "paperbaghat/PaperBagHatPrefab.prefab",
        "deathmask/DeathMaskPrefab.prefab"
    };

    public static void Run()
    {
        string outDir = "Assets/Resources/BBR/hats";
        Directory.CreateDirectory(outDir);
        int size = 256;
        // the view rotation of each hat (euler angles of the object): they are authored for the rider's head, facing right
        for (int i = 0; i < Prefabs.Length; i++)
        {
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(Root + Prefabs[i]);
            if (prefab == null)
            {
                Debug.LogError("HAT missing prefab " + Prefabs[i]);
                continue;
            }
            GameObject go = (GameObject)Object.Instantiate(prefab, Vector3.zero, Quaternion.identity);
            go.transform.position = Vector3.zero;
            Renderer[] renderers = go.GetComponentsInChildren<Renderer>(true);
            if (renderers.Length == 0)
            {
                Debug.LogError("HAT no renderer in " + Prefabs[i]);
                Object.DestroyImmediate(go);
                continue;
            }
            Bounds b = renderers[0].bounds;
            for (int r = 1; r < renderers.Length; r++) b.Encapsulate(renderers[r].bounds);

            GameObject camGo = new GameObject("HatCam");
            Camera cam = camGo.AddComponent<Camera>();
            cam.orthographic = true;
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.backgroundColor = new Color(0.17f, 0.45f, 0.75f, 1f);
            cam.orthographicSize = Mathf.Max(b.extents.x, b.extents.y) * 1.25f;
            cam.transform.position = b.center + new Vector3(0f, 0f, -10f * Mathf.Max(1f, b.extents.magnitude));
            cam.transform.rotation = Quaternion.identity;
            cam.nearClipPlane = 0.01f;
            cam.farClipPlane = 1000f;
            cam.allowMSAA = true;

            GameObject lightGo = new GameObject("HatLight");
            Light light = lightGo.AddComponent<Light>();
            light.type = LightType.Directional;
            light.intensity = 1.1f;
            lightGo.transform.rotation = Quaternion.Euler(35f, -30f, 0f);
            RenderSettings.ambientLight = new Color(0.65f, 0.65f, 0.7f);

            RenderTexture rt = new RenderTexture(size, size, 24, RenderTextureFormat.ARGB32);
            rt.antiAliasing = 8;
            cam.targetTexture = rt;
            cam.Render();
            RenderTexture.active = rt;
            Texture2D tex = new Texture2D(size, size, TextureFormat.RGB24, false);
            tex.ReadPixels(new Rect(0, 0, size, size), 0, 0);
            tex.Apply();
            RenderTexture.active = null;
            File.WriteAllBytes(outDir + "/hat_" + (i + 1) + ".png", tex.EncodeToPNG());
            Debug.Log("HAT rendered " + Prefabs[i] + " bounds " + b.size);

            cam.targetTexture = null;
            Object.DestroyImmediate(rt);
            Object.DestroyImmediate(tex);
            Object.DestroyImmediate(camGo);
            Object.DestroyImmediate(lightGo);
            Object.DestroyImmediate(go);
        }
        AssetDatabase.Refresh();
        EditorApplication.Exit(0);
    }
}
