using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Simple click test component - no namespace conflicts
/// </summary>
public class ClickTest : MonoBehaviour, IPointerClickHandler
{
    [Header("Settings")]
    public bool enableLogs = true;

    private void Start()
    {
        // Check essential components
        EventSystem eventSystem = FindObjectOfType<EventSystem>();
        Physics2DRaycaster raycaster = FindObjectOfType<Physics2DRaycaster>();
        Collider2D collider = GetComponent<Collider2D>();

        if (enableLogs)
        {
            UnityEngine.Debug.Log("[ClickTest] EventSystem: " + (eventSystem != null ? "Found" : "MISSING"));
            UnityEngine.Debug.Log("[ClickTest] Physics2DRaycaster: " + (raycaster != null ? "Found" : "MISSING"));
            UnityEngine.Debug.Log("[ClickTest] Collider2D: " + (collider != null ? "Found" : "MISSING"));
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (enableLogs)
        {
            UnityEngine.Debug.Log("[ClickTest] CLICK DETECTED! Button: " + eventData.button);
        }
    }

    private void OnGUI()
    {
        if (!enableLogs) return;

        GUILayout.BeginArea(new Rect(10, 10, 250, 60));
        GUILayout.Label("Click Test", GUI.skin.box);
        GUILayout.Label("Click this object to test");
        GUILayout.EndArea();
    }
}