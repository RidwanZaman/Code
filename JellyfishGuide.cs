using UnityEngine;
using UnityEngine.SceneManagement;

public class JellyfishGuide : MonoBehaviour
{
    public static JellyfishGuide Instance;

    // UI Elements
    public GameObject dialogueUI;
    public GameObject interactUI;
    public GameObject[] surveySliders; // Assign all 3 sliders in Inspector
    public TMPro.TextMeshProUGUI dialogueText;

    // State
    private int currentStep = 0;
    private bool surveyActive = false;

    private string[] dialogues = {
        "Welcome! Move with thumbstick, interact with trigger",
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
        dialogueUI.SetActive(false);
        interactUI.SetActive(false);
        foreach (var slider in surveySliders)
            slider.SetActive(false);

        ShowCurrentDialogue();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E)) return;

        if (surveyActive)
        {
            AdvanceSurvey();
        }
        else
        {
            AdvanceDialogue();
        }
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
        surveySliders[0].SetActive(true); // Show first slider
        dialogueUI.SetActive(false);
    }

    void AdvanceSurvey()
    {
        // Hide current slider
        surveySliders[currentStep - 1].SetActive(false);

        // Show next slider or finish
        if (currentStep - 1 < surveySliders.Length - 1)
        {
            surveySliders[currentStep].SetActive(true);
            currentStep++;
        }
        else
        {
            EndSurvey();
        }
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
