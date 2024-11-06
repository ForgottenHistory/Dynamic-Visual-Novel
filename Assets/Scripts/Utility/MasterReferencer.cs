using UnityEngine;

public class MasterReferencer : MonoBehaviour
{
    public WorldManager worldManager;
    public MessageManager messageManager;
    public UIManager uiManager;
    public LLMClient llmClient;
    public PromptCreator promptCreator;
    public PlayerData playerData;
    public LocationManager locationManager;
    public TimeManager timeManager;
}
