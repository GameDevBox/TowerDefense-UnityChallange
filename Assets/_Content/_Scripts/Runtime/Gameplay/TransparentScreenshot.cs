using UnityEngine;
using System.IO;

public class TransparentScreenshot : MonoBehaviour
{
    [Header("Screenshot Settings")]
    public Camera targetCamera;
    public string folderName = "Assets/Screenshots";
    public string fileName = "screenshot.png";
    public int resolutionWidth = 512;
    public int resolutionHeight = 512;

    [ContextMenu("Capture Transparent Screenshot")]
    public void CaptureTransparentScreenshot()
    {
        if (targetCamera == null)
        {
            DebugLogsManager.LogError("No camera assigned!");
            return;
        }

        if (!Directory.Exists(folderName))
            Directory.CreateDirectory(folderName);

        RenderTexture rt = new RenderTexture(resolutionWidth, resolutionHeight, 24, RenderTextureFormat.ARGB32);
        targetCamera.targetTexture = rt;

        Texture2D tex = new Texture2D(resolutionWidth, resolutionHeight, TextureFormat.ARGB32, false);

        Color oldColor = targetCamera.backgroundColor;
        CameraClearFlags oldFlags = targetCamera.clearFlags;

        targetCamera.clearFlags = CameraClearFlags.SolidColor;
        targetCamera.backgroundColor = new Color(0, 0, 0, 0); // transparent background

        targetCamera.Render();

        RenderTexture.active = rt;
        tex.ReadPixels(new Rect(0, 0, resolutionWidth, resolutionHeight), 0, 0);
        tex.Apply();

        targetCamera.targetTexture = null;
        RenderTexture.active = null;
        targetCamera.clearFlags = oldFlags;
        targetCamera.backgroundColor = oldColor;
        DestroyImmediate(rt);

        byte[] pngData = tex.EncodeToPNG();
        DestroyImmediate(tex);

        string filePath = Path.Combine(folderName, fileName);
        File.WriteAllBytes(filePath, pngData);

        DebugLogsManager.Log($"✅ Screenshot saved to: {filePath}");
    }
}
