using UnityEngine;

public class MusselInteraction2 : MonoBehaviour
{
    public GameObject interactUI;
    public GameObject guidingQPanel;
    public GameObject MusselPrefab;
    public GameObject MusselCrushed;
    public Renderer musselRenderer;
    public Transform focusPoint;

    public Color damagedColor = Color.white;
    public float colorShiftDuration = 2f;

    private bool canInteract = false;
    private bool hasInteracted = false;
    private bool interactionInProgress = false;
    private float colorShiftTimer = 0f;
    private bool colorShifting = false;

    void Start()
    {
        MusselCrushed.SetActive(false);
        interactUI.SetActive(true);
        guidingQPanel.SetActive(false);

        if (musselRenderer == null)
            musselRenderer = GetComponent<Renderer>();

        Debug.Log("Mussel interaction initialized");
    }

    void Update()
    {
        if (canInteract && !hasInteracted && !interactionInProgress && AnyVRInputPressed())
        {
            StartInteraction();
        }

        if (colorShifting)
        {
            UpdateColorShift();
        }
    }

    private bool AnyVRInputPressed()
    {
        // Check all common VR controller buttons
        return Input.GetKeyDown(KeyCode.JoystickButton0) ||  // Typically 'A' on Oculus, 'X' on Vive
               Input.GetKeyDown(KeyCode.JoystickButton1) ||  // Typically 'B' on Oculus, 'Y' on Vive
               Input.GetKeyDown(KeyCode.JoystickButton2) ||  // Typically 'X' on Oculus, unused on Vive
               Input.GetKeyDown(KeyCode.JoystickButton3) ||  // Typically 'Y' on Oculus, unused on Vive
               Input.GetKeyDown(KeyCode.JoystickButton4) ||  // Typically left shoulder button
               Input.GetKeyDown(KeyCode.JoystickButton5) ||  // Typically right shoulder button
               Input.GetKeyDown(KeyCode.JoystickButton6) ||  // Typically left stick click
               Input.GetKeyDown(KeyCode.JoystickButton7) ||  // Typically right stick click
               Input.GetKeyDown(KeyCode.JoystickButton8) ||  // Often start button
               Input.GetKeyDown(KeyCode.JoystickButton9) ||  // Often back/select button
               Input.GetKeyDown(KeyCode.JoystickButton10) || // Sometimes left controller menu
               Input.GetKeyDown(KeyCode.JoystickButton11) || // Sometimes right controller menu
               Input.GetKeyDown(KeyCode.JoystickButton12) || // Extra buttons
               Input.GetKeyDown(KeyCode.JoystickButton13) ||
               Input.GetKeyDown(KeyCode.JoystickButton14) ||
               Input.GetKeyDown(KeyCode.JoystickButton15) ||
               Input.GetKeyDown(KeyCode.JoystickButton16) ||
               Input.GetKeyDown(KeyCode.JoystickButton17) ||
               Input.GetKeyDown(KeyCode.JoystickButton18) ||
               Input.GetKeyDown(KeyCode.JoystickButton19) ||
               Input.GetKeyDown(KeyCode.E); // Keyboard fallback
    }

    private void StartInteraction()
    {
        hasInteracted = true;
        interactionInProgress = true;
        interactUI.SetActive(false);
        guidingQPanel.SetActive(false);

        StartColorShift();
        InteractionManager.Instance.StartInteraction(
            focusPoint,
            "Due to ocean acidification,\nthis mussel's shell has weakened over time.",
            () =>
            {
                QuestManager2.Instance.CompleteQuest(3, interactUI);
                Debug.Log("Mussel interaction completed");
            });
    }

    private void StartColorShift()
    {
        colorShifting = true;
        colorShiftTimer = 0f;
        Debug.Log("Starting mussel color shift");
    }

    private void UpdateColorShift()
    {
        colorShiftTimer += Time.deltaTime;
        float lerpValue = Mathf.Clamp01(colorShiftTimer / colorShiftDuration);
        musselRenderer.material.color = Color.Lerp(musselRenderer.material.color, damagedColor, lerpValue);

        if (lerpValue >= 1f)
        {
            FinishInteraction();
        }
    }

    private void FinishInteraction()
    {
        colorShifting = false;
        MusselCrushed.SetActive(true);
        MusselPrefab.SetActive(false);
        interactionInProgress = false;
        Debug.Log("Mussel transformation complete");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !hasInteracted)
        {
            canInteract = true;
            guidingQPanel.SetActive(true);
            Debug.Log("Player entered mussel interaction zone");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            canInteract = false;
            guidingQPanel.SetActive(false);
            Debug.Log("Player exited mussel interaction zone");
        }
    }

    public void ResetInteraction()
    {
        StopAllCoroutines();
        canInteract = false;
        hasInteracted = false;
        interactionInProgress = false;
        colorShifting = false;
        colorShiftTimer = 0f;

        interactUI.SetActive(true);
        guidingQPanel.SetActive(false);
        MusselPrefab.SetActive(true);
        MusselCrushed.SetActive(false);

        if (musselRenderer != null)
            musselRenderer.material.color = Color.white;

        Debug.Log("Mussel interaction reset");
    }
}
