using UnityEngine;

public class MusselInteraction2 : MonoBehaviour
{
    public GameObject interactUI;
    public GameObject guidingQPanel; // NEW: Guiding panel
    public GameObject MusselPrefab;
    public GameObject MusselCrushed; // Crushed mussel model
    public Renderer musselRenderer;
    public Transform focusPoint; // Camera focus point

    public Color damagedColor = Color.white;
    public float colorShiftDuration = 2f;

    private bool canInteract = false;
    private bool hasInteracted = false;
    private float colorShiftTimer = 0f;
    private bool colorShifting = false;

    void Start()
    {
        MusselCrushed.SetActive(false);
        interactUI.SetActive(true);
        guidingQPanel.SetActive(false); // Hide initially
        if (musselRenderer == null)
            musselRenderer = GetComponent<Renderer>();
    }

    void Update()
    {
        if (canInteract && Input.GetKeyDown(KeyCode.E) && !hasInteracted)
        {
            hasInteracted = true;
            interactUI.SetActive(false);
            guidingQPanel.SetActive(false); // Disable guiding panel

            // Start camera focus & dialogue
            StartColorShift();
            InteractionManager.Instance.StartInteraction(
                focusPoint,
                "Due to ocean acidification,\nthis mussel’s shell has weakened over time.",
                () =>
                {
                    // After dialogue, complete quest
                    QuestManager2.Instance.CompleteQuest(3, interactUI);
                    guidingQPanel.SetActive(false); // Fully disable
                });
        }

        if (colorShifting)
        {
            colorShiftTimer += Time.deltaTime;
            float lerpValue = Mathf.Clamp01(colorShiftTimer / colorShiftDuration);
            musselRenderer.material.color = Color.Lerp(musselRenderer.material.color, damagedColor, lerpValue);

            if (lerpValue >= 1f)
            {
                FinishInteraction();
            }
        }
    }

    void StartColorShift()
    {
        colorShifting = true;
    }

    void FinishInteraction()
    {
        colorShifting = false;
        MusselCrushed.SetActive(true);
        MusselPrefab.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !hasInteracted)
        {
            canInteract = true;
            guidingQPanel.SetActive(true); // Show guiding panel
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            canInteract = false;
            guidingQPanel.SetActive(false);
        }
    }
}
