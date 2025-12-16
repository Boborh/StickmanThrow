using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

[RequireComponent(typeof(TMP_Text))]
public class TMPLinkOpener : MonoBehaviour, IPointerClickHandler
{
    private TMP_Text textMeshPro;

    // Optional: you can assign a specific UI camera here if needed
    [SerializeField] private Camera uiCamera;

    void Awake()
    {
        textMeshPro = GetComponent<TMP_Text>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (textMeshPro == null)
            return;

        textMeshPro.ForceMeshUpdate();

        // Determine which camera to use based on the canvas
        var canvas = textMeshPro.canvas;
        Camera cam = uiCamera;

        if (cam == null && canvas != null)
        {
            if (canvas.renderMode == RenderMode.ScreenSpaceOverlay)
            {
                cam = null; // overlay doesn't need a camera
            }
            else
            {
                cam = canvas.worldCamera;
            }
        }

        int linkIndex = TMP_TextUtilities.FindIntersectingLink(textMeshPro, eventData.position, cam);
        if (linkIndex == -1)
            return;

        TMP_LinkInfo linkInfo = textMeshPro.textInfo.linkInfo[linkIndex];
        string url = linkInfo.GetLinkID();

        if (!string.IsNullOrEmpty(url))
        {
            Application.OpenURL(url);
        }
    }
}