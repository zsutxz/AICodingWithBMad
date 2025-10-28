using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Automatically sets up EventSystem if missing
/// </summary>
public class SetupEventSystem : MonoBehaviour
{
    private void Start()
    {
        SetupEventSystemIfNeeded();
    }

    [ContextMenu("Setup Event System")]
    public void SetupEventSystemIfNeeded()
    {
        // Check if EventSystem exists
        EventSystem eventSystem = FindObjectOfType<EventSystem>();
        if (eventSystem == null)
        {
            GameObject eventSystemGO = new GameObject("EventSystem");
            eventSystem = eventSystemGO.AddComponent<EventSystem>();
            eventSystemGO.AddComponent<StandaloneInputModule>();

            UnityEngine.Debug.Log("[SetupEventSystem] EventSystem created");
        }
        else
        {
            UnityEngine.Debug.Log("[SetupEventSystem] EventSystem already exists");
        }

        // Add Physics2DRaycaster to main camera if missing
        Camera mainCamera = Camera.main;
        if (mainCamera != null)
        {
            Physics2DRaycaster raycaster = mainCamera.GetComponent<Physics2DRaycaster>();
            if (raycaster == null)
            {
                mainCamera.gameObject.AddComponent<Physics2DRaycaster>();
                UnityEngine.Debug.Log("[SetupEventSystem] Physics2DRaycaster added to main camera");
            }
            else
            {
                UnityEngine.Debug.Log("[SetupEventSystem] Physics2DRaycaster already exists on main camera");
            }
        }
        else
        {
            UnityEngine.Debug.LogWarning("[SetupEventSystem] No main camera found!");
        }
    }
}