using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public GameObject starBullet;
    public GameObject jumpEffect;

    const float MOVE_VELOCITY = 6.0f;
    const float JUMP_VELOCITY = 15.0f;
    const float DASH_VELOCITY_X = 35.0f;
    const float DASH_VELOCITY_Y = 0.0f;
    const float DAMAGE_VELOCITY_X = 4.0f;
    const float DAMAGE_VELOCITY_Y = 8.0f;
    const float DASH_TIME = 0.13f;
    const float SHOT_POWER = 400.0f;

    const float STOP_TIME = 0.3f;
    const float INVINCIBLE_TIME = 0.6f;

    Rigidbody2D rb;
    SpriteRenderer sr;
    Animator anim;
    FootJudgement footJudgement;
    GameObject bulletPivot;

    enum AnimationState { STAND = 0, RUN = 1, JUMP = 2};
    AnimationState animationState, newAnimationState;

    float velocityX = 0;
    float velocityY = 0;
    float dashTime = 0;
    float stopTime = 0;
    float invincibleTime = 0;
    bool isUsedDash = false;
    bool isRight = true;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponentInChildren<SpriteRenderer>();
        anim = GetComponentInChildren<Animator>();
        footJudgement = GetComponentInChildren<FootJudgement>();
        bulletPivot = GameObject.Find("BulletPivot");
        animationState = AnimationState.STAND;
        newAnimationState = AnimationState.STAND;
    }

    // Update is called once per frame
    void Update()
    {
        dashTime -= Time.deltaTime;
        stopTime -= Time.deltaTime;
        invincibleTime -= Time.deltaTime;

        velocityX = rb.velocity.x;
        velocityY = rb.velocity.y;

        if (stopTime <= 0)
        {
            UpdateMove();
            UpdateDirection();
            UpdateJump();
            UpdateShot();
        }
        UpdateState();
        UpdateColor();
        rb.velocity = new Vector2(velocityX, velocityY);
    }

    private void UpdateMove()
    {
        if (dashTime > 0)
        {
            velocityX = isRight ? DASH_VELOCITY_X : -DASH_VELOCITY_X;
            velocityX *= (dashTime / DASH_TIME);
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
                GameObject effect = Instantiate(jumpEffect);
                effect.transform.position = new Vector2(transform.position.x + (isRight ? -0.3f : 0.3f), transform.position.y);
                effect.transform.localScale = new Vector2(effect.transform.localScale.x * (isRight ? 1 : -1), effect.transform.localScale.y);
            }
            else if (!isUsedDash)
            {
                velocityX = isRight ? DASH_VELOCITY_X : -DASH_VELOCITY_X;
                velocityY = DASH_VELOCITY_Y;
                dashTime = DASH_TIME;
                isUsedDash = true;
            }
        }
    }
    private void UpdateShot()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            GameObject bullet = Instantiate(starBullet);
            bullet.transform.position = bulletPivot.transform.position;
            bullet.transform.localScale = new Vector2(bullet.transform.localScale.x * (isRight ? 1 : -1), bullet.transform.localScale.y);
            bullet.GetComponent<Rigidbody2D>().AddForce(new Vector2(SHOT_POWER * (isRight ? 1 : -1), 0));
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
        } else
        {
            isUsedDash = false;
            if (Mathf.Abs(velocityX) < 0.2)
            {
                newAnimationState = AnimationState.STAND;
            }
            else 
            {
                newAnimationState = AnimationState.RUN;
            }
        }

        if (animationState != newAnimationState)
        {
            animationState = newAnimationState;
            anim.SetInteger("state", (int)animationState);
        }
    }
    private void UpdateColor()
    {
        float alpha = (invincibleTime > 0) ? 0.5f : 1.0f;
        if (dashTime > 0)
        {
            sr.color = new Color(0.3f, 0.8f, 1.0f, alpha);
        } else
        {
            sr.color = new Color(1.0f, 1.0f, 1.0f, alpha);
        }
    }

    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (invincibleTime > 0)
        {
            return;
        }
        if (collision.gameObject.tag == "Damage")
        {
            stopTime = STOP_TIME;
            invincibleTime = INVINCIBLE_TIME;
            
            bool isEnemyRight = transform.position.x < collision.gameObject.transform.position.x;
            velocityX = isEnemyRight ? -DAMAGE_VELOCITY_X : DAMAGE_VELOCITY_X;
            rb.velocity = new Vector2(velocityX, DAMAGE_VELOCITY_Y);
        }
    }
}
