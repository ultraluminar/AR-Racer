using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;

public class TapToPlace : MonoBehaviour
{
    [SerializeField] private Text debugText;
    [SerializeField] private GameObject objectToPlace;

    private PlayerInput playerInput;
    private InputAction touchAction;

    private ARPlaneManager planeManager;

    private bool objectPlaced = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (debugText != null)
        {
            debugText.text = "Tap to place the object in the scene.";
        }
        else
        {
            Debug.LogError("Debug Text reference not set in the Inspector.");
        }
    }

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        touchAction = playerInput.actions.FindAction("Touch");

        planeManager = FindFirstObjectByType<ARPlaneManager>();
    }

    private void OnEnable()
    {
        touchAction.performed += OnTap;
    }

    private void OnDisable()
    {
        touchAction.performed -= OnTap;
    }

    private void OnTap(InputAction.CallbackContext context)
    {
        Vector2 touchPosition = context.ReadValue<Vector2>();
        printDebugText("Tap detected! Position: " + touchPosition);
        // ar ray casting to see if the touch position hits a surface
        Ray ray = Camera.main.ScreenPointToRay(touchPosition);
        bool hitDetected = Physics.Raycast(ray, out RaycastHit hitInfo);
        if (!hitDetected)
        {
            printDebugText("Missed! :(");
            return;
        }
        if (objectPlaced)
        {
            printDebugText("Object already placed. Reset to place a new one.");
            return;
        }
        printDebugText("Hit! - " + hitInfo.point);
        // Instantiate an object at the hit point
        GameObject placedObject = Instantiate(objectToPlace, hitInfo.point, Quaternion.FromToRotation(Vector3.up, hitInfo.normal));
        planeManager.SetTrackablesActive(false);
        planeManager.enabled = false;
        objectPlaced = true;
    }

    private void printDebugText(string message)
    {
        if (debugText == null)
        {
            Debug.LogError("Debug Text reference not set in the Inspector.");
            return;
        }
        debugText.text = message;
    }
}
