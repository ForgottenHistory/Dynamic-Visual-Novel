/// <summary>
/// Data structure for the request payload sent to the LLM API
/// </summary>
[System.Serializable]
public class RequestData
{
    public string model;                 // The AI model to use (e.g., "Midnight-Miqu-70B-v1.5")
    public Message[] messages;           // Array of conversation messages
    public float temperature;            // Controls randomness in response (higher = more random)
    public float top_p;                  // Nucleus sampling parameter
    public int top_k;                    // Limits vocabulary to top K tokens
    public float repetition_penalty;     // Penalizes repetition in generated text
    public int max_tokens;               // Maximum length of generated response
    public float frequency_penalty;      // Penalizes frequent token usage
    public float presence_penalty;       // Penalizes repeated tokens
    public string[] stop;                // Array of sequences where generation should stop
}