using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "New Character", menuName = "Game/Character")]
public class Character : ScriptableObject, IGetDescription
{
    public enum Gender {
        MALE,
        FEMALE
    }

    // Constants

    // Descriptions for each character
    public string characterName;
    public int age;
    public Gender gender;
    
    [TextArea(3, 10)]
    public string description;
    [TextArea(3, 10), Tooltip("A short description of the character. Used when referred to in events.")]
    public string shortDescription;

    [Header("Personality traits")]

    // Personality traits
    [Tooltip("How social/outgoing they are"), Range(0, 100)]
    public int extroversion;
    [Tooltip("How cooperative/warm they are"), Range(0, 100)]
    public int agreeableness;
    [Tooltip("How organized/responsible they are"), Range(0, 100)]
    public int conscientiousness;
    [Tooltip("How emotionally stable/volatile they are"), Range(0, 100)]
    public int neuroticism;
    [Tooltip("How curious/creative they are"), Range(0, 100)]
    public int openness;

    [Header("Values/Interests")]

    // Values/Interests
    [Tooltip("Focus on goals/career"), Range(0, 100)]
    public int ambition;
    [Tooltip("Respect for rules/customs"), Range(0, 100)]
    public int tradition;
    [Tooltip("Pleasure-seeking vs self-restraint"), Range(0, 100)]
    public int hedonism;
    [Tooltip("Love of learning/thinking"), Range(0, 100)]
    public int intellectualism;

    // Dynamic
    // Use backing fields for properties
    private int _hunger = 0;

    // Stats
    // Stats with proper backing fields

    public int hunger
    {
        get { return _hunger; }
        set { _hunger = Mathf.Clamp(value, 0, 100); }
    }

    // Relations to other characters, including player
    public Dictionary<string, string> relationships = new Dictionary<string, string>();
    
    List<Memory> memories = new List<Memory>();
    public void AddMemory(string memory, string datetime)
    {
        Memory newMemory = new Memory();
        newMemory.dateTime = datetime;
        newMemory.memory = memory;
        memories.Add(newMemory);
    }

    public List<Memory> GetMemories()
    {
        return memories;
    }

    // Location
    public Location currentLocation { get; set; }

    // Components
    private WorldManager worldManager;

    public void Startup(WorldManager worldManager)
    {
        // Set the world manager
        this.worldManager = worldManager;

        // Get a random public location to spawn in
        List<Location> publicLocations = worldManager.locations
            .Where(x => x.isPublic)
            .ToList();

        if (publicLocations.Count > 0)
        {
            currentLocation = publicLocations[Random.Range(0, publicLocations.Count)];
            Debug.Log("Character spawned at " + currentLocation.locationName);
        }
    }

    // Called every hour
    public void UpdateCharacter()
    {
        // Update character stats
        hunger += 1;
    }

    public string GetDescription()
    {
        return characterName + " = " + description + "\n" + "Age: " + age + "\n" + "\n" + "Hunger: " + NumberScaleToText.HundredScaleToText(hunger);
    }
}