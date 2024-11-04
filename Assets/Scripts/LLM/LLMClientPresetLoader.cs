using UnityEngine;
using System.IO;

/// <summary>
/// Loads a preset LLM client settings file from the StreamingAssets folder
/// </summary>
public class LLMClientPresetLoader : MonoBehaviour
{
    [SerializeField] private string settingsFileName = "llm_settings.json";
    [SerializeField] private LLMClient llmClient;
    
    private void Awake()
    {
        if (llmClient == null)
        {
            llmClient = GetComponent<LLMClient>();
        }
        
        LoadSettings();
    }
    
    /// <summary>
    /// Loads the LLM client settings from a JSON file
    /// </summary>
    private void LoadSettings()
    {
        string filePath = Path.Combine(Application.streamingAssetsPath, settingsFileName);
        
        if (File.Exists(filePath))
        {
            string jsonContent = File.ReadAllText(filePath);
            LLMSettings settings = JsonUtility.FromJson<LLMSettings>(jsonContent);
            llmClient.Initialize(settings);
        }
        else
        {
            Debug.LogError($"Settings file not found at: {filePath}");
        }
    }
}