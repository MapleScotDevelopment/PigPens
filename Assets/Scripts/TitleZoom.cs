using DG.Tweening;
using TMPro;
using UnityEngine;

public class TitleZoom : MonoBehaviour {

    private float textHighY = 8.9f;
    private float textLowY = 8f;
    private float textScaleNormal = 0.0185f;
    private float textScaleZoom = 0.04f;

    public Canvas titleCanvas;
    public TMP_Text titleText;

    public void ZoomIn()
    {
        titleCanvas.sortingOrder = 2;
        titleText.transform.DOMoveY(textLowY, 0.3f);
        titleText.rectTransform.DOScale(textScaleZoom, 0.3f);
    }

    public void ZoomOut()
    {
        titleCanvas.sortingOrder = 1;
        titleText.transform.DOMoveY(textHighY, 0.3f);
        titleText.rectTransform.DOScale(textScaleNormal, 0.3f);
    }
}
