using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Text;

///////////////////////////////////////////////////////////////////////////
/// <summary>
/// Handles communication with the Language Learning Model (LLM) API
/// </summary>
public class LLMClient : MonoBehaviour
{
    ///////////////////////////////////////////////////////////////////////////
    private LLMSettings settings;        // Configuration settings for the LLM
    // Preamble for the input prompt
    [SerializeField] 
    private string preamble = "Only write for the character you are controlling. Do not write for other characters."; 

    ///////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Initializes the client with the specified settings
    /// </summary>
    /// <param name="settings">Configuration settings for the LLM</param>
    public void Initialize(LLMSettings settings)
    {
        this.settings = settings;
    }

    ///////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Formats the input prompt with preamble and character name
    /// </summary>
    /// <param name="combinedMessages">The conversation history</param>
    /// <param name="characterName">The name of the responding character</param>
    /// <returns>Formatted prompt string</returns>
    private string MakeInput(string combinedMessages, string characterName)
    {
        return $"\n{preamble}\n***\n{combinedMessages}\n{characterName}:";
    }

    ///////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Public interface to send a request to the LLM API
    /// </summary>
    /// <param name="prompt">The conversation prompt</param>
    /// <param name="respondingCharacter">The character who should respond</param>
    /// <param name="callback">Callback function to handle the response</param>
    public void SendRequest(string prompt, string respondingCharacter, System.Action<string> callback)
    {
        if (settings == null)
        {
            Debug.LogError("LLMClient not initialized with settings!");
            return;
        }
        StartCoroutine(MakeRequest(prompt, respondingCharacter, callback));
    }

    ///////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Coroutine that handles the actual API request
    /// </summary>
    /// <param name="prompt">The conversation prompt</param>
    /// <param name="respondingCharacter">The character who should respond</param>
    /// <param name="callback">Callback function to handle the response</param>
    private IEnumerator MakeRequest(string prompt, string respondingCharacter, System.Action<string> callback)
    {
        // Format the prompt with character name and preamble
        string fullPrompt = MakeInput(prompt, respondingCharacter);

        // Construct the request data object
        var requestData = new RequestData
        {
            model = settings.model,
            messages = new Message[] { new Message { role = "user", content = fullPrompt } },
            temperature = settings.temp,
            top_p = settings.top_p,
            top_k = settings.top_k,
            repetition_penalty = settings.rep_pen,
            max_tokens = settings.genamt,
            frequency_penalty = settings.freq_pen,
            presence_penalty = settings.presence_pen,
            stop = settings.dry_sequence_breakers
        };

        // Convert request data to JSON
        string jsonData = JsonUtility.ToJson(requestData);
        Debug.Log($"Sending request: {jsonData}");

        ///////////////////////////////////////////////////////////////////////////
        // Create and configure the web request
        using (UnityWebRequest request = new UnityWebRequest(settings.apiUrl, "POST"))
        {
            // Set up request body
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();

            // Set headers
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Authorization", $"Bearer {settings.apiToken}");

            // Send the request and wait for response
            yield return request.SendWebRequest();

            // Handle the response
            if (request.result == UnityWebRequest.Result.Success)
            {
                var response = JsonUtility.FromJson<ResponseData>(request.downloadHandler.text);
                if (response.choices != null && response.choices.Length > 0)
                {
                    string aiResponse = response.choices[0].message.content.Trim();
                    callback(aiResponse);
                }
                else
                {
                    callback("No content in the response.");
                }
            }
            else
            {
                Debug.LogError($"Full error: {request.downloadHandler.text}");
                callback($"Error: {request.error}");
            }
        }
    }

    ///////////////////////////////////////////////////////////////////////////
}