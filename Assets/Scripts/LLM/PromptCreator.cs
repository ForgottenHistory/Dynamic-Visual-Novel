using UnityEngine;
using System.Collections.Generic;
using System.Text;
using System;
using System.Linq;

public class PromptCreator : MonoBehaviour
{
    [System.Serializable]
    public class PromptSection
    {
        public string header;
        public string content;

        public PromptSection(string header, string content)
        {
            this.header = header;
            this.content = content;
        }

        public override string ToString()
        {
            if (string.IsNullOrEmpty(content))
                return "";
                
            return $"**{header}:**\n{content}\n";
        }
    }

    [Header("Prompt Settings")]
    [SerializeField]
    [TextArea(3, 10)]
    private string systemInstructions = "You are a creative writing AI that responds in character. Keep responses concise and natural.";
    
    [SerializeField] private bool includeTimestamp = true;
    [Header("Components")]
    [SerializeField] private MasterReferencer masterReferencer;

    [Header("Debug")]
    [SerializeField] private bool debugMode = false;

    private StringBuilder promptBuilder = new StringBuilder();
    private List<PromptSection> sections = new List<PromptSection>();

    // Cache for description objects
    private Dictionary<string, IGetDescription> worldDescriptions = new Dictionary<string, IGetDescription>();
    private Dictionary<string, IGetDescription> currentLocation = new Dictionary<string, IGetDescription>();
    private Dictionary<string, IGetDescription> currentCharacters = new Dictionary<string, IGetDescription>();
    private Dictionary<string, IGetDescription> currentEvents = new Dictionary<string, IGetDescription>();
    private Dictionary<string, string> worldState = new Dictionary<string, string>();
    private PlayerData playerData;

    void Start()
    {
        playerData = masterReferencer.playerData;
    }

    /// <summary>
    /// Replaces keywords in the text with values from the current context.
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    public string ReplaceKeywords(string text, string speaker)
    {
        // Replace keywords with values from the current context
        text = text.Replace("{{char}}", speaker);
        text = text.Replace("{{user}}", playerData.playerName);

        // Return the updated text
        return text;
    }

    /// <summary>
    /// Clears all characters from the current context
    /// </summary>
    public void ClearCharacters()
    {
        if (debugMode) Debug.Log("Clearing all characters from prompt context");
        currentCharacters.Clear();
    }

    /// <summary>
    /// Updates the current location context
    /// </summary>
    public void SetLocation(string locationName, IGetDescription location)
    {
        currentLocation.Clear();
        if (location != null)
        {
            if (debugMode) Debug.Log($"Setting location: {locationName}");
            currentLocation[locationName] = location;
        }
    }

    /// <summary>
    /// Adds or updates a character in the current scene
    /// </summary>
    public void AddCharacter(string characterName, IGetDescription character)
    {
        if (character != null)
        {
            if (debugMode) Debug.Log($"Adding character to prompt context: {characterName}");
            currentCharacters[characterName] = character;
        }
    }

    /// <summary>
    /// Adds or updates a world description (time, weather, etc.)
    /// </summary>
    public void AddWorldDescription(string key, IGetDescription description)
    {
        if (description != null)
        {
            if (debugMode) Debug.Log($"Adding world description: {key}");
            worldDescriptions[key] = description;
        }
    }

    /// <summary>
    /// Removes a character from the current scene
    /// </summary>
    public void RemoveCharacter(string characterName)
    {
        if (debugMode) Debug.Log($"Removing character from prompt context: {characterName}");
        currentCharacters.Remove(characterName);
    }

    /// <summary>
    /// Adds or updates an active event
    /// </summary>
    public void AddEvent(string eventName, IGetDescription eventObject)
    {
        if (eventObject != null)
        {
            if (debugMode) Debug.Log($"Adding event to prompt context: {eventName}");
            currentEvents[eventName] = eventObject;
        }
    }

    /// <summary>
    /// Removes an event that is no longer active
    /// </summary>
    public void RemoveEvent(string eventName)
    {
        if (debugMode) Debug.Log($"Removing event from prompt context: {eventName}");
        currentEvents.Remove(eventName);
    }

    /// <summary>
    /// Clears all events from the current context
    /// </summary>
    public void ClearEvents()
    {
        if (debugMode) Debug.Log("Clearing all events from prompt context");
        currentEvents.Clear();
    }

    /// <summary>
    /// Updates world state information
    /// </summary>
    public void UpdateWorldState(string key, string value)
    {
        if (debugMode) Debug.Log($"Updating world state: {key} = {value}");
        worldState[key] = value;
    }

    /// <summary>
    /// Creates a formatted prompt combining all current context and conversation history
    /// </summary>
    public string CreatePrompt(List<DialogueMessage> conversationHistory)
    {
        sections.Clear();
        promptBuilder.Clear();

        // Add system instructions
        if (!string.IsNullOrEmpty(systemInstructions))
        {
            sections.Add(new PromptSection("System", systemInstructions));
        }

        // Add timestamp if enabled
        if (includeTimestamp)
        {
            sections.Add(new PromptSection("Current Time", 
                System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")));
        }

        // Add world state if any exists
        if (worldState.Count > 0)
        {
            promptBuilder.Clear();
            foreach (var state in worldState)
            {
                promptBuilder.AppendLine($"{state.Key}: {state.Value}");
            }
            sections.Add(new PromptSection("World State", promptBuilder.ToString().TrimEnd()));
        }

        // Add world descriptions
        if (worldDescriptions.Count > 0)
        {
            promptBuilder.Clear();
            foreach (var desc in worldDescriptions)
            {
                promptBuilder.AppendLine($"{desc.Key}: {desc.Value.GetDescription()}");
            }
            sections.Add(new PromptSection("World Information", promptBuilder.ToString().TrimEnd()));
        }

        // Add location description
        if (currentLocation.Count > 0)
        {
            promptBuilder.Clear();
            foreach (var loc in currentLocation)
            {
                promptBuilder.AppendLine($"{loc.Key}: {loc.Value.GetDescription()}");
            }
            sections.Add(new PromptSection("Current Location", promptBuilder.ToString().TrimEnd()));
        }

        // Add character descriptions
        if (currentCharacters.Count > 0)
        {
            promptBuilder.Clear();
            foreach (var character in currentCharacters)
            {
                promptBuilder.AppendLine($"{character.Key}: {character.Value.GetDescription()}");
            }
            sections.Add(new PromptSection("Characters Present", promptBuilder.ToString().TrimEnd()));
        }

        // Add active events
        if (currentEvents.Count > 0)
        {
            promptBuilder.Clear();
            foreach (var evt in currentEvents)
            {
                promptBuilder.AppendLine($"{evt.Key}: {evt.Value.GetDescription()}");
            }
            sections.Add(new PromptSection("Active Events", promptBuilder.ToString().TrimEnd()));
        }

        // Add conversation history
        if (conversationHistory != null && conversationHistory.Count > 0)
        {
            promptBuilder.Clear();
            foreach (var message in conversationHistory)
            {
                promptBuilder.AppendLine(message.ToString());
            }
            sections.Add(new PromptSection("Conversation", promptBuilder.ToString().TrimEnd()));
        }

        // Combine all sections
        promptBuilder.Clear();
        foreach (var section in sections)
        {
            string sectionText = section.ToString();
            if (!string.IsNullOrEmpty(sectionText))
            {
                promptBuilder.AppendLine(sectionText);
            }
        }

        return promptBuilder.ToString().TrimEnd();
    }
}