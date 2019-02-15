using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BadPiggyController : MonoBehaviour, IThemedObject {

    private Animator animator;
    private AudioSource squeal;

    private ThemeController themeController;

    public const float BAD_PIGGY_DELAY = 1.75f;
    private const float LEFT_SIDE=-28;
    private const float RIGHT_SIDE = 28;

    private void Awake()
    {
        // register with the theme controller
        themeController = ThemeController.Instance;
        themeController.RegisterThemedObjectHandler(this);
        animator = GetComponentInParent<Animator>();
    }

    // Use this for initialization
    void Start () {
        squeal = GetComponentInParent<AudioSource>();
    }

    public void ThemeChanged()
    {
        var badPiggyAnim = themeController.GetThemedObject<AnimationClip>("BadPiggyAnim");
        var badPiggyAnimController = new AnimatorOverrideController(animator.runtimeAnimatorController);

        var anims = new List<KeyValuePair<AnimationClip, AnimationClip>>();
        foreach (var a in badPiggyAnimController.animationClips)
            anims.Add(new KeyValuePair<AnimationClip, AnimationClip>(a, badPiggyAnim));

        badPiggyAnimController.ApplyOverrides(anims);

        animator.runtimeAnimatorController = badPiggyAnimController;
    }

    public void Trigger()
    {
        animator.SetBool("Idle", false);
        DOTween.Sequence()
            .AppendCallback(squeal.Play)                // start squealing
            .Append(transform.DOMoveX(LEFT_SIDE, BAD_PIGGY_DELAY)) // move to left side over half a second
            .AppendCallback(squeal.Stop)                // stop squealing
            .Append(transform.DOMoveX(RIGHT_SIDE, 0f)) // Reset position to right side
            .AppendInterval(0.25f)
            .AppendCallback(Stop);            // stop pig running
    }

    public void Stop()
    {
        animator.SetBool("Idle", true);
    }
}
