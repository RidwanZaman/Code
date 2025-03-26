using UnityEngine;

public class OctopusInteraction : MonoBehaviour
{
    public GameObject interactUI;
    public Transform focusPoint; // Empty GameObject to focus camera
    private bool canInteract = false;
    private bool hasInteracted = false;
    private bool interactionInProgress = false;

    void Start() { interactUI.SetActive(true); }

    void Update()
    {
        if (canInteract && !hasInteracted && !interactionInProgress &&
            (Input.GetKeyDown(KeyCode.JoystickButton4) || Input.GetKeyDown(KeyCode.JoystickButton5) || Input.GetKeyDown(KeyCode.E)))
        {
            hasInteracted = true;
            interactionInProgress = true;
            interactUI.SetActive(false);

            InteractionManager.Instance.StartInteraction(
                focusPoint,
                "Masters of disguise, problem-solvers, and tool users—octopuses rely on coral reefs for shelter and survival. But as climate change destroys these reefs, their future hangs in the balance.\r\n",
                () => {
                    interactionInProgress = false;
                    QuestManager.Instance.CompleteQuest(3, interactUI); 
                });
        }
    }

    private void OnTriggerEnter(Collider other) { if (other.CompareTag("Player")) canInteract = true; }
    private void OnTriggerExit(Collider other) { canInteract = false; }
}
