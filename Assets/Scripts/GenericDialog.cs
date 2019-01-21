using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.Events;
using TMPro;

[RequireComponent(typeof(CanvasGroup))]
public class GenericDialog : Dialog
{

    public TMP_Text title;
    public TMP_Text message;
    public TMP_Text accept, decline;
    public Button acceptButton, declineButton;

    private bool hasDecline = false;
    private CanvasGroup cg;

    private float hiddenY = 20f;
    private float showY = 0f;

    void Awake()
    {
        cg = GetComponent<CanvasGroup>();
        DialogManager.RegisterDialog("generic", this);
    }

    public GenericDialog OnAccept(string text, UnityAction action)
    {
        accept.text = text;
        acceptButton.onClick.RemoveAllListeners();
        acceptButton.onClick.AddListener(action);
        return this;
    }

    public GenericDialog NoDecline()
    {
        hasDecline = false;
        return this;
    }

    public GenericDialog OnDecline(string text, UnityAction action)
    {
        decline.text = text;
        declineButton.onClick.RemoveAllListeners();
        declineButton.onClick.AddListener(action);
        hasDecline = true;
        return this;
    }

    public GenericDialog Title(string title)
    {
        this.title.text = title;
        return this;
    }

    public GenericDialog Message(string message)
    {
        this.message.text = message;
        return this;
    }

    public override void Show()
    {
        declineButton.gameObject.SetActive(hasDecline);
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
        {
            if (k == KeyCode.Escape)
            {
                if (hasDecline)
                    declineButton.onClick.Invoke();
                else
                    acceptButton.onClick.Invoke();
            }
            if (k == KeyCode.Return)
                acceptButton.onClick.Invoke();
        }
    }

    private static GenericDialog instance;
    public static GenericDialog Instance()
    {
        if (!instance)
        {
            instance = FindObjectOfType(typeof(GenericDialog)) as GenericDialog;
            if (!instance)
                Debug.Log("There need to be at least one active GenericDialog on the scene");
        }

        return instance;
    }

}