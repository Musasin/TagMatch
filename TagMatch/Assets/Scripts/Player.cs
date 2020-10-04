using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    const float MOVE_VELOCITY = 6.0f;
    const float JUMP_VELOCITY = 15.0f;

    Rigidbody2D rb;
    Animator anim;
    FootJudgement footJudgement;

    enum AnimationState { STAND = 0, RUN = 1, JUMP = 2};
    AnimationState animationState, newAnimationState;

    float velocityX = 0;
    float velocityY = 0;
    bool isRight = true;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<Animator>();
        footJudgement = GetComponentInChildren<FootJudgement>();
        animationState = AnimationState.STAND;
        newAnimationState = AnimationState.STAND;
    }

    // Update is called once per frame
    void Update()
    {
        velocityX = rb.velocity.x;
        velocityY = rb.velocity.y;
        UpdateMove();
        UpdateJump();
        UpdateDirection();
        UpdateState();
        rb.velocity = new Vector2(velocityX, velocityY);
    }

    private void UpdateMove()
    {
        float dx = Input.GetAxisRaw("Horizontal");
        if (dx > 0)
        {
            newAnimationState = AnimationState.RUN;
            velocityX = MOVE_VELOCITY;
            isRight = true;
        }
        else if (dx < 0)
        {
            newAnimationState = AnimationState.RUN;
            velocityX = -MOVE_VELOCITY;
            isRight = false;
        }
        else
        {
            velocityX = 0;
        }
    }
    private void UpdateJump()
    {
        if (Input.GetButtonDown("Jump") && footJudgement.GetIsLanding())
        {
            velocityY = JUMP_VELOCITY;
        }
    }
    private void UpdateDirection()
    {
        if (isRight)
        {
            transform.localScale = new Vector2(1, 1);
        }
        else
        {
            transform.localScale = new Vector2(-1, 1);
        }
    }
    private void UpdateState()
    {
        if (footJudgement.GetIsLanding() == false)
        {
            newAnimationState = AnimationState.JUMP;
        }
        else if (velocityX == 0)
        {
            newAnimationState = AnimationState.STAND;
        }
        else 
        {
            newAnimationState = AnimationState.RUN;
        }

        if (animationState != newAnimationState)
        {
            animationState = newAnimationState;
            anim.SetInteger("state", (int)animationState);
        }
    }
}
