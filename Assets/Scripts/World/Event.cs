using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "New Character Event", menuName = "Game/Character Event")]
public class CharacterEvent : ScriptableObject, IGetDescription
{
    public string eventName = null;
    [TextArea(3, 10)]
    public string description = null;
    [TextArea(3, 10)]
    public string startMessage = null;

    private WorldManager worldManager = null;

    public bool IsActive { get; set; } = false;

    public List<EventCondition> conditions = new List<EventCondition>();
    public Character choosenCharacter { get; private set; } = null;

    public void Startup(WorldManager worldManager)
    {
        this.worldManager = worldManager;
    }

    public void StartEvent()
    {
        // Start the event
        choosenCharacter = GetRandomCharacter();
        IsActive = true;
    }

    // Function to check if the event can occur
    public bool CanOccur(Location location, List<Character> characters)
    {
        // Check if any of the conditions are met
        foreach (EventCondition condition in conditions)
        {
            if (condition.CheckCondition(location, characters))
            {
                return true;
            }
        }

        // If none of the conditions are met, return false
        return false;
    }

    public Character GetRandomCharacter()
    {
        // Use HashSet to automatically handle duplicates
        HashSet<Character> uniqueCharacters = new HashSet<Character>();
        foreach (EventCondition condition in conditions)
        {
            List<Character> charactersAtLocation = worldManager.GetCharactersAtLocation(worldManager.playerLocation);
            if (condition.characters.Count == 0)
            {
                uniqueCharacters.UnionWith(charactersAtLocation);
                continue;
            }
            else
            {
                // Only add characters that are at the location and in the list of characters in the condition
                foreach (Character character in charactersAtLocation)
                {
                    if (condition.characters.Contains(character))
                    {
                        uniqueCharacters.Add(character);
                    }
                }
            }
        }

        if (uniqueCharacters.Count == 0)
        {
            return null;
        }
        else if (uniqueCharacters.Count == 1)
        {
            return uniqueCharacters.First();
        }

        // Convert to array for random selection
        Character[] characterArray = uniqueCharacters.ToArray();
        return characterArray[Random.Range(0, characterArray.Length)];
    }

    public void EndEvent()
    {
        IsActive = false;
        choosenCharacter = null;
    }

    public string GetDescription()
    {
        return eventName + " = " + description;
    }
}

/// <summary>
/// A condition that can be checked for an event to occur.
/// </summary>
[CreateAssetMenu(fileName = "New Event Condition", menuName = "Game/Event Condition")]
public class EventCondition : ScriptableObject
{
    public List<Location> locations = new List<Location>();
    public List<Character> characters = new List<Character>();
    public bool needsCharacterOnLocation = true;
    public float chance = 100.0f; // 100% chance by default

    // Function to check if the condition is met
    public bool CheckCondition(Location location, List<Character> characters)
    {
        // Roll using chance
        if (Random.Range(0.0f, 100.0f) > chance)
        {
            return false;
        }

        // Check if the location is in the list of locations
        if (CheckLocation(location) == false)
        {
            return false;
        }

        // Check for characters 
        if (CheckCharacters(characters) == false)
        {
            return false;
        }

        return true;
    }

    private bool CheckLocation(Location location)
    {
        // If the list of locations is empty, return true
        // This means to accept all locations
        if (locations.Count == 0 && location.isPublic)
        {
            return true;
        }

        // Check if the location is in the list of locations
        if (locations.Contains(location))
        {
            return true;
        }

        return false;
    }

    private bool CheckCharacters(List<Character> characters)
    {
        // If the list of characters is empty, return true
        // This means to accept all characters
        if (this.characters.Count == 0)
        {
            // No characters present on this location, and it's needed
            if (needsCharacterOnLocation && characters.Count == 0)
            {
                return false;
            }

            return true;
        }

        // Check if all characters are in the list of characters
        foreach (Character character in characters)
        {
            if (!this.characters.Contains(character))
            {
                return false;
            }
        }

        return true;
    }
}