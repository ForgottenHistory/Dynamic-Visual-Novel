using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A condition that can be checked for an event to occur.
/// </summary>
[CreateAssetMenu(fileName = "New Event Condition", menuName = "Event Condition/Generic")]
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

    virtual protected bool CheckCharacters(List<Character> characters)
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

/// <summary>
/// A condition that can be checked for an event to occur.
/// </summary>
[CreateAssetMenu(fileName = "New Gender Condition", menuName = "Event Condition/Gender")]
public class GenderEventCondition : EventCondition
{
    public Character.Gender requiredGender;
    protected override bool CheckCharacters(List<Character> characters)
    {
        // Remove any characters that don't match the required gender
        List<Character> validCharacters = new List<Character>();
        foreach (Character character in characters)
        {
            if (character.gender == requiredGender)
            {
                validCharacters.Add(character);
            }
        }
        characters = validCharacters;

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