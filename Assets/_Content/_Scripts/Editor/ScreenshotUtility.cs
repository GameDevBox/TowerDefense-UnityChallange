using System.IO;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
public class ScreenshotUtility : EditorWindow
{
    private GameObject targetPrefab;
    private Vector2 imageSize = new Vector2(256, 256);
    private Color backgroundColor = Color.clear;
    private string saveFolderPath = "Assets/Previews/";
    private Camera renderCamera;
    private Light renderLight;
    private Vector3 cameraOffset = new Vector3(0, 0, -3);
    private Vector3 modelRotation = new Vector3(0, 45, 0);

    // NEW: Preview variables
    private RenderTexture previewTexture;
    private GameObject previewInstance;
    private Camera sceneCamera;
    private bool useSceneCamera = false;
    private Vector2 scrollPosition;
    private float previewSize = 200f;

    [MenuItem("Tools/Screenshot Utility")]
    public static void ShowWindow()
    {
        GetWindow<ScreenshotUtility>("Screenshot Utility");
    }

    void OnEnable()
    {
        // Create preview texture
        previewTexture = new RenderTexture(256, 256, 24);
        SceneView.duringSceneGui += DuringSceneGUI;
    }

    void OnDisable()
    {
        CleanupPreview();
        if (previewTexture != null)
        {
            previewTexture.Release();
            DestroyImmediate(previewTexture);
        }
        SceneView.duringSceneGui -= DuringSceneGUI;
    }

    void OnGUI()
    {
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

        GUILayout.Label("Prefab Screenshot Utility", EditorStyles.boldLabel);

        EditorGUI.BeginChangeCheck();
        targetPrefab = (GameObject)EditorGUILayout.ObjectField("Target Prefab", targetPrefab, typeof(GameObject), false);
        if (EditorGUI.EndChangeCheck())
        {
            UpdatePreview();
        }

        EditorGUILayout.Space();
        GUILayout.Label("Live Preview", EditorStyles.boldLabel);

        if (targetPrefab != null && previewTexture != null)
        {
            // Display preview texture
            Rect previewRect = GUILayoutUtility.GetRect(previewSize, previewSize);
            EditorGUI.DrawPreviewTexture(previewRect, previewTexture);

            // Preview controls
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Refresh Preview"))
            {
                UpdatePreview();
            }
            if (GUILayout.Button("Reset View"))
            {
                modelRotation = new Vector3(0, 45, 0);
                cameraOffset = new Vector3(0, 0, -3);
                UpdatePreview();
            }
            EditorGUILayout.EndHorizontal();
        }
        else
        {
            GUILayout.Label("No prefab selected for preview", EditorStyles.helpBox);
        }

        EditorGUILayout.Space();
        GUILayout.Label("Render Settings", EditorStyles.boldLabel);

        useSceneCamera = EditorGUILayout.Toggle("Use Scene Camera", useSceneCamera);
        if (useSceneCamera)
        {
            EditorGUILayout.HelpBox("Uses the current Scene view camera for screenshot", MessageType.Info);
        }
        else
        {
            imageSize = EditorGUILayout.Vector2Field("Image Size", imageSize);
            backgroundColor = EditorGUILayout.ColorField("Background Color", backgroundColor);
        }

        // Model and camera controls with live update
        EditorGUI.BeginChangeCheck();
        modelRotation = EditorGUILayout.Vector3Field("Model Rotation", modelRotation);
        cameraOffset = EditorGUILayout.Vector3Field("Camera Offset", cameraOffset);
        if (EditorGUI.EndChangeCheck() && targetPrefab != null)
        {
            UpdatePreview();
        }

        // Quick rotation buttons
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Quick Rotations:");
        if (GUILayout.Button("Front")) { modelRotation = Vector3.zero; UpdatePreview(); }
        if (GUILayout.Button("45°")) { modelRotation = new Vector3(0, 45, 0); UpdatePreview(); }
        if (GUILayout.Button("Side")) { modelRotation = new Vector3(0, 90, 0); UpdatePreview(); }
        if (GUILayout.Button("Back")) { modelRotation = new Vector3(0, 180, 0); UpdatePreview(); }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();
        saveFolderPath = EditorGUILayout.TextField("Save Folder", saveFolderPath);

        // Create folder button
        if (GUILayout.Button("Create Save Folder"))
        {
            CreateSaveFolder();
        }

        EditorGUILayout.Space();

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Take Screenshot", GUILayout.Height(30)))
        {
            if (targetPrefab != null)
            {
                TakeScreenshot();
            }
            else
            {
                EditorUtility.DisplayDialog("Error", "Please assign a target prefab!", "OK");
            }
        }

        if (GUILayout.Button("Screenshot Selected", GUILayout.Height(30)))
        {
            TakeScreenshotOfSelected();
        }
        EditorGUILayout.EndHorizontal();

        if (GUILayout.Button("Batch Screenshot All Prefabs in Folder"))
        {
            BatchScreenshotAllPrefabs();
        }

        EditorGUILayout.Space();
        GUILayout.Label("Instructions:", EditorStyles.boldLabel);
        GUILayout.Label("• Assign a prefab to see live preview", EditorStyles.wordWrappedLabel);
        GUILayout.Label("• Adjust rotation and camera offset in real-time", EditorStyles.wordWrappedLabel);
        GUILayout.Label("• Use quick rotation buttons for common angles", EditorStyles.wordWrappedLabel);
        GUILayout.Label("• Choose between custom camera or scene camera", EditorStyles.wordWrappedLabel);

        EditorGUILayout.EndScrollView();
    }

    void DuringSceneGUI(SceneView sceneView)
    {
        // Store scene camera reference
        sceneCamera = sceneView.camera;
    }

    void UpdatePreview()
    {
        CleanupPreview();

        if (targetPrefab == null) return;

        try
        {
            // Create temporary preview objects
            GameObject tempScene = new GameObject("PreviewScene");
            tempScene.hideFlags = HideFlags.HideAndDontSave;

            // Instantiate prefab
            previewInstance = PrefabUtility.InstantiatePrefab(targetPrefab) as GameObject;
            previewInstance.transform.SetParent(tempScene.transform);
            previewInstance.transform.position = Vector3.zero;
            previewInstance.transform.rotation = Quaternion.Euler(modelRotation);

            // Setup preview camera
            GameObject cameraObj = new GameObject("PreviewCamera");
            cameraObj.transform.SetParent(tempScene.transform);
            cameraObj.hideFlags = HideFlags.HideAndDontSave;
            Camera previewCam = cameraObj.AddComponent<Camera>();
            previewCam.transform.position = cameraOffset;
            previewCam.transform.LookAt(Vector3.zero);
            previewCam.backgroundColor = backgroundColor;
            previewCam.clearFlags = CameraClearFlags.SolidColor;
            previewCam.targetTexture = previewTexture;

            // Setup preview lighting
            GameObject lightObj = new GameObject("PreviewLight");
            lightObj.transform.SetParent(tempScene.transform);
            lightObj.hideFlags = HideFlags.HideAndDontSave;
            Light previewLight = lightObj.AddComponent<Light>();
            previewLight.type = LightType.Directional;
            previewLight.transform.rotation = Quaternion.Euler(50, -30, 0);
            previewLight.intensity = 1f;

            // Render preview
            previewCam.Render();

            // Cleanup temporary objects (they'll be destroyed immediately)
            DestroyImmediate(tempScene);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Preview update failed: {e.Message}");
        }
    }

    void CleanupPreview()
    {
        GameObject[] tempObjects = FindObjectsByType<GameObject>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (GameObject obj in tempObjects)
        {
            if (obj != null && (obj.name == "PreviewScene" || obj.transform.parent?.name == "PreviewScene"))
            {
                DestroyImmediate(obj);
            }
        }
        previewInstance = null;
    }

    void CreateSaveFolder()
    {
        if (!Directory.Exists(saveFolderPath))
        {
            Directory.CreateDirectory(saveFolderPath);
            AssetDatabase.Refresh();
            Debug.Log($"Created folder: {saveFolderPath}");
        }
    }

    void TakeScreenshotOfSelected()
    {
        GameObject selected = Selection.activeGameObject;
        if (selected != null)
        {
            targetPrefab = selected;
            UpdatePreview();
            TakeScreenshot();
        }
        else
        {
            EditorUtility.DisplayDialog("Error", "No GameObject selected in hierarchy!", "OK");
        }
    }

    void TakeScreenshot()
    {
        if (useSceneCamera && sceneCamera != null)
        {
            TakeScreenshotWithSceneCamera();
        }
        else
        {
            TakeScreenshotWithCustomCamera();
        }
    }

    void TakeScreenshotWithCustomCamera()
    {
        GameObject tempScene = new GameObject("TempScreenshotScene");

        try
        {
            // Instantiate the prefab
            GameObject instance = PrefabUtility.InstantiatePrefab(targetPrefab) as GameObject;
            instance.transform.SetParent(tempScene.transform);
            instance.transform.position = Vector3.zero;
            instance.transform.rotation = Quaternion.Euler(modelRotation);

            // Setup camera
            GameObject cameraObj = new GameObject("ScreenshotCamera");
            cameraObj.transform.SetParent(tempScene.transform);
            renderCamera = cameraObj.AddComponent<Camera>();
            renderCamera.transform.position = cameraOffset;
            renderCamera.transform.LookAt(Vector3.zero);
            renderCamera.backgroundColor = backgroundColor;
            renderCamera.clearFlags = CameraClearFlags.SolidColor;

            // Setup lighting
            GameObject lightObj = new GameObject("ScreenshotLight");
            lightObj.transform.SetParent(tempScene.transform);
            renderLight = lightObj.AddComponent<Light>();
            renderLight.type = LightType.Directional;
            renderLight.transform.rotation = Quaternion.Euler(50, -30, 0);
            renderLight.intensity = 1f;

            // Create render texture
            RenderTexture renderTexture = new RenderTexture((int)imageSize.x, (int)imageSize.y, 24);
            renderCamera.targetTexture = renderTexture;

            // Render
            renderCamera.Render();

            // Save to file
            SaveRenderTextureToFile(renderTexture);

            // Cleanup
            renderCamera.targetTexture = null;
            DestroyImmediate(renderTexture);
        }
        finally
        {
            DestroyImmediate(tempScene);
        }

        AssetDatabase.Refresh();
        EditorUtility.DisplayDialog("Success", $"Screenshot saved to: {saveFolderPath}", "OK");
    }

    void TakeScreenshotWithSceneCamera()
    {
        if (sceneCamera == null)
        {
            EditorUtility.DisplayDialog("Error", "No Scene camera found! Please make sure a Scene view is open.", "OK");
            return;
        }

        // Create render texture matching scene view size
        int width = (int)imageSize.x;
        int height = (int)imageSize.y;

        RenderTexture renderTexture = new RenderTexture(width, height, 24);
        Camera tempCamera = null;

        try
        {
            GameObject tempCamObj = new GameObject("TempSceneCamera");
            tempCamera = tempCamObj.AddComponent<Camera>();
            tempCamera.CopyFrom(sceneCamera);
            tempCamera.targetTexture = renderTexture;
            tempCamera.Render();

            SaveRenderTextureToFile(renderTexture);
        }
        finally
        {
            if (tempCamera != null)
            {
                tempCamera.targetTexture = null;
                DestroyImmediate(tempCamera.gameObject);
            }
            DestroyImmediate(renderTexture);
        }

        AssetDatabase.Refresh();
        EditorUtility.DisplayDialog("Success", $"Scene camera screenshot saved to: {saveFolderPath}", "OK");
    }

    void SaveRenderTextureToFile(RenderTexture renderTexture)
    {
        // Ensure directory exists
        if (!Directory.Exists(saveFolderPath))
        {
            Directory.CreateDirectory(saveFolderPath);
        }

        // Create texture2D from render texture
        RenderTexture.active = renderTexture;
        Texture2D texture = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGBA32, false);
        texture.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        texture.Apply();
        RenderTexture.active = null;

        // Encode to PNG
        byte[] bytes = texture.EncodeToPNG();

        // Generate filename
        string timestamp = System.DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss");
        string filename = $"{targetPrefab.name}_Preview_{timestamp}.png";
        string fullPath = Path.Combine(saveFolderPath, filename);

        // Save file
        File.WriteAllBytes(fullPath, bytes);

        DestroyImmediate(texture);

        Debug.Log($"Screenshot saved: {fullPath}");
    }

    void BatchScreenshotAllPrefabs()
    {
        string folderPath = EditorUtility.OpenFolderPanel("Select Folder with Prefabs", "Assets", "");
        if (string.IsNullOrEmpty(folderPath)) return;

        folderPath = "Assets" + folderPath.Replace(Application.dataPath, "");
        string[] prefabGuids = AssetDatabase.FindAssets("t:Prefab", new[] { folderPath });

        if (prefabGuids.Length == 0)
        {
            EditorUtility.DisplayDialog("Info", "No prefabs found in selected folder!", "OK");
            return;
        }

        int successCount = 0;
        for (int i = 0; i < prefabGuids.Length; i++)
        {
            string guid = prefabGuids[i];
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);

            if (prefab != null)
            {
                targetPrefab = prefab;
                UpdatePreview();
                TakeScreenshotWithCustomCamera();
                successCount++;

                float progress = (float)i / prefabGuids.Length;
                if (EditorUtility.DisplayCancelableProgressBar("Batch Screenshot",
                    $"Processing {prefab.name} ({i + 1}/{prefabGuids.Length})", progress))
                {
                    break;
                }
            }
        }

        EditorUtility.ClearProgressBar();
        EditorUtility.DisplayDialog("Complete",
            $"Successfully captured {successCount} screenshots!", "OK");
    }
}
#endif