using UnityEngine;
using UnityEngine.SceneManagement;

public class JellyfishGuide : MonoBehaviour
{
    public static JellyfishGuide Instance;

    // UI Elements
    public GameObject dialogueUI;
    public GameObject interactUI;
    public TMPro.TextMeshProUGUI dialogueText;

    // State
    private int currentStep = 0;
    private bool surveyActive = false;

    private string[] dialogues = {
        "Welcome! please answer our survey",
        "Let's do a quick survey (Press trigger)",
        "Survey complete! (Press trigger)",
        "Press menu button to begin"
    };

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        // Hide all UI initially
        dialogueUI.SetActive(true);
        interactUI.SetActive(false);


        ShowCurrentDialogue();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E)) return;

    }

    void AdvanceDialogue()
    {
        currentStep++;

        // Start survey when reaching step 1
        if (currentStep == 1)
        {
            StartSurvey();
            return;
        }

        // End tutorial at final step
        if (currentStep >= dialogues.Length)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
            return;
        }

        ShowCurrentDialogue();
    }

    void StartSurvey()
    {
        surveyActive = true;

        dialogueUI.SetActive(false);
    }

    void EndSurvey()
    {
        surveyActive = false;
        currentStep = 2; // Skip to post-survey message
        ShowCurrentDialogue();
    }

    void ShowCurrentDialogue()
    {
        if (currentStep < dialogues.Length)
        {
            dialogueText.text = dialogues[currentStep];
            dialogueUI.SetActive(true);
        }
    }

    // For the interaction tutorial
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            interactUI.SetActive(true);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            interactUI.SetActive(false);
    }
}
