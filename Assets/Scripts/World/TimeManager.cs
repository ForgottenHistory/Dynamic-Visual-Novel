using UnityEngine;
using System;

public class TimeManager : MonoBehaviour, IGetDescription
{
    [Header("Time Settings")]
    [SerializeField] private float timeScale = 1.0f; // How fast time passes
    [SerializeField] private float startingTime = 12.0f; // Start at noon
    
    private float elapsedGameTime = 0.0f; // Total time elapsed since game start
    private float currentTimeOfDay = 0.0f; // Current time of day (0-24)

    public event Action<float> OnHourChanged;
    public event Action<float> OnMinuteChanged;

    private void Start()
    {
        currentTimeOfDay = startingTime;
    }

    private void Update()
    {
        UpdateTime();
    }

    private void UpdateTime()
    {
        // Store previous time values for change detection
        float previousHour = Mathf.Floor(currentTimeOfDay);
        float previousMinute = Mathf.Floor(currentTimeOfDay * 60);

        // Accumulate elapsed time
        elapsedGameTime += Time.deltaTime;
        
        // Calculate new time of day
        currentTimeOfDay = (startingTime + elapsedGameTime * timeScale) % 24.0f;

        // Check for hour change
        float newHour = Mathf.Floor(currentTimeOfDay);
        if (newHour != previousHour)
        {
            OnHourChanged?.Invoke(currentTimeOfDay);
        }

        // Check for minute change
        float newMinute = Mathf.Floor(currentTimeOfDay * 60);
        if (newMinute != previousMinute)
        {
            OnMinuteChanged?.Invoke(currentTimeOfDay);
        }
    }

    /// <summary>
    /// Gets the current hour in 24-hour format (0-23.99)
    /// </summary>
    public float GetCurrentHour()
    {
        return currentTimeOfDay;
    }

    /// <summary>
    /// Gets the current hour in 12-hour format (0-11.99)
    /// </summary>
    public float GetCurrentHour12()
    {
        return currentTimeOfDay % 12.0f;
    }

    /// <summary>
    /// Gets whether it's currently AM or PM
    /// </summary>
    public bool IsAM()
    {
        return currentTimeOfDay < 12.0f;
    }

    /// <summary>
    /// Gets a formatted time string (e.g., "3:30 PM")
    /// </summary>
    public string GetFormattedTime()
    {
        float hour12 = GetCurrentHour12();
        if (hour12 == 0) hour12 = 12; // Convert 0 to 12 for 12-hour format
        
        float minutes = (currentTimeOfDay % 1) * 60;
        string ampm = IsAM() ? "AM" : "PM";
        
        return $"{Mathf.Floor(hour12):0}:{minutes:00} {ampm}";
    }

    /// <summary>
    /// Gets time period of day (Morning, Afternoon, Evening, Night)
    /// </summary>
    public string GetTimePeriod()
    {
        if (currentTimeOfDay >= 5 && currentTimeOfDay < 12)
            return "Morning";
        else if (currentTimeOfDay >= 12 && currentTimeOfDay < 17)
            return "Afternoon";
        else if (currentTimeOfDay >= 17 && currentTimeOfDay < 21)
            return "Evening";
        else
            return "Night";
    }

    /// <summary>
    /// Implements IGetDescription for the prompt system
    /// </summary>
    public string GetDescription()
    {
        return $"It is {GetFormattedTime()} ({GetTimePeriod()})";
    }
}