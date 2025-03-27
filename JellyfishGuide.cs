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

    // Survey Manager Reference
    private MultiSliderManager sliderManager;

    // State Tracking
    private int dialogueStep = 0;
    private bool isInteracting = false;
    private bool canInteract = false;
    private bool surveyInProgress = false;
    private int currentSurveyQuestion = 0;

    private string[] dialogues = {
        "Welcome! Let's learn VR movement. Push the thumbstick forward to move.",
        "Now try turning by rotating the thumbstick left/right.",
        "Anything with ! above them is interactable. Try finding something and interact with it. Go close to it and press the trigger. (Press trigger to continue)",
        "Great job! Before we begin, we want to gauge your understanding of climate change first. (PRESS TRIGGER)",
        "Question 1: How concerned are you about climate change's impact on ocean life? (1 = Not concerned at all, 5 = Extremely concerned)",
        "Question 2: How much do you feel connected to species affected by climate change? (1 = Not connected, 5 = Strongly connected)",
        "Question 3: How likely are you to take action to help prevent ocean damage? (1 = Not likely, 5 = Very likely)",
        "I will appear later to check up on you. (Press trigger)",
        "The red box shows quests, green shows data, and yellow points the way. (Press trigger)",
        "Try to do quests in order. Press the menu button to start.",
    };

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        sliderManager = FindObjectOfType<MultiSliderManager>(); // Find the survey system
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
            case 0:
            case 1:
                if (AnyVRInputPressed()) NextDialogue();
                break;
            case 2:
                if (AnyVRInputPressed()) StartCoroutine(EnableInteractionMode());
                break;
            case 3:
                if (AnyVRInputPressed()) StartSurvey();
                break;
            case 7:
            case 8:
                if (AnyVRInputPressed()) NextDialogue();
                break;
            case 9:
                if (AnyVRInputPressed()) SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
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

    public void CompleteSurvey()
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

            if (dialogueStep == 8)
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
        return Input.GetKeyDown(KeyCode.JoystickButton0) ||  
               Input.GetKeyDown(KeyCode.JoystickButton1) ||  
               Input.GetKeyDown(KeyCode.JoystickButton2) ||  
               Input.GetKeyDown(KeyCode.JoystickButton3) ||  
               Input.GetKeyDown(KeyCode.JoystickButton4) ||  
               Input.GetKeyDown(KeyCode.JoystickButton5) ||  
               Input.GetKeyDown(KeyCode.JoystickButton6) ||  
               Input.GetKeyDown(KeyCode.JoystickButton7) ||  
               Input.GetKeyDown(KeyCode.JoystickButton8) ||  
               Input.GetKeyDown(KeyCode.JoystickButton9) ||  
               Input.GetKeyDown(KeyCode.JoystickButton10) || 
               Input.GetKeyDown(KeyCode.JoystickButton11) || 
               Input.GetKeyDown(KeyCode.JoystickButton12) || 
               Input.GetKeyDown(KeyCode.JoystickButton13) ||
               Input.GetKeyDown(KeyCode.JoystickButton14) ||
               Input.GetKeyDown(KeyCode.JoystickButton15) ||
               Input.GetKeyDown(KeyCode.E); 
    }

    // Called when the Confirm Button is pressed
    public void OnConfirmSurvey()
    {
        sliderManager.ConfirmValues();
        CompleteSurvey();
    }
}
