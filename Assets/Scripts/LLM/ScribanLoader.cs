using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Scriban;
using Scriban.Runtime;
using UnityEngine;
using UnityEngine.Networking;

public class ScribanLoader : MonoBehaviour
{
    [SerializeField] private string templateFile;
    
    private Template compiledTemplate;
    
    void Start() 
    {
        TestTemplate();
    }

    public async void TestTemplate()
    {
        await LoadTemplate("TestFile.txt");
        
        var testChar = new CharacterData
        {
            Name = "Gandalf",
            CurrentMood = "angry",
            Goal = "find the ring",
            Inventory = new List<string> { "sword", "staff" }
        };

        string result = RenderTemplate(testChar);
        Debug.Log(result);
    }


    // Instead of SerializeField
    private async Task LoadTemplate(string templateName)
    {
        string path = Path.Combine(Application.streamingAssetsPath, "Templates", templateName);
        
        using (UnityWebRequest www = UnityWebRequest.Get(path))
        {
            await www.SendWebRequest();
            if (www.result == UnityWebRequest.Result.Success)
            {
                compiledTemplate = Template.Parse(www.downloadHandler.text);
            }
        }
    }

    public string RenderTemplate(CharacterData character)
    {
        var hasItemFunction = DelegateCustomFunction.CreateFunc<string, bool>(itemName => 
            character.Inventory.Contains(itemName)
        );

        // Create the context with all the data
        var templateContext = new ScriptObject
        {
            ["character"] = new ScriptObject
            {
                ["name"] = character.Name,
                ["mood"] = character.CurrentMood,
                ["currentGoal"] = character.Goal,
                ["hasItem"] = hasItemFunction
            }
        };

        // Create a context and set the global script object
        var context = new TemplateContext();
        context.PushGlobal(templateContext);

        // Render the template with the context
        return compiledTemplate.Render(context);
    }
}

// Example supporting class
[System.Serializable]
public class CharacterData
{
    public string Name;
    public string CurrentMood;
    public string Goal;
    public List<string> Inventory;
}