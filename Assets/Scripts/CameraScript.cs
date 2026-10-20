using UnityEngine;
using UnityEngine.InputSystem;

public class CameraScript : MonoBehaviour
{
    [Header("Mouse Look")]
    public float mouseSensitivity = 0.1f;
    public float lookLimit = 80.0f;
    

    [SerializeField] private GameObject head;
    private float verticalLook = 0f;
    private Vector2 lookInput;

    // Called automatically by PlayerInput component via "Send Messages"
    // OR bound programmatically via C# Input Actions
    public void OnLook(InputValue value)
    {
        lookInput = value.Get<Vector2>();
    }

    public void OnCancel(InputValue value)
    {
        if (value.isPressed)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        verticalLook = transform.localEulerAngles.x;
        if (verticalLook > 180f) verticalLook -= 360f;
    }

    void LateUpdate()
    {
        if (Mouse.current != null)
        {
            Vector2 mouseDelta = Mouse.current.delta.ReadValue();
            
            // Calculate pitch (up/down)
            float mouseY = mouseDelta.y * mouseSensitivity;
            verticalLook -= mouseY;
        }
        verticalLook = Mathf.Clamp(verticalLook, -lookLimit, lookLimit);

        transform.localRotation = Quaternion.Euler(verticalLook, transform.localRotation.eulerAngles.y, 0f);

        lookInput = Vector2.zero;
    }
}