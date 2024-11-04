using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Analytics;

[CreateAssetMenu(fileName = "New Character Event", menuName = "Game/Character Event")]
public class CharacterEvent : ScriptableObject, IGetDescription
{
    public string eventName = null;
    [TextArea(3, 10)]
    public string description = null;
    [TextArea(3, 10)]
    public string startMessage = null;

    // If the event can be left by the player
    // If not the player has to wait until allowed to by the character/s
    public bool canLeave = true;

    private WorldManager worldManager = null;

    public bool IsActive { get; set; } = false;

    public List<EventCondition> conditions = new List<EventCondition>();
    public Character choosenCharacter { get; private set; } = null;


    private string startTimeDate = null;

    public void Startup(WorldManager worldManager)
    {
        this.worldManager = worldManager;
    }

    public void StartEvent(string startTimeDate)
    {
        // Start the event
        choosenCharacter = GetRandomCharacter();
        IsActive = true;
        this.startTimeDate = startTimeDate;
    }

    // Function to check if the event can occur
    public bool CanOccur(Location location, List<Character> characters)
    {
        // Check if any of the conditions are met
        int conditionsMet = 0;
        foreach (EventCondition condition in conditions)
        {
            if (condition.CheckCondition(location, characters))
            {
                conditionsMet++;
            }
        }

        // If all conditions are met, return true
        if (conditionsMet == conditions.Count)
        {
            return true;
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
        return eventName + " = " + description + "\nStart time: " + startTimeDate;
    }
}