using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class UIManager : MonoBehaviour
{
    public enum PANELS
    {
        DIALOGUE,
        HISTORY,
        MENU,
        MAP
    }

    [Header("Dialogue UI")]
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private TextMeshProUGUI speakerNameText;

    [Header("Typing Effect")]
    [SerializeField] private TypingEffect dialogueTypingEffect;

    [Header("Input UI")]
    [SerializeField] private TMP_InputField playerInputField;
    [SerializeField] private Button sendButton;
    [SerializeField] private Button historyButton;
    [SerializeField] private Button menuButton;

    [Header("Dialogue History")]
    [SerializeField] private TextMeshProUGUI historyText;
    [SerializeField] private ScrollRect historyScrollRect;

    [Header("Components")]
    [SerializeField] private MasterReferencer masterReferencer;
    [SerializeField] private TextFormatter textFormatter;

    [Header("Panels")]
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private GameObject historyPanel;
    [SerializeField] private GameObject menuPanel;
    [SerializeField] private GameObject mapPanel;

    private MessageManager messageManager;
    private WorldManager worldManager;
    private PlayerData playerData;
    private void Awake()
    {
        messageManager = masterReferencer.messageManager;
        worldManager = masterReferencer.worldManager;
        playerData = masterReferencer.playerData;

        if (textFormatter == null)
            textFormatter = FindFirstObjectByType<TextFormatter>();

        if (dialogueTypingEffect == null)
            dialogueTypingEffect = dialogueText.GetComponent<TypingEffect>();

        // Validate components
        if (textFormatter == null)
        {
            Debug.LogError("TextFormatter not assigned to UIManager!");
        }

        if (dialogueTypingEffect == null)
        {
            Debug.LogError("TypingEffect not assigned to UIManager!");
        }
    }

    private void Start()
    {
        // Hide history panel by default
        if (historyPanel != null)
        {
            historyPanel.SetActive(false);
        }
    }

    /// <summary>
    /// Handles player input submission
    /// </summary>
    public void OnInputSubmitted()
    {
        string input = playerInputField.text;
        if (string.IsNullOrWhiteSpace(input)) return;

        // Add player's message to conversation
        string playerName = playerData.playerName;
        messageManager.AddMessage(playerName, input);
        UpdateDialogue(playerName, input, false);

        // Clear input field
        playerInputField.text = "";
        playerInputField.ActivateInputField();

        string characterName = worldManager.GetCurrentEvent().choosenCharacter.characterName;

        // Request AI response
        messageManager.GetAIResponse(characterName, (response) =>
        {
            Debug.Log("AI Response: " + response);
            UpdateDialogue(characterName, response);
            UpdateHistory();
        });
    }

    /// <summary>
    /// Updates the dialogue UI with formatted text
    /// </summary>
    public void UpdateDialogue(string speaker, string message, bool useTypewriter = true)
    {
        // Don't type the same message twice
        if (dialogueText.text == message)
        {
            return;
        }

        speakerNameText.text = speaker;
        string formattedText = textFormatter.FormatText(message);

        if (useTypewriter)
        {
            dialogueTypingEffect.TypeText(formattedText);
        }
        else
        {
            dialogueText.text = formattedText;
        }
    }


    /// <summary>
    /// Updates the history panel with the full conversation
    /// </summary>
    private void UpdateHistory()
    {
        if (historyText != null)
        {
            var history = messageManager.GetHistory();
            historyText.text = string.Join("\n\n", history);

            // Remove "SYSTEM:" from the history
            historyText.text = historyText.text.Replace("SYSTEM:", "");

            // Scroll to bottom of history
            if (historyScrollRect != null)
            {
                StartCoroutine(ScrollHistoryToBottom());
            }
        }
    }

    private IEnumerator ScrollHistoryToBottom()
    {
        yield return new WaitForEndOfFrame();
        historyScrollRect.normalizedPosition = new Vector2(0, 0);
    }

    /// <summary>
    /// Toggles the history panel visibility
    /// </summary>
    public void ToggleHistory()
    {
        if (historyPanel != null)
        {
            historyPanel.SetActive(!historyPanel.activeSelf);
            if (historyPanel.activeSelf)
            {
                UpdateHistory();
            }
        }
    }

    /// <summary>
    /// Shows the game menu (implement based on your needs)
    /// </summary>
    public void ShowMenu()
    {
        // Implement menu functionality
        Debug.Log("Menu button clicked");
        Application.Quit();
    }

    /// <summary>
    /// Skips the typing effect and shows the full text immediately
    /// </summary>
    public void SkipTyping()
    {
        dialogueTypingEffect.SkipTyping();
    }

    /// <summary>
    /// Clears the dialogue UI
    /// </summary>
    public void ClearDialogue()
    {
        speakerNameText.text = "";
        dialogueTypingEffect.Clear();
        playerInputField.text = "";
    }

    /// <summary>
    /// Shows the specified panel and hides all others
    /// </summary>
    /// <param name="panel"></param>
    public void ShowPanel(PANELS panel)
    {
        dialoguePanel.SetActive(panel == PANELS.DIALOGUE);
        historyPanel.SetActive(panel == PANELS.HISTORY);
        //menuPanel.SetActive(panel == PANELS.MENU);
        //mapPanel.SetActive(panel == PANELS.MAP);
    }

    public void OnRegenerateClicked()
    {
        // Update to show us regenerating the last AI response
        UpdateDialogue(worldManager.GetCurrentEvent().choosenCharacter.characterName, "Regenerating last AI response...");

        // Regenerate the last AI response
        messageManager.RegenerateLastResponse();
    }
}