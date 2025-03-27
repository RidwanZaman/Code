using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class JellyfishGuide : MonoBehaviour
{
    public static JellyfishGuide Instance;

    // UI Elements
    public GameObject dialogueUI;
    public GameObject interactUI;
    public GameObject questUI;
    public GameObject dataUI;
    public GameObject GuidingArrow;
    public GameObject Slider1;
    public GameObject Slider2;
    public GameObject Slider3;
    public TMPro.TextMeshProUGUI dialogueText;

    // State Tracking
    private int dialogueStep = 0;
    private bool isInteracting = false;
    private bool canInteract = false;
    private bool surveyInProgress = false;
    private int currentSurveyQuestion = 0;

    private string[] dialogues = {
        "Welcome! Let's learn VR movement. Push the thumbstick forward to move.", // 0
        "Now try turning by rotating the thumbstick left/right.", // 1
        "Anything with ! above them is interactable. Try finding something and interact with it. Go close to it and press the trigger. (Press trigger to continue)", // 2
        "Great job! Before we begin, we want to gauge your understanding of climate change first. (PRESS TRIGGER)", // 3
        "Question 1: How concerned are you about climate change's impact on ocean life? (1 = Not concerned at all, 5 = Extremely concerned)", // 4
        "Question 2: How much do you feel connected to species affected by climate change? (1 = Not connected, 5 = Strongly connected)", // 5
        "Question 3: How likely are you to take action to help prevent ocean damage? (1 = Not likely, 5 = Very likely)", // 6
        "I will appear later to check up on you. (Press trigger)", // 7
        "The red box shows quests, green shows data, and yellow points the way. (Press trigger)", // 8
        "Try to do quests in order. Press the menu button to start.", // 9
    };

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        InitializeUI();
        ShowDialogue();
    }

    void InitializeUI()
    {
        interactUI.SetActive(false);
        questUI.SetActive(false);
        dataUI.SetActive(false);
        GuidingArrow.SetActive(false);
        Slider1.SetActive(false);
        Slider2.SetActive(false);
        Slider3.SetActive(false);
    }

    void Update()
    {
        if (surveyInProgress)
        {
            HandleSurveyInput();
        }
        else if (AnyVRInputPressed())
        {
            HandleTutorialInput();
        }
    }

    void HandleTutorialInput()
    {
        switch (dialogueStep)
        {
            case 0: // Move forward
                if (AnyVRInputPressed()) NextDialogue();
                break;

            case 1: // Turning
                if (AnyVRInputPressed()) NextDialogue();
                break;

            case 2: // Interaction instruction
                if (AnyVRInputPressed())
                    StartCoroutine(EnableInteractionMode());
                break;

            case 3: // Start survey
                if (AnyVRInputPressed())
                    StartSurvey();
                break;

            case 7: // Post-survey instructions
            case 8: // UI explanation
                if (Input.GetKeyDown(KeyCode.JoystickButton15))
                    NextDialogue();
                break;

            case 9: // Start game
                if (AnyVRInputPressed())
                    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
                break;
        }
    }

    void HandleSurveyInput()
    {
        if (Input.GetKeyDown(KeyCode.JoystickButton15))
        {
            switch (currentSurveyQuestion)
            {
                case 0:
                    ActivateSlider(Slider1, 4);
                    break;
                case 1:
                    ActivateSlider(Slider2, 5);
                    break;
                case 2:
                    ActivateSlider(Slider3, 6);
                    break;
                case 3:
                    CompleteSurvey();
                    break;
            }
            currentSurveyQuestion++;
        }
    }

    void ActivateSlider(GameObject slider, int dialogueIndex)
    {
        Slider1.SetActive(false);
        Slider2.SetActive(false);
        Slider3.SetActive(false);

        slider.SetActive(true);
        dialogueText.text = dialogues[dialogueIndex];
    }

    void CompleteSurvey()
    {
        surveyInProgress = false;
        Slider1.SetActive(false);
        Slider2.SetActive(false);
        Slider3.SetActive(false);
        dialogueStep = 7;
        ShowDialogue();
    }

    void StartSurvey()
    {
        surveyInProgress = true;
        currentSurveyQuestion = 0;
    }

    IEnumerator EnableInteractionMode()
    {
        isInteracting = true;
        dialogueUI.SetActive(false);
        interactUI.SetActive(true);
        yield return new WaitUntil(() => canInteract && Input.GetKeyDown(KeyCode.JoystickButton15));
        interactUI.SetActive(false);
        isInteracting = false;
        NextDialogue();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && dialogueStep >= 2)
        {
            canInteract = true;
            interactUI.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            canInteract = false;
            interactUI.SetActive(false);
        }
    }

    public void ShowDialogue()
    {
        if (dialogueStep < dialogues.Length)
        {
            dialogueText.text = dialogues[dialogueStep];
            dialogueUI.SetActive(true);

            if (dialogueStep == 8) // When explaining UI elements
            {
                questUI.SetActive(true);
                dataUI.SetActive(true);
                GuidingArrow.SetActive(true);
            }
        }
    }

    public void NextDialogue()
    {
        dialogueStep++;
        ShowDialogue();
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
}
