using UnityEngine;
using System.Collections;
using System.IO;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class SimpleScreenshot : MonoBehaviour
{
    [Header("Screenshot Settings")]
    public KeyCode screenshotKey = KeyCode.F12;
    public string folderName = "Screenshots";
    public int superSize = 1;
    public bool includeTimestamp = true;
    public bool transparentBackground = true;

    [Header("Save Location")]
    [SerializeField] private string customSavePath = "";
    public bool useCustomPath = false;

    void Update()
    {
        if (Input.GetKeyDown(screenshotKey))
        {
            TakeScreenshot();
        }
    }

    public void TakeScreenshot()
    {
        string folderPath = GetSaveFolderPath();

        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        // Generate filename
        string timestamp = includeTimestamp ?
            System.DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") : "";
        string filename = $"Screenshot{timestamp}.png";
        string fullPath = Path.Combine(folderPath, filename);

        // Take screenshot
        ScreenCapture.CaptureScreenshot(fullPath, superSize);

        Debug.Log($"Screenshot saved: {fullPath}");

#if UNITY_EDITOR
        // Refresh AssetDatabase if saving to Assets folder
        if (folderPath.StartsWith(Application.dataPath))
        {
            AssetDatabase.Refresh();
        }
#endif
    }

    // Method to take screenshot of specific camera with transparency
    public void TakeCameraScreenshot(Camera camera, string filename = "CameraScreenshot.png")
    {
        StartCoroutine(CaptureCameraScreenshot(camera, filename));
    }

    private IEnumerator CaptureCameraScreenshot(Camera camera, string filename)
    {
        yield return new WaitForEndOfFrame();

        CameraClearFlags originalClearFlags = camera.clearFlags;
        Color originalBackgroundColor = camera.backgroundColor;

        if (transparentBackground)
        {
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = new Color(0, 0, 0, 0); // Transparent black
        }

        RenderTexture renderTexture = new RenderTexture(Screen.width, Screen.height, 24, RenderTextureFormat.ARGB32);
        camera.targetTexture = renderTexture;

        // Render
        camera.Render();

        // Read pixels with alpha channel
        RenderTexture.active = renderTexture;
        Texture2D screenshot = new Texture2D(Screen.width, Screen.height, TextureFormat.RGBA32, false);
        screenshot.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        screenshot.Apply();

        // Restore original camera settings
        camera.targetTexture = null;
        camera.clearFlags = originalClearFlags;
        camera.backgroundColor = originalBackgroundColor;
        RenderTexture.active = null;

        // Save to file
        byte[] bytes = screenshot.EncodeToPNG();
        string folderPath = GetSaveFolderPath();
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }
        string fullPath = Path.Combine(folderPath, filename);
        File.WriteAllBytes(fullPath, bytes);

        // Cleanup
        DestroyImmediate(renderTexture);
        DestroyImmediate(screenshot);

        Debug.Log($"Camera screenshot saved: {fullPath}");

#if UNITY_EDITOR
        // Refresh AssetDatabase if saving to Assets folder
        if (folderPath.StartsWith(Application.dataPath))
        {
            AssetDatabase.Refresh();
        }
#endif
    }

    public void TakeCameraScreenshotWithBackground(Camera camera, Color backgroundColor, string filename = "CameraScreenshot.png")
    {
        StartCoroutine(CaptureCameraScreenshotWithBackground(camera, backgroundColor, filename));
    }

    private IEnumerator CaptureCameraScreenshotWithBackground(Camera camera, Color backgroundColor, string filename)
    {
        yield return new WaitForEndOfFrame();

        // Store original camera settings
        CameraClearFlags originalClearFlags = camera.clearFlags;
        Color originalBackgroundColor = camera.backgroundColor;

        // Set up camera with custom background
        camera.clearFlags = CameraClearFlags.SolidColor;
        camera.backgroundColor = backgroundColor;

        // Create render texture
        RenderTexture renderTexture = new RenderTexture(Screen.width, Screen.height, 24, RenderTextureFormat.ARGB32);
        camera.targetTexture = renderTexture;

        // Render
        camera.Render();

        // Read pixels
        RenderTexture.active = renderTexture;
        Texture2D screenshot = new Texture2D(Screen.width, Screen.height, TextureFormat.RGBA32, false);
        screenshot.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        screenshot.Apply();

        // Restore original camera settings
        camera.targetTexture = null;
        camera.clearFlags = originalClearFlags;
        camera.backgroundColor = originalBackgroundColor;
        RenderTexture.active = null;

        // Save to file
        byte[] bytes = screenshot.EncodeToPNG();
        string folderPath = GetSaveFolderPath();
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }
        string fullPath = Path.Combine(folderPath, filename);
        File.WriteAllBytes(fullPath, bytes);

        // Cleanup
        DestroyImmediate(renderTexture);
        DestroyImmediate(screenshot);

        Debug.Log($"Camera screenshot with background saved: {fullPath}");

#if UNITY_EDITOR
        // Refresh AssetDatabase if saving to Assets folder
        if (folderPath.StartsWith(Application.dataPath))
        {
            AssetDatabase.Refresh();
        }
#endif
    }

    public string GetSaveFolderPath()
    {
        if (useCustomPath && !string.IsNullOrEmpty(customSavePath))
        {
            // If custom path is relative to Assets folder, make it absolute
            if (customSavePath.StartsWith("Assets/") || customSavePath.StartsWith("Assets\\"))
            {
                return Path.Combine(Application.dataPath, customSavePath.Substring(7));
            }
            return customSavePath;
        }
        else
        {
            // Default to Assets/Screenshots folder
            return Path.Combine(Application.dataPath, folderName);
        }
    }

    // Helper method to get the display path for UI
    public string GetDisplayPath()
    {
        string path = GetSaveFolderPath();
        if (path.StartsWith(Application.dataPath))
        {
            return "Assets" + path.Substring(Application.dataPath.Length);
        }
        return path;
    }
}