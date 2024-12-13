using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class SuperpositionObject : MonoBehaviour
{
    // Represents a potential state of the object in superposition
    [System.Serializable]
    public class SuperpositionState
    {
        public GameObject stateObject; // The GameObject representing this state
        [HideInInspector] public bool isActive = false; // Tracks whether this state is active
    }

    // List of all possible states for this object
    public List<SuperpositionState> states = new List<SuperpositionState>();
    public LayerMask visibilityLayerMask = -1; // Mask defining which layers are visible (default: all layers)
    private Camera mainCamera; 
    private SuperpositionState currentState;

    [Tooltip("Adjusts visibility bounds inward or outward. Positive values make objects stay visible longer.")]
    public float visibilityOffset = -0.082f;

    private void Start()
    {
        mainCamera = Camera.main;

        // If no main camera is found, disable the script and log an error
        if (mainCamera == null)
        {
            Debug.LogError("No main camera found!");
            enabled = false;
            return;
        }

        // Deactivate all states at the start
        foreach (var state in states)
        {
            if (state.stateObject != null)
                state.stateObject.SetActive(false);
        }

        // Initialize the state by picking one at random
        ChangeState();
    }

    private void Update()
    {
        // If the current state is null or no longer visible, attempt to change to a new state
        if (currentState == null || !IsStateVisible(currentState))
        {
            TryChangeState();
        }
    }

    // Checks if a given state is currently visible in the camera's view
    private bool IsStateVisible(SuperpositionState state)
    {
        if (state?.stateObject == null)
            return false;

        // Get all renderers within the state's GameObject
        Renderer[] renderers = state.stateObject.GetComponentsInChildren<Renderer>();
        Plane[] frustumPlanes = GeometryUtility.CalculateFrustumPlanes(mainCamera); // Get the camera's view frustum planes

        foreach (Renderer renderer in renderers)
        {
            if (renderer == null || !renderer.enabled)
                continue;

            // Adjust the renderer's bounds by the visibilityOffset
            Bounds adjustedBounds = renderer.bounds;
            adjustedBounds.Expand(visibilityOffset * adjustedBounds.size.magnitude);

            // Check if the adjusted bounds intersect the view frustum
            if (GeometryUtility.TestPlanesAABB(frustumPlanes, adjustedBounds))
            {
                return true; // The object is visible
            }
        }
        return false; // The object is not visible
    }

    // Attempts to change to a new state if the current one is no longer visible
    private void TryChangeState()
    {
        // Find states that are not visible and are not the current state
        var invisibleStates = states.Where(s => s != currentState && !IsStateVisible(s)).ToList();

        if (invisibleStates.Count == 0)
        {
            Debug.Log("No available states to change to");
            return;
        }

        // Randomly pick one of the invisible states to activate
        ChangeState(invisibleStates[Random.Range(0, invisibleStates.Count)]);
    }

    // Changes the active state to the given state, or selects one at random if none is provided
    private void ChangeState(SuperpositionState newState = null)
    {
        // Deactivate the current state if it exists
        if (currentState != null)
        {
            currentState.stateObject.SetActive(false);
            currentState.isActive = false;
        }

        // If no new state is provided, select one randomly from available states
        if (newState == null)
        {
            var availableStates = states.Where(s => s.stateObject != null).ToList();
            if (availableStates.Count > 0)
                newState = availableStates[Random.Range(0, availableStates.Count)];
        }

        // Activate the new state
        if (newState != null)
        {
            newState.stateObject.SetActive(true);
            newState.isActive = true;
            currentState = newState;
        }
    }
}
