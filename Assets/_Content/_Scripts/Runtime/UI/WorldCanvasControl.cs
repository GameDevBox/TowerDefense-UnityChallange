using UnityEngine;

public class WorldCanvasControl : MonoBehaviour
{
    private Canvas canvas;
    private void Start()
    {
        canvas = GetComponent<Canvas>();

        if (!canvas)
            canvas.worldCamera = Camera.main;
    }
}
