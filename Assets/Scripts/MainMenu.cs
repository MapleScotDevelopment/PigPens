using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class MainMenu : Dialog
{
    public Canvas TitleCanvas;
    private TitleZoom titleZoom;

    private CanvasGroup cg;
    private float showY = 0f;
    private float hiddenY = -25f;

    // Use this for initialization
    void Awake () {
        cg = GetComponent<CanvasGroup>();
        DialogManager.RegisterDialog("MainMenu", this);
        titleZoom = TitleCanvas.GetComponent<TitleZoom>();
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

        titleZoom.ZoomIn();
    }

    public override void Hide()
    {
        cg.DOKill();
        cg.interactable = false;
        //cg.DOFade(0f, 0.3f).OnComplete(() => cg.blocksRaycasts = false);
        Tweener tweener = transform.DOMoveY(hiddenY, 0.3f);
        tweener.SetEase(Ease.OutBounce);

        titleZoom.ZoomOut();
    }

    public override void HandleKeyPress(KeyCode k)
    {
        if (cg.interactable)
        {
            if (k == KeyCode.Escape)
                OnExit();
            if (k == KeyCode.Tab)
                ; // TODO: move to next inputfield

            if (k == KeyCode.Return)
                OnStart();
        }
    }

    public void OnStart()
    {
        EventSystem.current.SetSelectedGameObject(null);
        GameController.StartGame();
    }

    public void OnExit()
    {
        EventSystem.current.SetSelectedGameObject(null);
        cg.interactable = false;
        GenericDialog dialog = GenericDialog.Instance();
        dialog.Title("Quit Game?")
            .Message("Are you sure you want to quit the game?")
            .OnAccept("Yes", () => Application.Quit())
            .OnDecline("No", () => { DialogManager.Hide("generic"); cg.interactable = true; } );

        DialogManager.Show("generic");
    }

    public void OnAbout()
    {
        EventSystem.current.SetSelectedGameObject(null);
        DialogManager.Show("About");
    }
}
