using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BadPiggyController : MonoBehaviour {

    private Animator animator;
    private AudioSource squeal;
    public const float BAD_PIGGY_DELAY = 1.75f;
    private readonly float LEFT_SIDE=-28;
    private readonly float RIGHT_SIDE = 28;

    // Use this for initialization
    void Start () {
        animator = GetComponentInParent<Animator>();
        squeal = GetComponentInParent<AudioSource>();
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
