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
    public int sexual = 5;
    public int xenophobia = 5;

    public string GetDescription()
    {
        string prompt = worldName + " = " + description + "\nWorld Stats = ";
        prompt += "Population: " + population + ", ";
        prompt += "Crime Rate: " + NumberScaleToText(crimeRate) + ", ";
        prompt += "Pollution Rate: " + NumberScaleToText(pollutionRate) + ", ";
        prompt += "Happiness Rate: " + NumberScaleToText(happinessRate) + ", ";
        prompt += "Economic Richness: " + NumberScaleToText(economicRichness) + ", ";
        prompt += "Education: " + NumberScaleToText(education) + ", ";
        prompt += "Health: " + NumberScaleToText(health) + ", ";
        prompt += "Beauty: " + NumberScaleToText(beauty) + ", ";
        prompt += "Political Corruption: " + NumberScaleToText(politicalCorruption) + ", ";
        prompt += "Religiousity: " + NumberScaleToText(religiousity) + ", ";
        prompt += "Sexual Activity: " + NumberScaleToText(sexual) + ", ";
        prompt += "Xenophobia: " + NumberScaleToText(xenophobia) + "\n";

        return prompt;
    }

    string NumberScaleToText(int number)
    {
        switch (number)
        {
            case 0:
                return "None";
            case 1:
                return "Very Low";
            case 2:
                return "Low";
            case 3:
                return "Moderate";
            case 4:
                return "Average";
            case 5:
                return "Normal";
            case 6:
                return "Above Average";
            case 7:
                return "High";
            case 8:
                return "Very High";
            case 9:
                return "Extreme";
            case 10:
                return "Maximum";
            default:
                return "Unknown";
        }
    }
}