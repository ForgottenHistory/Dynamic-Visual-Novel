using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class WorldManager : MonoBehaviour
{
    [Header("Location Data")]
    public World world;

    [Header("Components")]
    [SerializeField] private MasterReferencer masterReferencer;
    [SerializeField] private TimeManager timeManager;

    [HideInInspector]
    public List<Location> locations;

    [HideInInspector]
    public List<Character> characters;

    [HideInInspector]
    public List<CharacterEvent> locationalEvents;

    private Location _playerLocation;
    public Location playerLocation
    {
        get { return _playerLocation; }
        set
        {
            _playerLocation = value;
            UpdateMessageManagerContext();
        }
    }

    private MessageManager messageManager;
    private PromptCreator promptCreator;
    private UIManager uiManager;
    private CharacterEvent currentEvent;

    public CharacterEvent GetCurrentEvent() => currentEvent;

    void Start()
    {
        // Get references
        messageManager = masterReferencer.messageManager;
        uiManager = masterReferencer.uiManager;
        promptCreator = masterReferencer.promptCreator;

        // Get world data
        locations = world.locations;
        characters = world.characters;
        locationalEvents = world.characterEvents;

        if (timeManager == null)
            timeManager = FindFirstObjectByType<TimeManager>();

        // Initialize the world
        foreach (Character character in characters)
        {
            character.Startup(this);
        }

        foreach (CharacterEvent characterEvent in locationalEvents)
        {
            characterEvent.Startup(this);
        }

        // Subscribe to time events
        timeManager.OnHourChanged += HandleHourChanged;

        // Initial context update
        UpdateMessageManagerContext();
    }

    private void OnDestroy()
    {
        if (timeManager != null)
            timeManager.OnHourChanged -= HandleHourChanged;
    }

    private void HandleHourChanged(float newTime)
    {
        UpdateCharacters();
        UpdateMessageManagerContext();
    }

    /// <summary>
    /// Updates the MessageManager's context with current world state
    /// </summary>
    private void UpdateMessageManagerContext()
    {
        if (messageManager != null)
        {
            messageManager.UpdateWorldContext();

            // Add time manager to prompt context
            promptCreator.UpdateWorldState("Time", timeManager.GetDescription());
        }
    }

    /// <summary>
    /// Handle player movement and trigger events
    /// </summary>
    public void PlayerMoveToLocation(Location location)
    {
        // Move the player to the location
        playerLocation = location;

        // Clear any existing conversation
        messageManager.ClearMessages();

        // Check for possible events
        List<CharacterEvent> possibleEvents = new List<CharacterEvent>();
        foreach (CharacterEvent locationalEvent in locationalEvents)
        {
            if (locationalEvent.CanOccur(playerLocation, GetCharactersAtLocation(playerLocation)))
            {
                possibleEvents.Add(locationalEvent);
            }
        }

        // Start a random event or show default message
        if (possibleEvents.Count > 0)
        {
            StartEvent(possibleEvents[Random.Range(0, possibleEvents.Count)]);
        }
        else
        {
            messageManager.AddMessage("SYSTEM", "Nothing happening at this location, keep exploring!");
            uiManager.UpdateDialogue("SYSTEM", "Nothing happening at this location, keep exploring!");
        }
    }

    /// <summary>
    /// Start a new character event.
    /// </summary>
    /// <param name="newEvent"></param>
    private void StartEvent(CharacterEvent newEvent)
    {
        // Start the event
        currentEvent = newEvent;
        currentEvent.StartEvent();

        // Update message manager context
        messageManager.ClearMessages();

        // Replace keywords in start message
        string startMessageReformatted = masterReferencer.promptCreator.ReplaceKeywords(
            currentEvent.startMessage, currentEvent.choosenCharacter.characterName);

        // Then send the message
        messageManager.AddMessage("SYSTEM", startMessageReformatted);
        uiManager.UpdateDialogue("SYSTEM", startMessageReformatted);
    }

    public List<Character> GetCharactersAtLocation(Location location)
    {
        List<Character> charactersAtLocation = new List<Character>();
        foreach (Character character in characters)
        {
            if (character.currentLocation == location)
            {
                charactersAtLocation.Add(character);
            }
        }
        return charactersAtLocation;
    }

    private void UpdateCharacters()
    {
        foreach (Character character in characters)
        {
            character.UpdateCharacter();
        }

        // Update context after character updates
        UpdateMessageManagerContext();
    }
}