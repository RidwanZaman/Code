using UnityEngine;

public class SeaStarInteraction : MonoBehaviour
{
    public GameObject interactUI;
    public GameObject guidingQPanel;
    public Transform focusPoint;

    private bool canInteract = false;
    private bool hasInteracted = false;
    private bool interactionInProgress = false;

    void Start()
    {
        interactUI.SetActive(true);
        guidingQPanel.SetActive(true); 
    }

    void Update()
    {
        // Detect side trigger press using Unity's preexisting input system
        if (canInteract && !hasInteracted && !interactionInProgress &&
            (Input.GetKeyDown(KeyCode.JoystickButton4) || Input.GetKeyDown(KeyCode.JoystickButton5) || Input.GetKeyDown(KeyCode.E)))
        {
            hasInteracted = true;
            interactionInProgress = true;
            interactUI.SetActive(false);

            // Start interaction sequence
            InteractionManager.Instance.StartInteraction(
                focusPoint,
                "Imagine a single creature keeping entire forests alive. Sunflower sea stars devour five sea urchins a week—without them, kelp forests would collapse.\r\n",
                () =>
                {
                    interactionInProgress = false;
                    QuestManager.Instance.CompleteQuest(0, interactUI);
                });
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !hasInteracted)
        {
            canInteract = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            canInteract = false;
        }
    }
}
