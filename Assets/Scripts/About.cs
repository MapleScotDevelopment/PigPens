using System.Collections;
using System.Collections.Generic;
using TMPro;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class About : Dialog
{
    private CanvasGroup cg;
    private float showY = 0f;
    private float hiddenY = -25f;

    // Use this for initialization
    void Awake()
    {
        cg = GetComponent<CanvasGroup>();
        DialogManager.RegisterDialog("About", this);
    }

    public override void Show()
    {
        this.transform.SetAsLastSibling();
        cg.DOKill();
        cg.blocksRaycasts = true;
        cg.interactable = true;
        //cg.DOFade(1f, 0.3f);
        Tweener tweener = transform.DOMoveY(showY, 0.3f);
        tweener.SetEase(Ease.InBounce);
    }

    public override void Hide()
    {
        cg.DOKill();
        cg.interactable = false;
        //cg.DOFade(0f, 0.3f).OnComplete(() => cg.blocksRaycasts = false);
        Tweener tweener = transform.DOMoveY(hiddenY, 0.3f);
        tweener.SetEase(Ease.OutBounce);
    }

    public override void HandleKeyPress(KeyCode k)
    {
        if (cg.interactable)
            OnOk();
    }

    public void OnOk()
    {
        EventSystem.current.SetSelectedGameObject(null);
        DialogManager.Hide("About");
    }

    public void OnRate()
    {
        EventSystem.current.SetSelectedGameObject(null);
        Application.OpenURL(UrlHelper.AppUrl);
    }

    public void OnMapleScotUrl()
    {
        EventSystem.current.SetSelectedGameObject(null);
        Application.OpenURL(UrlHelper.MapleScotUrl);
    }
}
