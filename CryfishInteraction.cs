using UnityEngine;
using System.Collections;

public class CrayfishInteraction : MonoBehaviour
{
    public GameObject interactUI;
    public Transform focusPoint;
    public AIController aiController;
    private bool canInteract = false;
    private bool hasInteracted = false;
    private bool interactionInProgress = false;
    private Transform player;
    public float turnSpeed = 2f; // Slow turning

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
                    "What if your home was dissolving around you? The endangered crayfish Austropotamobius pallipes is struggling to survive as freshwater acidification erodes its habitat.\r\n",
                    () =>
                    {
                        aiController.enabled = true; // Re-enable AI
                        interactionInProgress = false;
                        QuestManager.Instance.CompleteQuest(1, interactUI);
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
