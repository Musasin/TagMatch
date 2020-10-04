using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    const float MOVE_VELOCITY = 6.0f;
    const float JUMP_VELOCITY = 15.0f;
    const float DASH_VELOCITY_X = 25.0f;
    const float DASH_VELOCITY_Y = 0.0f;
    const float DASH_TIME = 0.1f;

    Rigidbody2D rb;
    SpriteRenderer sr;
    Animator anim;
    FootJudgement footJudgement;

    enum AnimationState { STAND = 0, RUN = 1, JUMP = 2};
    AnimationState animationState, newAnimationState;

    float velocityX = 0;
    float velocityY = 0;
    float dashTime = 0;
    bool isRight = true;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponentInChildren<SpriteRenderer>();
        anim = GetComponentInChildren<Animator>();
        footJudgement = GetComponentInChildren<FootJudgement>();
        animationState = AnimationState.STAND;
        newAnimationState = AnimationState.STAND;
    }

    // Update is called once per frame
    void Update()
    {
        dashTime -= Time.deltaTime;
        velocityX = rb.velocity.x;
        velocityY = rb.velocity.y;
        UpdateMove();
        UpdateDirection();
        UpdateJump();
        UpdateState();
        UpdateColor();
        Debug.Log(velocityX);
        rb.velocity = new Vector2(velocityX, velocityY);
    }

    private void UpdateMove()
    {
        if (dashTime > 0)
        {
            velocityX = isRight ? DASH_VELOCITY_X : -DASH_VELOCITY_X;
            velocityY = DASH_VELOCITY_Y;
            return;
        }
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
        } else
        {
            velocityX = 0;
        }
    }
    private void UpdateJump()
    {
        if (Input.GetButtonDown("Jump"))
        {
            if (footJudgement.GetIsLanding())
            {
                velocityY = JUMP_VELOCITY;
            } else
            {
                velocityX = isRight ? DASH_VELOCITY_X : -DASH_VELOCITY_X;
                velocityY = DASH_VELOCITY_Y;
                dashTime = DASH_TIME;
            }
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
        else if (Mathf.Abs(velocityX) < 0.2)
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
    private void UpdateColor()
    {
        if (dashTime > 0)
        {
            sr.color = new Color(0.3f, 0.8f, 1.0f);
        } else
        {
            sr.color = new Color(1.0f, 1.0f, 1.0f);
        }
    }
}
