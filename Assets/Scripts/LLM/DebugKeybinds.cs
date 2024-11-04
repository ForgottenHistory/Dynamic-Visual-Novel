using UnityEngine;

public class DebugKeybinds : MonoBehaviour
{
    [SerializeField] private MasterReferencer masterReferencer;
    public bool DebugMode = false;
    private MessageManager messageManager;
    private WorldManager worldManager;

    void Start()
    {
        // Get references
        messageManager = masterReferencer.messageManager;
        worldManager = masterReferencer.worldManager;
    }

    void Update()
    {
        if (DebugMode)
        {
            DebugInputs();
        }

    }

    void DebugInputs()
    {
        // When using debug mode, use keybinds to trigger system messages
        if (Input.GetKeyDown(KeyCode.F1))
        {
            // Request a memory summary from the player character
            var memoryRequest = new SystemMessages.MemorySummaryRequest("Jeff");
            messageManager.SendSystemMessage(memoryRequest, memory =>
            {
                Debug.Log("Memory Summary: " + memory);
            });
        }
        else if(Input.GetKeyDown(KeyCode.F2))
        {
            // Request an emotional analysis of all characters
            var emotionalRequest = new SystemMessages.EmotionalAnalysisRequest();
            messageManager.SendSystemMessage(emotionalRequest, emotionalAnalysis =>
            {
                Debug.Log("Emotional Analysis: " + emotionalAnalysis);
            });
        }
        else if(Input.GetKeyDown(KeyCode.F3))
        {
            // Request a plot summary of the last 5 messages
            var plotRequest = new SystemMessages.PlotSummaryRequest(5);
            messageManager.SendSystemMessage(plotRequest, plotSummary =>
            {
                Debug.Log("Plot Summary: " + plotSummary);
            });
        }
        else if(Input.GetKeyDown(KeyCode.F4))
        {
            // Request a custom instruction
            var customRequest = new SystemMessages.CustomRequest("Please provide a summary of the recent conversation.");
            messageManager.SendSystemMessage(customRequest, customInstruction =>
            {
                Debug.Log("Custom Instruction: " + customInstruction);
            });
        }
    }
}
