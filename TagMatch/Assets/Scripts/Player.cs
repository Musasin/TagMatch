using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Player : MonoBehaviour
{
    public GameObject starBullet, mustangBullet;
    public GameObject jumpEffect, invincibleEffect;

    const float MOVE_VELOCITY = 6.0f;
    const float JUMP_VELOCITY = 15.0f;
    const float DASH_VELOCITY_X = 45.0f;
    const float DASH_VELOCITY_Y = 0.0f;
    const float BACKFLIP_VELOCITY_X = 10.0f;
    const float DAMAGE_VELOCITY_X = 4.0f;
    const float DAMAGE_VELOCITY_Y = 8.0f;
    const float DASH_TIME = 0.13f;
    const float BACKFLIP_TIME = 0.4f;
    const float SHOT_POWER = 400.0f;
    
    const float STOP_TIME = 0.3f;
    const float SHOT_IMPOSSIBLE_TIME = 0.4f;
    const float INVINCIBLE_TIME = 0.6f;
    
    const int YUKARI_BULLET_EXIST_MAX = 5;
    const int MAKI_BULLET_EXIST_MAX = 2;

    Rigidbody2D rb;
    FootJudgement footJudgement;
    GameObject bulletPivot, squatBulletPivot;
    GameObject playerImage, yukariImage, makiImage;
    Animator playerImageAnimator, yukariAnimator, makiAnimator;

    enum AnimationState { STAND = 0, RUN = 1, JUMP = 2, SQUAT = 3};
    AnimationState animationState, newAnimationState;
    public enum SwitchState { YUKARI = 0, YUKARI_ONLY = 1, MAKI = 2, MAKI_ONLY = 3};
    SwitchState switchState;

    float velocityX = 0;
    float velocityY = 0;
    float dashTime = 0;
    float backflipTime = 0;
    float stopTime = 0;
    float shotImpossibleTime = 0;
    float invincibleTime = 0;
    float squatInvincibleTime = 0;
    bool isUsedDash = false;
    bool isRight = true;
    
    int yukariBulletCount = 0;
    int makiBulletCount = 0;

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
        shotImpossibleTime -= Time.deltaTime;
        invincibleTime -= Time.deltaTime;
        squatInvincibleTime -= Time.deltaTime;

        velocityX = rb.velocity.x;
        velocityY = rb.velocity.y;

        if (stopTime <= 0)
        {
            UpdateMove();
            UpdateDirection();
            if (dashTime <= 0 && backflipTime <= 0) {
                UpdateJump();
                if (shotImpossibleTime <= 0)
                {
                    UpdateShot();
                    UpdateSwitch();
                }
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
                if (IsYukari())
                {
                    velocityX = isRight ? DASH_VELOCITY_X : -DASH_VELOCITY_X;
                    velocityY = DASH_VELOCITY_Y;
                    dashTime = DASH_TIME;
                    isUsedDash = true;
                }
                else if (IsMaki())
                {
                    velocityY = JUMP_VELOCITY;
                    GameObject effect = Instantiate(jumpEffect);
                    effect.transform.position = new Vector2(transform.position.x + (isRight ? -0.3f : 0.3f), transform.position.y);
                    effect.transform.localScale = new Vector2(effect.transform.localScale.x * (isRight ? 1 : -1), effect.transform.localScale.y);
                    isUsedDash = true;
                }
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
                if (IsMaki()) return; // マキさんの上射撃は未実装

                backflipTime = BACKFLIP_TIME;

                Sequence bulletSequence = DOTween.Sequence()
                    .AppendCallback(() => { InstantiateBullet(Bullet.BulletType.YUKARI, starBullet, isRight ? 30 : 150); })
                    .AppendInterval(0.1f)
                    .AppendCallback(() => { InstantiateBullet(Bullet.BulletType.YUKARI, starBullet, isRight ? 45 : 135); })
                    .AppendInterval(0.1f)
                    .AppendCallback(() => { InstantiateBullet(Bullet.BulletType.YUKARI, starBullet, isRight ? 60 : 120); });

                Sequence sequence = DOTween.Sequence()
                    .Append(yukariImage.transform.DOLocalRotate(new Vector3(0, 0, 360), BACKFLIP_TIME, RotateMode.FastBeyond360))
                    .Join(bulletSequence);
                sequence.Play();
            } 
            else
            {
                bool isSquat = Input.GetAxisRaw("Vertical") < 0 && footJudgement.GetIsLanding();
                if (IsYukari() && yukariBulletCount < YUKARI_BULLET_EXIST_MAX)
                {
                    InstantiateBullet(Bullet.BulletType.YUKARI, starBullet, isRight ? 0 : 180, isSquat);
                } 
                else if (IsMaki() && makiBulletCount < MAKI_BULLET_EXIST_MAX)
                {
                    InstantiateBullet(Bullet.BulletType.MAKI, mustangBullet, isRight ? 0 : 180, isSquat);
                }
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
            shotImpossibleTime = SHOT_IMPOSSIBLE_TIME;
        }
    }
    private void InstantiateBullet(Bullet.BulletType bulletType, GameObject bulletObj, float angleZ, bool isSquat = false)
    {
        float addforceX = Mathf.Cos(angleZ * Mathf.Deg2Rad) * SHOT_POWER;
        float addforceY = Mathf.Sin(angleZ * Mathf.Deg2Rad) * SHOT_POWER;

        GameObject bullet = Instantiate(bulletObj);
        bullet.transform.position = isSquat ? squatBulletPivot.transform.position : bulletPivot.transform.position;
        bullet.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angleZ));
        bullet.GetComponent<Rigidbody2D>().AddForce(new Vector2(addforceX, addforceY));
        bullet.GetComponent<Bullet>().SetBulletType(bulletType);
        bullet.GetComponent<Bullet>().SetPlayerScript(this);
        AddBulletCount(bulletType, 1);
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
            if (IsMaki())
            {
                // 他のステート -> しゃがみ でしゃがみ無敵
                if (newAnimationState == AnimationState.SQUAT)
                {
                    squatInvincibleTime = 1.0f;
                    GameObject effect = Instantiate(invincibleEffect);
                    effect.transform.position = new Vector2(transform.position.x, transform.position.y - 0.3f);
                } 
                // しゃがみ -> 他のステート でしゃがみ無敵解除
                else if (animationState == AnimationState.SQUAT)
                {
                    squatInvincibleTime = 0.0f;
                }
            }

            animationState = newAnimationState;
            yukariAnimator.SetInteger("state", (int)animationState);
            makiAnimator.SetInteger("state", (int)animationState);
        }
    }
    private void UpdateColor()
    {
        float alpha = (invincibleTime > 0 || squatInvincibleTime > 0) ? 0.5f : 1.0f;
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

    
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Item")
        {
            Coin coin = collision.gameObject.GetComponent<Coin>();
            if (coin != null)
            {
                coin.GetCoin();
            }
        }
        if (invincibleTime > 0 || squatInvincibleTime > 0)
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
    
    private bool IsYukari()
    {
        return switchState == SwitchState.YUKARI || switchState == SwitchState.YUKARI_ONLY;
    }
    private bool IsMaki()
    {
        return switchState == SwitchState.MAKI || switchState == SwitchState.MAKI_ONLY;
    }

    public void AddBulletCount(Bullet.BulletType bulletType, int count)
    {
        if (bulletType == Bullet.BulletType.YUKARI)
        {
            yukariBulletCount += count;
        } 
        else if (bulletType == Bullet.BulletType.MAKI)
        {
            makiBulletCount += count;
        }
    }

    public SwitchState GetSwitchState()
    {
        return switchState;
    }
}
