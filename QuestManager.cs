using UnityEngine;
using TMPro;
using System;

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance;
    public TextMeshProUGUI questText; // UI for quest objectives
    public TextMeshProUGUI dataText; //  New: Data canvas text
    public GameObject caveTrigger;
    public GameObject arrowText;

    private int currentQuestIndex = 0;

    private string[] quests =
    {
        "Find the crayfish and observe\nits behavior.",
        "Find and observe a\nswimming turtle.",
        "Find and interact with the octopus.",
        "Enter the cave to continue\nyour journey."
    };

    private string[] interactionInfo =
    {
        "**Star fish**\nSunflower sea stars (Pycnopodia helianthoides) are voracious predators, consuming nearly five purple sea urchins per week. By controlling urchin populations, they play a pivotal role in maintaining the balance and health of kelp forest ecosystems.\r\n",
        "**Shellfish Creatures and Acidification**\nOysters, Mussels and shellfish creatures alike construct their shells from calcium carbonate. Increased ocean acidification reduces carbonate ion availability, hindering shell formation and compromising their structural integrity.\r\n",
        "**Turtles and Sand Temperatures**\nSea turtle hatchling sex ratios are temperature-dependent; balanced sand temperatures are crucial to ensure a healthy mix of male and female turtles, supporting stable population dynamics\r\n",
        "**Octopuses and Coral dependence**\n​Octopuses are highly intelligent creatures that rely on coral reefs for shelter and survival. However, rising ocean temperatures and acidification are destroying these reefs, leaving octopuses exposed and vulnerable to predators. Additionally, heat stress from ocean warming can impair octopus vision by reducing essential proteins in their eyes, further threatening their survival. These combined factors contribute to declining octopus populations.\r\n"
    };

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        dataText.gameObject.SetActive(true);
        caveTrigger.SetActive(false);
    }

    public void CompleteQuest(int questID, GameObject interactUI)
    {
        Debug.Log($"🔵 Attempting to complete Quest {questID}");
        Debug.Log($"🔹 Current Quest Index BEFORE check: {currentQuestIndex}");

        if (questID == currentQuestIndex)
        {
            interactUI.SetActive(false);

            if (currentQuestIndex < quests.Length - 1)
            {
                questText.text = quests[currentQuestIndex];
                dataText.text = interactionInfo[currentQuestIndex];

                currentQuestIndex = questID + 1;
                FindObjectOfType<ArrowManager>().UpdateArrowTarget(currentQuestIndex);
            }
            else
            {
                caveTrigger.SetActive(true);
                questText.text = "All quests completed! Enter the cave.";
                arrowText.SetActive(false); // Disable arrow after all quests
            }
        }
    }
}
