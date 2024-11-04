/// <summary>
/// Data structure for the response received from the LLM API
/// </summary>
[System.Serializable]
public class ResponseData
{
    public Choice[] choices; // Array of possible responses (usually just one)
}
