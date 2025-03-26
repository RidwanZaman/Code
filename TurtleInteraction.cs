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
        if (canInteract && !hasInteracted && !interactionInProgress &&
            (Input.GetKeyDown(KeyCode.JoystickButton4) || Input.GetKeyDown(KeyCode.JoystickButton5) || Input.GetKeyDown(KeyCode.E)))
        {
            hasInteracted = true;
            interactionInProgress = true;
            interactUI.SetActive(false);

            aiController.enabled = false;

            StartCoroutine(SmoothFacePlayer(() =>
            {
                InteractionManager.Instance.StartInteraction(
                    focusPoint,
                    "The temperature of sand can decide the fate of an entire species. Too warm, and almost all hatchlings will be female—too cold, and survival rates drop.\r\n",
                    () =>
                    {
                 
                        interactionInProgress = false;
                        QuestManager.Instance.CompleteQuest(2, interactUI);
                        aiController.enabled = true; // Enable AI after
                    });
            }));
        }
    }

    IEnumerator SmoothFacePlayer(System.Action onDone)
    {
        Quaternion startRot = transform.rotation;
        Vector3 dir = (player.position - transform.position).normalized;
        Quaternion endRot = Quaternion.LookRotation(new Vector3(dir.x, 0, dir.z));

        float t = 0;
        while (t < 1)
        {
            t += Time.deltaTime * turnSpeed;
            transform.rotation = Quaternion.Slerp(startRot, endRot, t);
            yield return null;
        }

        onDone?.Invoke();
    }

    private void OnTriggerEnter(Collider other) { if (other.CompareTag("Player")) canInteract = true; }
    private void OnTriggerExit(Collider other) { canInteract = false; }
}
