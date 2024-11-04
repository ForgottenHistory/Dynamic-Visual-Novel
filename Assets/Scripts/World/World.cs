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
        prompt += "Crime Rate: " + NumberScaleToText.TenScaleToText(crimeRate) + ", ";
        prompt += "Pollution Rate: " + NumberScaleToText.TenScaleToText(pollutionRate) + ", ";
        prompt += "Happiness Rate: " + NumberScaleToText.TenScaleToText(happinessRate) + ", ";
        prompt += "Economic Richness: " + NumberScaleToText.TenScaleToText(economicRichness) + ", ";
        prompt += "Education: " + NumberScaleToText.TenScaleToText(education) + ", ";
        prompt += "Health: " + NumberScaleToText.TenScaleToText(health) + ", ";
        prompt += "Beauty: " + NumberScaleToText.TenScaleToText(beauty) + ", ";
        prompt += "Political Corruption: " + NumberScaleToText.TenScaleToText(politicalCorruption) + ", ";
        prompt += "Religiousity: " + NumberScaleToText.TenScaleToText(religiousity) + ", ";
        prompt += "Sexual Activity: " + NumberScaleToText.TenScaleToText(sexual) + ", ";
        prompt += "Xenophobia: " + NumberScaleToText.TenScaleToText(xenophobia) + "\n";

        return prompt;
    }
}