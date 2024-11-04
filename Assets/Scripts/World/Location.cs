// Location.cs
using UnityEngine;
using System.Collections.Generic;
using System;

[CreateAssetMenu(fileName = "New Location", menuName = "Game/Location")]
public class Location : ScriptableObject, IGetDescription
{
    [Header("Location Settings")]
    public string locationName;
    [TextArea(3, 10)]
    public string description;
    public bool isPublic = true; // As in, not a private location like a home
    public Sprite backgroundImage;
    public List<Location> connections = new List<Location>();

    /// <summary>
    /// Create a string that describes the location
    /// Used by LLM
    /// </summary>
    /// <returns>string</returns>
    public string GetDescription()
    {
        // Combine the location name, description, and any other relevant information
        return locationName + "\n" + description + "\n" + "Connected locations: " + GetConnectedLocations() + "\n" + "Public: " + isPublic;
    }

    /// <summary>
    /// Create a string that lists all connected locations
    /// Used by LLM
    /// </summary>
    /// <returns>string</returns>
    public string GetConnectedLocations()
    {
        // Create a string that lists all connected locations
        // Used by LLM
        string connectedLocations = "";
        foreach (Location location in connections)
        {
            connectedLocations += location.locationName + ", ";
        }
        return connectedLocations;
    }
}
