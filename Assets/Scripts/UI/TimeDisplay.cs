using UnityEngine;
using TMPro;

[RequireComponent(typeof(TextMeshProUGUI))]
public class TimeDisplay : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TimeManager timeManager;
    
    [Header("Color Settings")]
    [SerializeField] private Color dayColor = new Color(1f, 0.95f, 0.8f); // Subtle warm orange
    [SerializeField] private Color nightColor = new Color(0.8f, 0.9f, 1f); // Subtle cool blue
    
    [Header("Transition Settings")]
    [SerializeField] [Range(0f, 24f)] private float nightStartHour = 18f; // When night begins (6 PM)
    [SerializeField] [Range(0f, 24f)] private float dayStartHour = 6f; // When day begins (6 AM)

    private TextMeshProUGUI timeText;

    private void Awake()
    {
        timeText = GetComponent<TextMeshProUGUI>();
        
        // If timeManager wasn't assigned in inspector, try to find it
        if (timeManager == null)
        {
            timeManager = FindFirstObjectByType<TimeManager>();
            if (timeManager == null)
            {
                Debug.LogError("TimeDisplay: No TimeManager found in scene!");
                enabled = false;
                return;
            }
        }
    }

    private void Start()
    {
        // Initialize the display immediately with the current time
        UpdateTimeDisplay(timeManager.GetCurrentHour());
        UpdateColor(); // Set initial color
    }

    private void OnEnable()
    {
        // Subscribe to minute changes to update the display
        timeManager.OnMinuteChanged += UpdateTimeDisplay;
    }

    private void OnDisable()
    {
        timeManager.OnMinuteChanged -= UpdateTimeDisplay;
    }

    private void Update()
    {
        UpdateColor();
    }

    private void UpdateTimeDisplay(float currentTime)
    {
        if (timeText != null)
        {
            timeText.text = timeManager.GetFormattedTime();
        }
    }

    private void UpdateColor()
    {
        float currentHour = timeManager.GetCurrentHour();
        
        // Calculate how deep into night or day we are (0-1)
        float t = 0f;
        
        if (currentHour >= nightStartHour || currentHour < dayStartHour)
        {
            // Night time
            if (currentHour >= nightStartHour)
                t = (currentHour - nightStartHour) / (24f - nightStartHour + dayStartHour);
            else
                t = (currentHour + 24f - nightStartHour) / (24f - nightStartHour + dayStartHour);
            
            timeText.color = Color.Lerp(dayColor, nightColor, t);
        }
        else
        {
            // Day time
            t = (currentHour - dayStartHour) / (nightStartHour - dayStartHour);
            timeText.color = Color.Lerp(nightColor, dayColor, t);
        }
    }
}