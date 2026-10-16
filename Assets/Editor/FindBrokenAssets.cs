using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class FindBrokenAssets : EditorWindow
{
    private List<string> problematicFiles = new List<string>();
    private Vector2 scrollPos;

    [MenuItem("Tools/Find Corrupted AssetRipper Meshes")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(FindBrokenAssets), false, "AssetRipper Cleaner");
    }

    private void OnGUI()
    {
        GUILayout.Label("AssetRipper Broken Component Finder", EditorStyles.boldLabel);
        GUILayout.Space(10);

        if (GUILayout.Button("Scan Prefabs", GUILayout.Height(35)))
        {
            ScanPrefabs();
        }

        if (GUILayout.Button("Scan Scenes", GUILayout.Height(35)))
        {
            ScanScenes();
        }

        if (problematicFiles.Count > 0)
        {
            GUILayout.Space(15);
            GUILayout.Label(string.Format("Found {0} problematic asset(s):", problematicFiles.Count), EditorStyles.boldLabel);

            scrollPos = GUILayout.BeginScrollView(scrollPos);
            for (int i = 0; i < problematicFiles.Count; i++)
            {
                string path = problematicFiles[i];
                if (GUILayout.Button(path, EditorStyles.miniButton))
                {
                    UnityEngine.Object obj = AssetDatabase.LoadAssetAtPath(path, typeof(UnityEngine.Object));
                    Selection.activeObject = obj;
                    EditorGUIUtility.PingObject(obj);
                }
            }
            GUILayout.EndScrollView();
        }
    }

    private void ScanPrefabs()
    {
        problematicFiles.Clear();
        string[] guids = AssetDatabase.FindAssets("t:Prefab");

        try
        {
            for (int i = 0; i < guids.Length; i++)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[i]);
                EditorUtility.DisplayProgressBar("Scanning Prefabs...", path, (float)i / (float)guids.Length);

                GameObject prefab = AssetDatabase.LoadAssetAtPath(path, typeof(GameObject)) as GameObject;
                if (prefab != null)
                {
                    Component[] comps = prefab.GetComponentsInChildren<Component>(true);
                    for (int j = 0; j < comps.Length; j++)
                    {
                        // If Unity removed a corrupted MeshFilter/MeshRenderer, it leaves a null component slot
                        if (comps[j] == null)
                        {
                            if (!problematicFiles.Contains(path))
                            {
                                problematicFiles.Add(path);
                                UnityEngine.Debug.LogError(string.Format("<b>[BROKEN PREFAB]:</b> {0}", path), prefab);
                            }
                            break;
                        }
                    }
                }
            }
        }
        finally
        {
            EditorUtility.ClearProgressBar();
        }

        Debug.Log(string.Format("Prefab scan finished. Found <b>{0}</b> broken prefabs.", problematicFiles.Count));
    }

    private void ScanScenes()
    {
        problematicFiles.Clear();
        string[] guids = AssetDatabase.FindAssets("t:Scene");

        try
        {
            for (int i = 0; i < guids.Length; i++)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[i]);
                if (!path.StartsWith("Assets/")) continue;

                EditorUtility.DisplayProgressBar("Scanning Scenes...", path, (float)i / (float)guids.Length);

#if UNITY_5_3_OR_NEWER
                UnityEditor.SceneManagement.EditorSceneManager.OpenScene(path);
#else
                EditorApplication.OpenScene(path);
#endif

                GameObject[] allObjects = UnityEngine.Object.FindObjectsOfType(typeof(GameObject)) as GameObject[];
                if (allObjects != null)
                {
                    for (int r = 0; r < allObjects.Length; r++)
                    {
                        Component[] comps = allObjects[r].GetComponentsInChildren<Component>(true);
                        for (int j = 0; j < comps.Length; j++)
                        {
                            if (comps[j] == null)
                            {
                                if (!problematicFiles.Contains(path))
                                {
                                    problematicFiles.Add(path);
                                    Debug.LogError(string.Format("<b>[BROKEN SCENE OBJECT]:</b> Scene: {0} | Object: {1}", path, allObjects[r].name));
                                }
                                break;
                            }
                        }
                    }
                }
            }
        }
        finally
        {
            EditorUtility.ClearProgressBar();
        }

        Debug.Log(string.Format("Scene scan finished. Found <b>{0}</b> broken scenes.", problematicFiles.Count));
    }
}