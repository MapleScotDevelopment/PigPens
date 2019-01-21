using System.Collections;
using System.Collections.Generic;
using TMPro;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class Spinner : Dialog {

    public Image playerImage;
    public TMP_Text playerName;

    public Image arrow;

    public Sprite pinkPig;
    public Sprite yellowPig;
    public Sprite bluePig;
    public Sprite greenPig;
    public Sprite borgPig;

    public AudioClip SpinStart;
    public AudioClip Spin;
    public AudioClip SpinStop;
    public AudioSource audioSource;

    private const float ANIM_MIN_TIME = 1f;
    private const float ANIM_MAX_TIME = 3f;
    private const float WAIT_TIME_AFTER_SPIN = 1f;

    private const float SINGLE_FENCE_START_ANGLE = 2f;
    private const float SINGLE_FENCE_END_ANGLE = 178f;

    private const float BAD_PIGGY_START_ANGLE = 182f;
    private const float BAD_PIGGY_END_ANGLE = 268f;

    private const float DOUBLE_FENCE_START_ANGLE = 272f;
    private const float DOUBLE_FENCE_END_ANGLE = 358f;

    // 5 rotations per second = 1800 degrees/s = 30 degrees per frame at 60fps
    private const float SPEED_ANGLE = 1800f;

    private float rotationAngle = 0f;
    private float finalAngle = 0f;
    private float postSpinTime = 0f;

    private GameState finalState;

    private bool spinning;
    private bool startSpinStop;

    private CanvasGroup cg;
    private float showY = 0f;
    private float hiddenY = -25f;

    private void Awake()
    {
        cg = GetComponent<CanvasGroup>();
        DialogManager.RegisterDialog("Spinner", this);
    }

    private void Start()
    {
        Random.InitState(System.Environment.TickCount);
        BorgPlayer.spinner = this;
    }

    private void SetPlayer()
    {
        // Set the current player
        PlayerColor player = GameController.GetCurrentPlayer();
        playerName.text = GameController.GetPlayerData(player).playerName;
        if (playerImage.transform.localRotation.eulerAngles.y != 0)
            playerImage.transform.Rotate(0, -180, 0);
        switch (player)
        {
            case PlayerColor.BLANK:
                break;
            case PlayerColor.PINK:
                playerImage.sprite = pinkPig;
                break;
            case PlayerColor.YELLOW:
                playerImage.sprite = yellowPig;
                break;
            case PlayerColor.BLUE:
                playerImage.sprite = bluePig;
                break;
            case PlayerColor.GREEN:
                playerImage.sprite = greenPig;
                break;
            case PlayerColor.COMPUTER1:
                playerImage.sprite = borgPig;
                break;
            case PlayerColor.COMPUTER2:
                playerImage.sprite = borgPig;
                playerImage.transform.Rotate(0, 180, 0);
                break;
        }
    }

	// Update is called once per frame
	void FixedUpdate () {
        if (spinning)
        {
            if (rotationAngle > 0f)
            {
                // spin the arrow
                float zAngle = Time.deltaTime * SPEED_ANGLE;
                if (rotationAngle < 360f)
                {
                    zAngle *= 0.9f;
                    if (rotationAngle < 180f)
                    {
                        if (!startSpinStop)
                        {
                            audioSource.Stop();
                            audioSource.PlayOneShot(SpinStop);
                            startSpinStop = true;
                        }
                        zAngle *= 0.6f;
                        if (rotationAngle < 90f)
                            zAngle *= 0.3f;
                    }

                    if (rotationAngle < zAngle)
                        zAngle = rotationAngle;
                }
                arrow.transform.Rotate(0f, 0f, zAngle);
                rotationAngle -= zAngle;
            }
            else
            {
                postSpinTime = WAIT_TIME_AFTER_SPIN;
                spinning = false;
            }
        }

        if (postSpinTime > 0f)
        {
            postSpinTime -= Time.deltaTime;
            if (postSpinTime <= 0f)
                GameController.StateChange(finalState);
        }
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
        SetPlayer();
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
                OnExit();

            if (k == KeyCode.Return)
                OnSpin();
        }
    }

    public void OnSpin()
    {
        if (!spinning)
        {
            switch (Random.Range(1, 5))
            {
                case 1:
                case 2:
                    finalState = GameState.FENCE;
                    break;
                case 3:
                    finalState = GameState.FENCE2;
                    break;
                case 4:
                    finalState = GameState.BAD_PIGGY;
                    break;
            }

            switch (finalState)
            {
                case GameState.FENCE:
                    finalAngle = Random.Range(SINGLE_FENCE_START_ANGLE, SINGLE_FENCE_END_ANGLE);
                    break;
                case GameState.FENCE2:
                    finalAngle = Random.Range(DOUBLE_FENCE_START_ANGLE, DOUBLE_FENCE_END_ANGLE);
                    break;
                case GameState.BAD_PIGGY:
                    finalAngle = Random.Range(BAD_PIGGY_START_ANGLE, BAD_PIGGY_END_ANGLE);
                    break;
            }

            float rotationTime = Random.Range(ANIM_MIN_TIME, ANIM_MAX_TIME);
            rotationAngle = rotationTime * SPEED_ANGLE;

            float angle = rotationAngle % 360f;
            rotationAngle += finalAngle - (arrow.transform.eulerAngles.z + angle);

            Debug.Log("finalAngle: " + finalAngle.ToString() + "state: " + finalState);

            audioSource.PlayOneShot(SpinStart);
            this.StartCoroutine("SpinSound");
            startSpinStop = false;
            spinning = true;
        }
    }

    public void OnExit()
    {
        cg.interactable = false;
        GenericDialog dialog = GenericDialog.Instance();
        dialog.Title("Exit This Game?")
            .Message("Are you sure you want to exit this game?")
            .OnAccept("Yes", () =>
                {
                    GameController.StateChange(GameState.MENU);
                    DialogManager.Hide("generic");
                }
            )
            .OnDecline("No", () => { DialogManager.Hide("generic"); cg.interactable = true; });

        DialogManager.Show("generic");
    }

    public IEnumerator SpinSound()
    {
        yield return new WaitUntil(() => audioSource.isPlaying);
        audioSource.Stop();
        audioSource.clip = Spin;
        audioSource.loop = true;
        audioSource.Play();
        yield return null;
    }
}
