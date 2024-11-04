using System;

/// <summary>
/// Represents a single message in the chat conversation
/// </summary>
[Serializable]
public class Message
{
    public string role;      // The role of the message sender (e.g., "user", "assistant")
    public string content;   // The actual content of the message
}

///////////////////////////////////////////////////////////////////////////
/// <summary>
/// Represents a single choice in the API response
/// </summary>
[Serializable]
public class Choice
{
    public Message message;  // The generated message from the AI
}