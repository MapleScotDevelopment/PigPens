using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(TextMeshProUGUI))]
public class OpenHyperlink : MonoBehaviour, IPointerClickHandler
{
    TMP_Text pTextMeshPro;
    private Canvas pCanvas;
    private Camera pCamera;

    private void Start()
    {
        pTextMeshPro = GetComponent<TMP_Text>();
        pCanvas = GetComponentInParent<Canvas>();

        // Get a reference to the camera if Canvas Render Mode is not ScreenSpace Overlay.
        if (pCanvas.renderMode == RenderMode.ScreenSpaceOverlay)
            pCamera = null;
        else
            pCamera = pCanvas.worldCamera;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        int linkIndex = TMP_TextUtilities.FindIntersectingLink(pTextMeshPro, Input.mousePosition, pCamera);
        if (linkIndex != -1)
        { // was a link clicked?
            TMP_LinkInfo linkInfo = pTextMeshPro.textInfo.linkInfo[linkIndex];

            // open the link id as a url, which is the metadata we added in the text field
            Application.OpenURL(UrlHelper.GetUrl(linkInfo.GetLinkID()));
        }
    }
}