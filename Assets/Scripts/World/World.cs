// Location.cs
using UnityEngine;
using System.Collections.Generic;
using System;

[CreateAssetMenu(fileName = "New World", menuName = "Game/World")]
public class World : ScriptableObject, IGetDescription
{

    [Header("World Settings")]
    public string worldName = "Generic World";
    [TextArea(3, 10)]
    public string description = "A generic modern world with with a bustling city and surrounding countryside.";

    public List<Location> locations = new List<Location>();
    public List<Character> characters = new List<Character>();
    public List<CharacterEvent> characterEvents = new List<CharacterEvent>();

    [Header("World Stats")]
    public int population = 1000000;
    // Stats from 0 to 10
    public int crimeRate = 5;
    public int pollutionRate = 3;
    public int happinessRate = 7;
    public int economicRichness = 5;
    public int education = 5;
    public int health = 5;
    public int beauty = 5;
    public int politicalCorruption = 5;
    public int religiousity = 5;


    public string GetDescription()
    {
        string prompt = worldName + " = " + description + "\nWorld Stats = ";
        prompt += "Population: " + population + ", ";
        prompt += "Crime Rate: " + crimeRate + ", ";
        prompt += "Pollution Rate: " + pollutionRate + ", ";
        prompt += "Happiness Rate: " + happinessRate + ", ";
        prompt += "Economic Richness: " + economicRichness + ", ";
        prompt += "Education: " + education + ", ";
        prompt += "Health: " + health + ", ";
        prompt += "Beauty: " + beauty + ", ";
        prompt += "Political Corruption: " + politicalCorruption + ", ";
        prompt += "Religiousity: " + religiousity + ", ";

        return prompt;
    }
}