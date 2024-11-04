using UnityEngine;

/// <summary>
/// Holds data about the player character.
/// </summary>
[CreateAssetMenu(fileName = "New Player", menuName = "Game/Player")]
public class PlayerData : ScriptableObject, IGetDescription
{
    public string playerName = "Jeff";
    [SerializeField] private string description = "An average male in his 20s.";

    public string GetDescription()
    {
        return playerName + " = " + description;
    }
}
