using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;

public class MessageManager : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private int maxHistoryLength = 20;
    [SerializeField] private bool debugPrompts = false;
    [SerializeField] private MasterReferencer masterReferencer;
    [Header("Context")]
    [SerializeField]
    private List<DialogueMessage> conversationHistory = new List<DialogueMessage>();
    private UIManager uiManager;
    private WorldManager worldManager;
    private PromptCreator promptCreator;
    private LLMClient llmClient;
    private PlayerData playerData;

    // Store last response data for regeneration
    private string lastCharacterName;
    private string lastPrompt;
    private Action<string> lastCallback;
    private DialogueMessage lastAIMessage;

    private void Awake()
    {
        promptCreator = masterReferencer.promptCreator;
        worldManager = masterReferencer.worldManager;
        uiManager = masterReferencer.uiManager;
        llmClient = masterReferencer.llmClient;
        playerData = masterReferencer.playerData;
    }

    /// <summary>
    /// Updates the current world context in the PromptCreator
    /// </summary>
    public void UpdateWorldContext()
    {
        if (worldManager == null || promptCreator == null) return;

        // Update world state
        if (worldManager.world != null)
        {
            promptCreator.UpdateWorldState("World", worldManager.world.GetDescription());
        }

        // Update location context
        if (worldManager.playerLocation != null)
        {
            promptCreator.SetLocation(worldManager.playerLocation.name, worldManager.playerLocation);
        }

        // Update characters at current location
        var charactersAtLocation = worldManager.GetCharactersAtLocation(worldManager.playerLocation);
        promptCreator.ClearCharacters(); // Clear existing characters first

        // Update player data
        if (playerData != null)
        {
            promptCreator.AddCharacter(playerData.playerName, playerData);
        }

        foreach (var character in charactersAtLocation)
        {
            promptCreator.AddCharacter(character.characterName, character);
        }

        // Update active events if any
        foreach (var evt in worldManager.locationalEvents)
        {
            if (evt.IsActive)
            {
                promptCreator.AddEvent(evt.eventName, evt);
            }
        }
    }

    /// <summary>
    /// Adds a new message to the conversation history
    /// </summary>
    public void AddMessage(string name, string message)
    {
        var newMessage = new DialogueMessage(name, message);
        conversationHistory.Add(newMessage);

        if (conversationHistory.Count > maxHistoryLength)
        {
            conversationHistory.RemoveAt(0);
        }

        if (debugPrompts)
        {
            Debug.Log($"Added message - {newMessage}");
        }
    }

    /// <summary>
    /// Requests an AI response for a specific character
    /// </summary>
    public void GetAIResponse(string characterName, System.Action<string> callback)
    {
        // Store request data for potential regeneration
        lastCharacterName = characterName;
        lastCallback = callback;

        // Update world context before generating prompt
        UpdateWorldContext();

        // Generate the full prompt
        lastPrompt = promptCreator.CreatePrompt(conversationHistory);
        lastPrompt = promptCreator.ReplaceKeywords(lastPrompt, characterName);

        if (debugPrompts)
        {
            Debug.Log($"Generated Prompt:\n{lastPrompt}");
        }

        // Send request to LLM client
        llmClient.SendRequest(lastPrompt, characterName, response =>
        {
            Debug.Log("AI Response: " + response);
            string cleanedResponse = TextCleaner.CleanupMessage(response);

            // Store the AI's message for potential regeneration
            lastAIMessage = new DialogueMessage(characterName, cleanedResponse);
            conversationHistory.Add(lastAIMessage);

            callback?.Invoke(cleanedResponse);
        });
    }

    /// <summary>
    /// Returns a copy of the current conversation history
    /// </summary>
    public List<DialogueMessage> GetHistory()
    {
        return new List<DialogueMessage>(conversationHistory);
    }

    /// <summary>
    /// Clears the conversation history
    /// </summary>
    public void ClearMessages()
    {
        conversationHistory.Clear();
    }

    /// <summary>
    /// Regenerates the last AI response with the same context
    /// </summary>
    public void RegenerateLastResponse()
    {
        if (string.IsNullOrEmpty(lastPrompt) || lastCallback == null)
        {
            Debug.LogWarning("No previous response to regenerate");
            return;
        }

        // Remove the last AI message from history
        RemoveLastAIMessage();

        if (debugPrompts)
        {
            Debug.Log($"Regenerating response for {lastCharacterName} with prompt:\n{lastPrompt}");
        }

        // Send new request to LLM client
        llmClient.SendRequest(lastPrompt, lastCharacterName, response =>
        {
            string cleanedResponse = TextCleaner.CleanupMessage(response);

            // Store the new AI message
            lastAIMessage = new DialogueMessage(lastCharacterName, cleanedResponse);
            conversationHistory.Add(lastAIMessage);
            lastCallback?.Invoke(cleanedResponse);
        });
    }

    /// <summary>
    /// Removes the last AI message from the conversation history
    /// </summary>
    private void RemoveLastAIMessage()
    {
        if (lastAIMessage != null && conversationHistory.Contains(lastAIMessage))
        {
            conversationHistory.Remove(lastAIMessage);
            lastAIMessage = null;
        }
    }

    /// <summary>
    /// Sends a system message based on the provided request type
    /// </summary>
    public void SendSystemMessage(SystemMessages.SystemRequest request, Action<string> callback = null)
    {
        // Update world context before generating prompt
        UpdateWorldContext();

        // Generate the a system prompt
        string generatedRequestPrompt = request.GeneratePrompt(conversationHistory);
        string systemPrompt = lastPrompt + "\n SYSTEM: " + generatedRequestPrompt;
        systemPrompt = promptCreator.ReplaceKeywords(systemPrompt, "SYSTEM");

        // Show on UI if requested
        if (request.ShowOnUI)
        {
            uiManager.UpdateDialogue("SYSTEM", generatedRequestPrompt);
        }

        // Send request to LLM client
        llmClient.SendRequest(systemPrompt, "SYSTEM", response =>
        {
            string cleanedResponse = TextCleaner.CleanupMessage(response);

            if (request.AddToHistory)
            {
                var systemMessage = new DialogueMessage("SYSTEM", cleanedResponse);
                conversationHistory.Add(systemMessage);
            }

            if (request.ShowOnUI && uiManager != null)
            {
                uiManager.UpdateDialogue("SYSTEM", cleanedResponse);
            }

            callback?.Invoke(cleanedResponse);
        });
    }
}