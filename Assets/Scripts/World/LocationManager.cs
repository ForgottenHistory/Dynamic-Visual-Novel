using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
using System.Linq;

public class LocationManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Image backgroundImage;
    [SerializeField] private ChooseButtonLayout buttonLayout;
    [SerializeField] private TextMeshProUGUI locationNameText;
    [SerializeField] private MasterReferencer masterReferencer;


    [Header("Settings")]
    [SerializeField] private KeyCode toggleConnectionsKey = KeyCode.F3;
    [SerializeField] private bool showConnectionsOnStart = true;

    private bool areConnectionsVisible = false;
    private GameObject currentLayoutObject;
    private WorldManager worldManager;

    private void Start()
    {   
        // Get references
        worldManager = masterReferencer.worldManager;

        areConnectionsVisible = showConnectionsOnStart;
        // Move to the first location in the list
        if (worldManager != null && worldManager.locations.Count > 0)
        {
            MoveToLocation(worldManager.locations[0]);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(toggleConnectionsKey))
        {
            ToggleConnectionButtons();
        }
    }

    public void ToggleConnectionButtons()
    {
        areConnectionsVisible = !areConnectionsVisible;
        
        if (areConnectionsVisible)
        {
            RefreshConnectionButtons();
        }
        else if (currentLayoutObject != null)
        {
            currentLayoutObject.SetActive(false);
        }
    }

    private void RefreshConnectionButtons()
    {
        if (currentLayoutObject != null)
        {
            currentLayoutObject.SetActive(false);
        }

        // Sort connections alphabetically
        List<Location> sortedConnections = worldManager.playerLocation.connections
            .OrderBy(x => x.locationName)
            .ToList();
        
        int connectionCount = sortedConnections.Count;

        currentLayoutObject = buttonLayout.GetLayout(connectionCount);
        if (currentLayoutObject == null) return;

        Button[] buttons = currentLayoutObject.GetComponentsInChildren<Button>(true);
        
        for (int i = 0; i < connectionCount; i++)
        {
            if (i < buttons.Length)
            {
                Location destinationLocation = sortedConnections[i];
                
                TextMeshProUGUI buttonText = buttons[i].GetComponentInChildren<TextMeshProUGUI>();
                if (buttonText != null)
                {
                    buttonText.text = destinationLocation.locationName;
                }

                buttons[i].onClick.RemoveAllListeners();
                buttons[i].onClick.AddListener(() => {
                    MoveToLocation(destinationLocation);
                    areConnectionsVisible = false;
                    currentLayoutObject.SetActive(false);
                });
            }
        }

        currentLayoutObject.SetActive(true);
    }

    public void MoveLocation()
    {
        ToggleConnectionButtons();
        MoveToLocation(worldManager.playerLocation);
    }

    public void MoveToLocation(Location location)
    {
        if (location == null)
        {
            Debug.LogError("Attempted to move to null location!");
            return;
        }

        worldManager.PlayerMoveToLocation(location);

        if (backgroundImage != null)
        {
            backgroundImage.sprite = location.backgroundImage;
        }

        if (locationNameText != null)
        {
            locationNameText.text = location.locationName;
        }
    }

    // Helper method to add a connection between locations at runtime
    public void AddConnection(Location fromLocation, Location toLocation)
    {
        if (fromLocation != null && toLocation != null)
        {
            if (!fromLocation.connections.Contains(toLocation))
            {
                fromLocation.connections.Add(toLocation);
            }
        }
    }
}