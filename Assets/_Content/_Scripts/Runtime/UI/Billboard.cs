using UnityEngine;

public class Billboard : MonoBehaviour
{
    [Header("Settings")]
    public bool billboardX = true;
    public bool billboardY = true;
    public bool billboardZ = true;
    public bool reverseFace = false;

    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;

        if (mainCamera == null)
        {
            mainCamera = FindFirstObjectByType<Camera>();
        }
    }

    void LateUpdate()
    {
        if (mainCamera == null) return;

        Vector3 targetPos = transform.position + mainCamera.transform.rotation * (reverseFace ? Vector3.forward : Vector3.back);
        Vector3 targetOrientation = mainCamera.transform.rotation * Vector3.up;

        transform.LookAt(targetPos, targetOrientation);

        Vector3 eulerAngles = transform.eulerAngles;
        if (!billboardX) eulerAngles.x = 0;
        if (!billboardY) eulerAngles.y = 0;
        if (!billboardZ) eulerAngles.z = 0;

        transform.eulerAngles = eulerAngles;
    }
}