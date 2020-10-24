using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Player : MonoBehaviour
{
    public GameObject starBullet;
    public GameObject jumpEffect;

    const float MOVE_VELOCITY = 6.0f;
    const float JUMP_VELOCITY = 15.0f;
    const float DASH_VELOCITY_X = 35.0f;
    const float DASH_VELOCITY_Y = 0.0f;
    const float BACKFLIP_VELOCITY_X = 10.0f;
    const float DAMAGE_VELOCITY_X = 4.0f;
    const float DAMAGE_VELOCITY_Y = 8.0f;
    const float DASH_TIME = 0.13f;
    const float BACKFLIP_TIME = 0.4f;
    const float SHOT_POWER = 400.0f;

    const float STOP_TIME = 0.3f;
    const float INVINCIBLE_TIME = 0.6f;

    Rigidbody2D rb;
    FootJudgement footJudgement;
    GameObject bulletPivot, squatBulletPivot;
    GameObject playerImage, yukariImage, makiImage;
    Animator playerImageAnimator, yukariAnimator, makiAnimator;

    enum AnimationState { STAND = 0, RUN = 1, JUMP = 2, SQUAT = 3};
    AnimationState animationState, newAnimationState;
    enum SwitchState { YUKARI = 0, YUKARI_ONLY = 1, MAKI = 2, MAKI_ONLY = 3};
    SwitchState switchState;

    float velocityX = 0;
    float velocityY = 0;
    float dashTime = 0;
    float backflipTime = 0;
    float stopTime = 0;
    float invincibleTime = 0;
    bool isUsedDash = false;
    bool isRight = true;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        footJudgement = GetComponentInChildren<FootJudgement>();
        bulletPivot = transform.Find("BulletPivot").gameObject;
        squatBulletPivot = transform.Find("SquatBulletPivot").gameObject;

        playerImage = transform.Find("PlayerImage").gameObject;
        yukariImage = playerImage.transform.Find("Yukari").gameObject;
        makiImage = playerImage.transform.Find("Maki").gameObject;
        playerImageAnimator = playerImage.GetComponent<Animator>();
        yukariAnimator = yukariImage.GetComponent<Animator>();
        makiAnimator = makiImage.GetComponent<Animator>();

        animationState = AnimationState.STAND;
        newAnimationState = AnimationState.STAND;
    }

    // Update is called once per frame
    void Update()
    {
        dashTime -= Time.deltaTime;
        backflipTime -= Time.deltaTime;
        stopTime -= Time.deltaTime;
        invincibleTime -= Time.deltaTime;

        velocityX = rb.velocity.x;
        velocityY = rb.velocity.y;

        if (stopTime <= 0)
        {
            UpdateMove();
            UpdateDirection();
            if (dashTime <= 0 && backflipTime <= 0) {
                UpdateJump();
                UpdateShot();
                UpdateSwitch();
            }
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
        if (backflipTime > 0)
        {
            velocityX = isRight ? -BACKFLIP_VELOCITY_X : BACKFLIP_VELOCITY_X;
            velocityX *= (backflipTime / BACKFLIP_TIME);
            return;
        }

        // しゃがみ判定
        float dy = Input.GetAxisRaw("Vertical");
        if (dy < 0 && footJudgement.GetIsLanding())
        {
            velocityX = 0;
            return;
        }

        // 移動判定
        float dx = Input.GetAxisRaw("Horizontal");
        if (dx > 0)
        {
            velocityX = MOVE_VELOCITY;
            isRight = true;
        }
        else if (dx < 0)
        {
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
            float dy = Input.GetAxisRaw("Vertical");
            if (dy > 0)
            {
                backflipTime = BACKFLIP_TIME;

                Sequence bulletSequence = DOTween.Sequence()
                    .AppendCallback(() => { InstantiateBullet(isRight ? 30 : 150); })
                    .AppendInterval(0.1f)
                    .AppendCallback(() => { InstantiateBullet(isRight ? 45 : 135); })
                    .AppendInterval(0.1f)
                    .AppendCallback(() => { InstantiateBullet(isRight ? 60 : 120); });

                Sequence sequence = DOTween.Sequence()
                    .Append(yukariImage.transform.DOLocalRotate(new Vector3(0, 0, 360), BACKFLIP_TIME, RotateMode.FastBeyond360))
                    .Join(bulletSequence);
                sequence.Play();
            } 
            else if (Input.GetAxisRaw("Vertical") < 0 && footJudgement.GetIsLanding()) // しゃがみ判定
            {
                InstantiateBullet(isRight ? 0 : 180, true);
            }
            else
            {
                InstantiateBullet(isRight ? 0 : 180);
            }

        }
    }
    private void UpdateSwitch()
    {
        if (Input.GetButtonDown("Switch"))
        {
            switch (switchState)
            {
                case SwitchState.YUKARI:
                    switchState = SwitchState.MAKI;
                    playerImageAnimator.SetInteger("switchState", (int)switchState);
                    break;
                case SwitchState.MAKI:
                    switchState = SwitchState.YUKARI;
                    playerImageAnimator.SetInteger("switchState", (int)switchState);
                    break;
                default:
                    break;
            }
        }
    }
    private void InstantiateBullet(float angleZ, bool isSquat = false)
    {
        float addforceX = Mathf.Cos(angleZ * Mathf.Deg2Rad) * SHOT_POWER;
        float addforceY = Mathf.Sin(angleZ * Mathf.Deg2Rad) * SHOT_POWER;

        GameObject bullet = Instantiate(starBullet);
        bullet.transform.position = isSquat ? squatBulletPivot.transform.position : bulletPivot.transform.position;
        bullet.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angleZ));
        bullet.GetComponent<Rigidbody2D>().AddForce(new Vector2(addforceX, addforceY));
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
        if (footJudgement.GetIsLanding() == false || backflipTime > 0)
        {
            newAnimationState = AnimationState.JUMP;
        } 
        else if (Input.GetAxisRaw("Vertical") < 0 && footJudgement.GetIsLanding()) // しゃがみ判定
        {
            newAnimationState = AnimationState.SQUAT;
        } 
        else 
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
            yukariAnimator.SetInteger("state", (int)animationState);
            makiAnimator.SetInteger("state", (int)animationState);
        }
    }
    private void UpdateColor()
    {
        float alpha = (invincibleTime > 0) ? 0.5f : 1.0f;
        if (dashTime > 0)
        {
            yukariImage.GetComponent<SpriteRenderer>().color = new Color(0.3f, 0.8f, 1.0f, alpha);
            makiImage.GetComponent<SpriteRenderer>().color = new Color(0.3f, 0.8f, 1.0f, alpha);
        } 
        else
        {
            yukariImage.GetComponent<SpriteRenderer>().color = new Color(1.0f, 1.0f, 1.0f, alpha);
            makiImage.GetComponent<SpriteRenderer>().color = new Color(1.0f, 1.0f, 1.0f, alpha);
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
