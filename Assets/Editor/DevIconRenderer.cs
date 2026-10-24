using System;
using System.IO;
using UnityEditor;
using UnityEngine;

// Renders the pictures of the Dev Items that have none (transparent PNG, 2x the size of an atlas icon: 304x336).
// Run once, with graphics (no -nographics), env BBR_ICON_DIR = output folder:
//   Unity.exe -batchmode -projectPath <p> -executeMethod DevIconRenderer.Run -quit
public static class DevIconRenderer
{
    private struct Job
    {
        public string name;
        public string prefab;
        public Vector3 euler;
        public float fill;

        public Job(string name, string prefab, Vector3 euler, float fill)
        {
            this.name = name;
            this.prefab = prefab;
            this.euler = euler;
            this.fill = fill;
        }
    }

    public static void Run()
    {
        string outDir = Environment.GetEnvironmentVariable("BBR_ICON_DIR");
        if (string.IsNullOrEmpty(outDir)) outDir = "Temp/DevIcons";
        Directory.CreateDirectory(outDir);
        Job[] jobs = new Job[]
        {
            new Job("chicken", "Assets/Resources/playsomething/units/decorations/goldenchicken/DecorationalGoldenChicken.prefab", new Vector3(0f, 0f, 0f), 0.8f),
            new Job("earthgoal", "Assets/Resources/playsomething/units/static/goalgate/GoalGatePrefab.prefab", new Vector3(0f, 0f, 0f), 0.8f),
            new Job("metalplatform", "Assets/Resources/playsomething/units/dynamic/metalplatforms/MetalPlatformPrefab.prefab", new Vector3(0f, 0f, 0f), 0.9f),
            new Job("boelb", "Assets/Resources/playsomething/units/characters/boelb/Boelb.prefab", new Vector3(0f, 0f, 0f), 0.8f),
        };
        int w = 304, h = 336;
        for (int j = 0; j < jobs.Length; j++)
        {
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(jobs[j].prefab);
            if (prefab == null)
            {
                Debug.LogError("ICON missing prefab " + jobs[j].prefab);
                continue;
            }
            GameObject go = (GameObject)UnityEngine.Object.Instantiate(prefab, Vector3.zero, Quaternion.Euler(jobs[j].euler));
            Renderer[] renderers = go.GetComponentsInChildren<Renderer>(true);
            if (renderers.Length == 0)
            {
                Debug.LogError("ICON no renderer in " + jobs[j].prefab);
                UnityEngine.Object.DestroyImmediate(go);
                continue;
            }
            Bounds b = renderers[0].bounds;
            for (int r = 1; r < renderers.Length; r++) b.Encapsulate(renderers[r].bounds);

            GameObject camGo = new GameObject("IconCam");
            Camera cam = camGo.AddComponent<Camera>();
            cam.orthographic = true;
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.backgroundColor = new Color(0f, 0f, 0f, 0f);
            float halfH = Mathf.Max(b.extents.y, b.extents.x * h / w) / jobs[j].fill;
            cam.orthographicSize = halfH;
            cam.aspect = (float)w / h;
            cam.transform.position = b.center + new Vector3(0f, 0f, -10f * Mathf.Max(1f, b.extents.magnitude));
            cam.transform.rotation = Quaternion.identity;
            cam.nearClipPlane = 0.01f;
            cam.farClipPlane = 1000f;

            GameObject lightGo = new GameObject("IconLight");
            Light light = lightGo.AddComponent<Light>();
            light.type = LightType.Directional;
            light.intensity = 1.1f;
            lightGo.transform.rotation = Quaternion.Euler(35f, -30f, 0f);
            RenderSettings.ambientLight = new Color(0.7f, 0.7f, 0.72f);

            RenderTexture rt = new RenderTexture(w, h, 24, RenderTextureFormat.ARGB32);
            rt.antiAliasing = 8;
            cam.targetTexture = rt;
            cam.Render();
            RenderTexture.active = rt;
            Texture2D tex = new Texture2D(w, h, TextureFormat.ARGB32, false);
            tex.ReadPixels(new Rect(0, 0, w, h), 0, 0);
            tex.Apply();
            RenderTexture.active = null;
            File.WriteAllBytes(outDir + "/" + jobs[j].name + ".png", tex.EncodeToPNG());
            Debug.Log("ICON rendered " + jobs[j].name + " bounds " + b.size);
            cam.targetTexture = null;
            UnityEngine.Object.DestroyImmediate(rt);
            UnityEngine.Object.DestroyImmediate(tex);
            UnityEngine.Object.DestroyImmediate(camGo);
            UnityEngine.Object.DestroyImmediate(lightGo);
            UnityEngine.Object.DestroyImmediate(go);
        }
    }
}
