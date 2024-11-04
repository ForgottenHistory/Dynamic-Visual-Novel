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
    [SerializeField] private KeyCode toggleConnectionsKey = KeyCode.F6;
    [SerializeField] private bool showConnectionsOnStart = true;

    private bool areConnectionsVisible = false;
    private GameObject currentLayoutObject;
    private WorldManager worldManager;
    private MessageManager messageManager;
    private UIManager uiManager;
    private PlayerData playerData;
    public Location pendingLocation { get; set; }
    private bool isTransitioning = false;

    private void Start()
    {
        // Get references
        worldManager = masterReferencer.worldManager;
        messageManager = masterReferencer.messageManager;
        uiManager = masterReferencer.uiManager;
        playerData = masterReferencer.playerData;

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
                buttons[i].onClick.AddListener(() =>
                {
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
    }

    public void MoveToLocation(Location location)
    {
        if (location == null)
        {
            Debug.LogError("Attempted to move to null location!");
            return;
        }

        // If we're already at a location and not transitioning, start the farewell
        if (worldManager.playerLocation != null && !isTransitioning)
        {
            StartTransition(location);
        }
        else if (isTransitioning && pendingLocation == location)
        {
            // Complete the transition
            CompletePendingTransition();
        }
        else
        {
            // First location or direct movement
            ExecuteLocationChange(location);
        }
    }

    private void StartTransition(Location destination)
    {
        isTransitioning = true;
        pendingLocation = destination;

        if (worldManager.GetCharactersAtPlayerLocation().Count == 0)
        {
            CompletePendingTransition();
            return;
        }

        // Instead of sending a farewell message immediately, update the input placeholder
        uiManager.ChangeInputPlaceholder($"Type your farewell message before leaving for {destination.locationName}...");

        // Wait for the player to type their farewell - they'll submit it through the normal input system
        // The UIManager will handle this in its UIInputs method
    }


    public void CancelTransition()
    {
        if (isTransitioning)
        {
            isTransitioning = false;
            pendingLocation = null;
        }
    }

    private void CompletePendingTransition()
    {
        if (pendingLocation != null)
        {
            ExecuteLocationChange(pendingLocation);
            isTransitioning = false;
            pendingLocation = null;
        }
    }

    private void ExecuteLocationChange(Location location)
    {
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