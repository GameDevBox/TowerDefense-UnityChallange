/*WASD / Arrow Keys: Move camera

Shift + WASD: Move faster

Mouse Wheel: Zoom in/out (changes height)

Right Mouse Button + Drag: Rotate camera
*/

using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 10f;
    public float fastMoveSpeed = 20f;
    public float edgeScrollSpeed = 8f;
    public float edgeScrollBoundary = 25f;

    [Header("Zoom Settings")]
    public float zoomSpeed = 50f;
    public float minHeight = 5f;
    public float maxHeight = 50f;
    public float zoomSmoothTime = 0.1f;

    [Header("Rotation Settings")]
    public float rotationSpeed = 3f;
    public bool allowRotation = true;
    public KeyCode rotationKey = KeyCode.Mouse1;

    [Header("Bounds Settings")]
    public bool useBounds = true;
    public Vector2 minBounds = new Vector2(-50f, -50f);
    public Vector2 maxBounds = new Vector2(50f, 50f);

    private Vector3 targetPosition;
    private float targetHeight;
    private float zoomVelocity;
    private Vector3 lastMousePosition;
    private bool isRotating = false;

    void Start()
    {
        targetPosition = transform.position;
        targetHeight = transform.position.y;

        // Make sure cursor is visible and unlocked for tower placement
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void Update()
    {
        HandleRotationInput();
        HandleMovementInput();
        HandleZoomInput();

        ApplySmoothMovement();
    }

    void HandleRotationInput()
    {
        if (!allowRotation) return;

        // Start rotating when right mouse button is pressed
        if (Input.GetKeyDown(rotationKey))
        {
            isRotating = true;
            lastMousePosition = Input.mousePosition;
            Cursor.visible = false;
        }

        // Stop rotating when right mouse button is released
        if (Input.GetKeyUp(rotationKey))
        {
            isRotating = false;
            Cursor.visible = true;
        }

        // Rotate camera while right mouse button is held down
        if (isRotating)
        {
            Vector3 currentMousePosition = Input.mousePosition;
            Vector3 mouseDelta = currentMousePosition - lastMousePosition;

            float rotationY = mouseDelta.x * rotationSpeed * Time.deltaTime;
            transform.RotateAround(transform.position, Vector3.up, rotationY);

            lastMousePosition = currentMousePosition;
        }
    }

    void HandleMovementInput()
    {
        Vector3 inputDirection = Vector3.zero;

        // Keyboard movement (WASD or Arrow Keys)
        inputDirection.x = Input.GetAxis("Horizontal");
        inputDirection.z = Input.GetAxis("Vertical");

        // Calculate movement speed (regular or fast with Shift)
        float currentMoveSpeed = Input.GetKey(KeyCode.LeftShift) ? fastMoveSpeed : moveSpeed;

        if (inputDirection != Vector3.zero)
        {
            Vector3 moveDirection = transform.TransformDirection(inputDirection);
            moveDirection.y = 0;
            moveDirection.Normalize();

            targetPosition += moveDirection * currentMoveSpeed * Time.deltaTime;
        }

        // Apply bounds
        if (useBounds)
        {
            targetPosition.x = Mathf.Clamp(targetPosition.x, minBounds.x, maxBounds.x);
            targetPosition.z = Mathf.Clamp(targetPosition.z, minBounds.y, maxBounds.y);
        }
    }

    void HandleZoomInput()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");

        if (scroll != 0)
        {
            targetHeight -= scroll * zoomSpeed * Time.deltaTime;
            targetHeight = Mathf.Clamp(targetHeight, minHeight, maxHeight);
        }
    }

    void ApplySmoothMovement()
    {
        float smoothedHeight = Mathf.SmoothDamp(transform.position.y, targetHeight, ref zoomVelocity, zoomSmoothTime);

        Vector3 newPosition = new Vector3(targetPosition.x, smoothedHeight, targetPosition.z);
        transform.position = newPosition;
    }

    #region Public Methods (Extra)

    /// <summary>
    /// Focus the camera on a specific world position while maintaining current height
    /// </summary>
    public void FocusOnPosition(Vector3 worldPosition)
    {
        targetPosition = new Vector3(worldPosition.x, targetPosition.y, worldPosition.z);
    }

    /// <summary>
    /// Focus the camera on a specific world position with specified height
    /// </summary>
    public void FocusOnPosition(Vector3 worldPosition, float height)
    {
        targetPosition = new Vector3(worldPosition.x, 0, worldPosition.z);
        targetHeight = Mathf.Clamp(height, minHeight, maxHeight);
    }

    /// <summary>
    /// Set camera bounds for movement
    /// </summary>
    public void SetBounds(Vector2 newMinBounds, Vector2 newMaxBounds)
    {
        minBounds = newMinBounds;
        maxBounds = newMaxBounds;
    }

    /// <summary>
    /// Enable or disable movement bounds
    /// </summary>
    public void EnableBounds(bool enable)
    {
        useBounds = enable;
    }

    /// <summary>
    /// Set zoom limits
    /// </summary>
    public void SetZoomLimits(float newMinHeight, float newMaxHeight)
    {
        minHeight = newMinHeight;
        maxHeight = newMaxHeight;
        targetHeight = Mathf.Clamp(targetHeight, minHeight, maxHeight);
    }

    /// <summary>
    /// Set current zoom level
    /// </summary>
    public void SetZoom(float height)
    {
        targetHeight = Mathf.Clamp(height, minHeight, maxHeight);
    }

    /// <summary>
    /// Get current zoom level (height)
    /// </summary>
    public float GetCurrentZoom()
    {
        return targetHeight;
    }

    /// <summary>
    /// Get current world position the camera is focused on
    /// </summary>
    public Vector3 GetCurrentFocusPosition()
    {
        return new Vector3(targetPosition.x, 0, targetPosition.z);
    }

    /// <summary>
    /// Enable or disable camera rotation
    /// </summary>
    public void SetRotationEnabled(bool enabled)
    {
        allowRotation = enabled;
        if (!enabled && isRotating)
        {
            isRotating = false;
            Cursor.visible = true;
        }
    }

    /// <summary>
    /// Set camera movement speed
    /// </summary>
    public void SetMoveSpeed(float speed)
    {
        moveSpeed = speed;
    }

    /// <summary>
    /// Set camera rotation speed
    /// </summary>
    public void SetRotationSpeed(float speed)
    {
        rotationSpeed = speed;
    }

    /// <summary>
    /// Reset camera to initial rotation (facing forward)
    /// </summary>
    public void ResetRotation()
    {
        transform.rotation = Quaternion.Euler(transform.eulerAngles.x, 0, transform.eulerAngles.z);
    }

    #endregion

    void OnDrawGizmosSelected()
    {
        if (useBounds)
        {
            Gizmos.color = Color.red;
            Vector3 center = new Vector3((minBounds.x + maxBounds.x) * 0.5f, 0, (minBounds.y + maxBounds.y) * 0.5f);
            Vector3 size = new Vector3(maxBounds.x - minBounds.x, 0.1f, maxBounds.y - minBounds.y);
            Gizmos.DrawWireCube(center, size);
        }
    }
}