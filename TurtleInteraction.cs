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
        for (int i = 0; i < 20; i++) // Check first 20 button indices
        {
            if (Input.GetKeyDown("joystick button " + i))
            {
                Debug.Log($"Turtle Interaction - Button {i} pressed");
                return true;
            }
        }
        
        // Also check keyboard E for testing
        return Input.GetKeyDown(KeyCode.E);
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
