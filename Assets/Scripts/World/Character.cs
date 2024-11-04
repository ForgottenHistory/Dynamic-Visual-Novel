using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "New Character", menuName = "Game/Character")]
public class Character : ScriptableObject, IGetDescription
{
    // Constants

    // Descriptions for each character
    public string characterName;
    public int age;
    [TextArea(3, 10)]
    public string description;
    [TextArea(3, 10), Tooltip("A short description of the character. Used when referred to in events.")]
    public string shortDescription;

    // Dynamic
    // Use backing fields for properties
    private float _health = 100f;
    private float _hunger = 0f;

    // Stats
    // Stats with proper backing fields
    public float health 
    { 
        get { return _health; } 
        set { _health = Mathf.Clamp(value, 0.0f, 100.0f); } 
    }
    
    public float hunger 
    { 
        get { return _hunger; } 
        set { _hunger = Mathf.Clamp(value, 0.0f, 100.0f); } 
    }
    public Location currentLocation { get; set; }

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
        hunger += 1f;
    }

    public string GetDescription()
    {
        return characterName + " = " + description + "\n" + "Age: " + age + "\n" + "Health: " + health + "\n" + "Hunger: " + hunger;
    }
}