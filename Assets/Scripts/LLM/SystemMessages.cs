using System.Collections.Generic;

public class SystemMessages
{
    /// <summary>
    /// Base class for all system message requests
    /// </summary>
    public abstract class SystemRequest
    {
        public bool AddToHistory { get; protected set; }
        public bool ShowOnUI { get; protected set; }

        public abstract string GeneratePrompt(List<DialogueMessage> history);
    }

    /// <summary>
    /// Request to create a memory summary from a character's perspective
    /// </summary>
    public class MemorySummaryRequest : SystemRequest
    {
        private readonly string characterName;

        public MemorySummaryRequest(string characterName, bool addToHistory = false, bool showOnUI = false)
        {
            this.characterName = characterName;
            this.AddToHistory = addToHistory;
            this.ShowOnUI = showOnUI;
        }

        public override string GeneratePrompt(List<DialogueMessage> history)
        {
            return "Please create a concise memory summary from {{char}}'s perspective of the recent conversation. " +
                   "Focus on key events, decisions, and emotional moments that would be important for the character to remember. " +
                   "Format the summary in first person as if {{char}} is recording their thoughts.";
        }
    }

    /// <summary>
    /// Request to analyze the emotional states of characters
    /// </summary>
    public class EmotionalAnalysisRequest : SystemRequest
    {
        public EmotionalAnalysisRequest(bool addToHistory = false, bool showOnUI = true)
        {
            this.AddToHistory = addToHistory;
            this.ShowOnUI = showOnUI;
        }

        public override string GeneratePrompt(List<DialogueMessage> history)
        {
            return "Analyze the emotional state of all characters in the recent conversation. " +
                   "Consider their words, actions, and reactions. Provide insights into their current feelings and mental state.";
        }
    }

    /// <summary>
    /// Request to summarize key plot points
    /// </summary>
    public class PlotSummaryRequest : SystemRequest
    {
        private readonly int messageCount;

        public PlotSummaryRequest(int messageCount = -1, bool addToHistory = true, bool showOnUI = false)
        {
            this.messageCount = messageCount;
            this.AddToHistory = addToHistory;
            this.ShowOnUI = showOnUI;
        }

        public override string GeneratePrompt(List<DialogueMessage> history)
        {
            string scope = messageCount > 0 ? $"the last {messageCount} messages" : "the recent conversation";
            return $"Please summarize the key plot points and significant events from {scope}. " +
                   "Focus on story progression, important decisions, and meaningful character interactions.";
        }
    }

    /// <summary>
    /// Request to summarize key plot points
    /// </summary>
    public class RelationshipRequest : SystemRequest
    {
        private readonly int messageCount;

        public RelationshipRequest(bool addToHistory = true, bool showOnUI = false)
        {
            this.AddToHistory = addToHistory;
            this.ShowOnUI = showOnUI;
        }

        public override string GeneratePrompt(List<DialogueMessage> history)
        {
            return "Analyze the emotional state of all characters in the recent conversation." +
                "What would you consider them now to be in terms of their relationship? Reply with only a few words.";
        }
    }

    /// <summary>
    /// Request for a custom system instruction
    /// </summary>
    public class CustomRequest : SystemRequest
    {
        private readonly string instruction;

        public CustomRequest(string instruction, bool addToHistory = false, bool showOnUI = false)
        {
            this.instruction = instruction;
            this.AddToHistory = addToHistory;
            this.ShowOnUI = showOnUI;
        }

        public override string GeneratePrompt(List<DialogueMessage> history)
        {
            return instruction;
        }
    }
}