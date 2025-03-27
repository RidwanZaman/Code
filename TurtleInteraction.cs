using UnityEngine;
using System.Collections;

public class TurtleInteraction : MonoBehaviour
{
    public GameObject interactUI;
    public Transform focusPoint;
    public AIController aiController;
    private bool canInteract = false;
    private bool hasInteracted = false;
    private bool interactionInProgress = false;
    private Transform player;
    public float turnSpeed = 2f;

    void Start()
    {
        interactUI.SetActive(true);
        aiController = GetComponent<AIController>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        if (canInteract && !hasInteracted && !interactionInProgress && AnyVRInputPressed())
        {
            StartInteraction();
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
        aiController.enabled = false;

        StartCoroutine(SmoothFacePlayer(() =>
        {
            InteractionManager.Instance.StartInteraction(
                focusPoint,
                "The temperature of sand can decide the fate of an entire species. Too warm, and almost all hatchlings will be female too cold, and survival rates drop.\r\n",
                () =>
                {
                    aiController.enabled = true;
                    interactionInProgress = false;
                    QuestManager.Instance.CompleteQuest(2, interactUI);
                });
        }));
    }

    IEnumerator SmoothFacePlayer(System.Action onDone)
    {
        Quaternion startRot = transform.rotation;
        Vector3 dir = (player.position - transform.position).normalized;
        dir.y = 0; // Keep horizontal rotation only
        Quaternion endRot = Quaternion.LookRotation(dir);

        float t = 0;
        while (t < 1)
        {
            t += Time.deltaTime * turnSpeed;
            transform.rotation = Quaternion.Slerp(startRot, endRot, t);
            yield return null;
        }

        try
        {
            onDone?.Invoke();
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Turtle interaction callback failed: {e.Message}");
            // Fallback: ensure AI is re-enabled if something fails
            aiController.enabled = true;
            interactionInProgress = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            canInteract = true;
            Debug.Log("Player entered turtle interaction zone");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            canInteract = false;
            Debug.Log("Player exited turtle interaction zone");
        }
    }

    // Safety method to reset interaction if needed
    public void ResetInteraction()
    {
        StopAllCoroutines();
        canInteract = false;
        hasInteracted = false;
        interactionInProgress = false;
        interactUI.SetActive(true);
        aiController.enabled = true;
    }
}
