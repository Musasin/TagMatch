using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    public GameObject starBullet, mustangBullet, electricBarrier, greatElectricFire, jumpAttack;
    public GameObject jumpEffect, invincibleEffect, healEffect;

    const float MOVE_VELOCITY = 6.0f;
    const float JUMP_VELOCITY = 15.0f;
    const float DASH_VELOCITY_X = 45.0f;
    const float DASH_VELOCITY_Y = 0.0f;
    const float BACKFLIP_VELOCITY_X = 10.0f;
    const float DOWNSHOT_VELOCITY_Y = 7.0f;
    const float DAMAGE_VELOCITY_X = 4.0f;
    const float DAMAGE_VELOCITY_Y = 8.0f;
    const float DEAD_VELOCITY_X = 10.0f;
    const float DEAD_VELOCITY_Y = 20.0f;
    const float DASH_TIME = 0.13f;
    const float BACKFLIP_TIME = 0.4f;
    const float DOWNSHOT_TIME = 0.4f;
    const float SHOT_POWER = 400.0f;
    
    const int MP_COST_YUKARI_DASH = 5;
    const int MP_COST_YUKARI_UP_SHOT = 5;
    const int MP_COST_YUKARI_DOWN_SHOT = 10;

    const int MP_COST_MAKI_JUMP = 5;
    const int MP_COST_MAKI_BARRIER = 20;
    const int MP_COST_MAKI_ELECTRIC_FIRE = 40;
    const int MP_COST_MAKI_HEAL = 40;
    
    const float STOP_TIME = 0.25f;
    const float SHOT_IMPOSSIBLE_TIME = 0.4f;
    const float INVINCIBLE_TIME = 0.6f;
    const float MP_RECOVER_TIME = 3.0f;
    const float BARRIER_INVINCIBLE_TIME = 3.0f;
    
    const int YUKARI_BULLET_EXIST_MAX = 4;
    const int MAKI_BULLET_EXIST_MAX = 2;

    Rigidbody2D rb;
    FootJudgement footJudgement;
    GameObject bulletPivot, squatBulletPivot;
    GameObject playerImage, yukariImage, makiImage;
    Animator playerImageAnimator, yukariAnimator, makiAnimator;
    
    BoxCollider2D boxCollider;
    Vector2 boxColliderDefaultSize, boxColliderDefaultOffset;

    enum AnimationState { STAND = 0, RUN = 1, JUMP = 2, SQUAT = 3};
    AnimationState animationState, newAnimationState;
    Vector2 firstPos, lastStandPos1, lastStandPos2;

    float velocityX = 0;
    float velocityY = 0;
    float dashTime = 0;
    float backflipTime = 0;
    float downShotTime = 0;
    float stopTime = 0;
    float shotImpossibleTime = 0;
    float invincibleTime = 0;
    float squatInvincibleTime = 0;
    float mpRecoverTime = 0;
    float checkLastStandPosTime = 0;
    float getOffTime = 0;
    bool isUsedDash = false;
    bool isRight = true;
    bool isDead = false;
    
    int yukariBulletCount = 0;
    int makiBulletCount = 0;
    int barrierBulletCount = 0;
    int electricFireCount = 0;

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
        
        boxCollider = GetComponent<BoxCollider2D>();
        boxColliderDefaultSize = boxCollider.size;
        boxColliderDefaultOffset = boxCollider.offset;

        firstPos = transform.position;
        animationState = AnimationState.STAND;
        newAnimationState = AnimationState.STAND;

        string sceneName = SceneManager.GetActiveScene().name;
        string stageName = sceneName.Split('-')[0];
        if (stageName == "Stage3")
        {
            StaticValues.switchState = StaticValues.SwitchState.YUKARI_ONLY;
        }
        if (stageName == "Stage4")
        {
            StaticValues.switchState = StaticValues.SwitchState.MAKI_ONLY;
        }
        if (stageName == "Stage5") // 元に戻す
        {
            StaticValues.switchState = StaticValues.SwitchState.MAKI;
        }

        Switch(StaticValues.switchState, false);
    }

    // Update is called once per frame
    void Update()
    {
        if (StaticValues.isPause)
        {
            // 慣性で滑らないようにポーズ中は横移動を強制的に停止させる
            rb.velocity = new Vector2(velocityX, rb.velocity.y);
            return;
        }

        if (isDead)
        {
            return;
        }

        if (StaticValues.isTalkPause)
        {
            animationState = AnimationState.STAND;
            yukariAnimator.SetInteger("state", (int)animationState);
            makiAnimator.SetInteger("state", (int)animationState);
            return;
        }

        dashTime -= Time.deltaTime;
        backflipTime -= Time.deltaTime;
        downShotTime -= Time.deltaTime;
        stopTime -= Time.deltaTime;
        shotImpossibleTime -= Time.deltaTime;
        invincibleTime -= Time.deltaTime;
        squatInvincibleTime -= Time.deltaTime;
        mpRecoverTime += Time.deltaTime;
        checkLastStandPosTime -= Time.deltaTime;
        getOffTime -= Time.deltaTime;

        velocityX = rb.velocity.x;
        velocityY = rb.velocity.y;

        if (mpRecoverTime >= MP_RECOVER_TIME)
        {
            mpRecoverTime = 0;
            if (IsYukari())
            {
                StaticValues.AddMP(false, 5);
            } else if (IsMaki())
            {
                StaticValues.AddMP(true, 5);
            }
        }

        if (stopTime <= 0)
        {
            UpdateMove();
            UpdateDirection();
            if (dashTime <= 0 && backflipTime <= 0 && downShotTime <= 0) {
                UpdateJump();
                if (shotImpossibleTime <= 0)
                {
                    UpdateShot();
                    UpdateSwitch();
                }
            }
        }
        UpdateState();
        UpdateColliderSize();
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
        if (downShotTime > 0)
        {
            velocityY = DOWNSHOT_VELOCITY_Y;
            velocityY *= ((DOWNSHOT_TIME - downShotTime) / DOWNSHOT_TIME);
            return;
        }

        // しゃがみ判定
        float dy = Input.GetAxisRaw("Vertical");
        if (dy < 0 && GetIsLanding())
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
        if (KeyConfig.GetJumpKeyDown())
        {
            if (GetIsLanding())
            {
                if (Input.GetAxisRaw("Vertical") < 0) // しゃがみ判定
                {
                    getOffTime = 0.1f;
                } else
                {
                    velocityY = JUMP_VELOCITY;
                    GameObject effect = Instantiate(jumpEffect);
                    effect.transform.position = new Vector2(transform.position.x + (isRight ? -0.3f : 0.3f), transform.position.y);
                    effect.transform.localScale = new Vector2(effect.transform.localScale.x * (isRight ? 1 : -1), effect.transform.localScale.y);
                    AudioManager.Instance.PlaySE("jump");
                }
            }
            else if (!isUsedDash)
            {
                if (IsYukari() && StaticValues.yukariMP >= MP_COST_YUKARI_DASH && StaticValues.GetSkill("y_dash_1"))
                {
                    StaticValues.AddMP(true, -MP_COST_YUKARI_DASH);
                    velocityX = isRight ? DASH_VELOCITY_X : -DASH_VELOCITY_X;
                    velocityY = DASH_VELOCITY_Y;
                    dashTime = DASH_TIME;
                    if (StaticValues.GetSkill("y_dash_2"))
                    {
                        invincibleTime = DASH_TIME;
                    }
                    isUsedDash = true;
                    AudioManager.Instance.PlaySE("dash");
                    AudioManager.Instance.PlayExVoice("yukari_dash", true);
                }
                else if (IsMaki() && StaticValues.makiMP >= MP_COST_MAKI_JUMP && StaticValues.GetSkill("m_jump_1"))
                {
                    // くるりんジャンプ
                    if (StaticValues.GetSkill("m_jump_2"))
                    {
                        DOTween.Sequence()
                            .Append(makiImage.transform.DOLocalRotate(new Vector3(0, 0, 360), BACKFLIP_TIME, RotateMode.FastBeyond360)).Play();
                        InstantiateSpecialBullet(Bullet.BulletType.MAKI_JUMP_ATTACK, jumpAttack, BACKFLIP_TIME);
                        AudioManager.Instance.PlaySE("dash");
                        AudioManager.Instance.PlayExVoice("maki_jump", true);
                    }
                    StaticValues.AddMP(false, -MP_COST_MAKI_JUMP);
                    velocityY = JUMP_VELOCITY;
                    GameObject effect = Instantiate(jumpEffect);
                    effect.transform.position = new Vector2(transform.position.x + (isRight ? -0.3f : 0.3f), transform.position.y);
                    effect.transform.localScale = new Vector2(effect.transform.localScale.x * (isRight ? 1 : -1), effect.transform.localScale.y);
                    isUsedDash = true;
                    AudioManager.Instance.PlaySE("jump");
                }
            }
        }
    }
    private void UpdateShot()
    {
        if (KeyConfig.GetShotKeyDown())
        {
            float dy = Input.GetAxisRaw("Vertical");
            if (dy > 0)
            {
                if (IsYukari() && StaticValues.yukariMP >= MP_COST_YUKARI_UP_SHOT && StaticValues.GetSkill("y_shot_1"))
                {
                    backflipTime = BACKFLIP_TIME;
                    StaticValues.AddMP(true, -MP_COST_YUKARI_UP_SHOT);

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
                    AudioManager.Instance.PlayExVoice("yukari_up_shot", true);
                }
                else if (IsMaki())
                {
                    if (barrierBulletCount > 0)
                    {
                        if (StaticValues.makiMP >= MP_COST_MAKI_ELECTRIC_FIRE && StaticValues.GetSkill("m_shot_2") && electricFireCount <= 0)
                        {
                            StaticValues.AddMP(false, -MP_COST_MAKI_ELECTRIC_FIRE);
                            InstantiateSpecialBullet(Bullet.BulletType.MAKI_ELECTRIC_FIRE, greatElectricFire, BARRIER_INVINCIBLE_TIME);
                            AddBulletCount(Bullet.BulletType.MAKI_ELECTRIC_FIRE, 1);
                            AudioManager.Instance.PlayExVoice("maki_electric", true);
                        }
                    } else
                    {
                        if (StaticValues.makiMP >= MP_COST_MAKI_BARRIER && StaticValues.GetSkill("m_shot_1"))
                        {
                            StaticValues.AddMP(false, -MP_COST_MAKI_BARRIER);
                            invincibleTime = BARRIER_INVINCIBLE_TIME;
                            InstantiateSpecialBullet(Bullet.BulletType.MAKI_BARRIER, electricBarrier, BARRIER_INVINCIBLE_TIME);
                            AddBulletCount(Bullet.BulletType.MAKI_BARRIER, 1);
                            AudioManager.Instance.PlayExVoice("maki_barrier", true);
                        }
                    }
                }
            } 
            else if (dy < 0 && !GetIsLanding()) // 空中下射撃
            {
                if (IsYukari() && StaticValues.yukariMP >= MP_COST_YUKARI_DOWN_SHOT && StaticValues.GetSkill("y_shot_2"))
                {
                    StaticValues.AddMP(true, -MP_COST_YUKARI_DOWN_SHOT);
                    downShotTime = DOWNSHOT_TIME;

                    Sequence sequence = DOTween.Sequence()
                        .Append(yukariImage.transform.DOLocalRotate(new Vector3(0, 0, -45), BACKFLIP_TIME / 8))
                        .AppendCallback(() => { InstantiateBullet(Bullet.BulletType.YUKARI, starBullet, isRight ? -45 : -135); })
                        .AppendInterval(BACKFLIP_TIME / 8)
                        .Append(yukariImage.transform.DOLocalRotate(new Vector3(0, 0, -60), BACKFLIP_TIME / 8))
                        .AppendCallback(() => { InstantiateBullet(Bullet.BulletType.YUKARI, starBullet, isRight ? -80 : -100); })
                        .AppendInterval(BACKFLIP_TIME / 8)
                        .AppendCallback(() => { InstantiateBullet(Bullet.BulletType.YUKARI, starBullet, isRight ? -120 : -60); })
                        .Append(yukariImage.transform.DOLocalRotate(new Vector3(0, 0, -360), BACKFLIP_TIME * 4 / 8, RotateMode.FastBeyond360));
                    sequence.Play();
                    AudioManager.Instance.PlayExVoice("yukari_down_shot", true);
                }
                else if (IsMaki()) 
                {
                    // マキさんは空中下攻撃不可
                }
            }
            else
            {
                bool isSquat = Input.GetAxisRaw("Vertical") < 0 && GetIsLanding();
                if (isSquat)
                {
                    // 低姿勢射撃
                    if (IsYukari() && StaticValues.GetSkill("y_down_1") && yukariBulletCount < YUKARI_BULLET_EXIST_MAX)
                    {
                        InstantiateBullet(Bullet.BulletType.YUKARI, starBullet, isRight ? 0 : 180, isSquat);
                    }
                    // ヒール
                    else if (IsMaki() && StaticValues.GetSkill("m_down_1") && StaticValues.makiMP >= MP_COST_MAKI_HEAL) 
                    {
                        StaticValues.AddMP(false, -MP_COST_MAKI_HEAL);
                        StaticValues.AddHP(true, 50);
                        StaticValues.AddHP(false, 50);
                        GameObject effect = Instantiate(healEffect);
                        effect.transform.position = new Vector2(transform.position.x, transform.position.y + 1.0f);
                        AudioManager.Instance.PlaySE("restore");
                        AudioManager.Instance.PlayExVoice("maki_heal", true);
                    }
                }
                else
                {
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
    }
    private void UpdateSwitch()
    {
        if (KeyConfig.GetSwitchKeyDown())
        {
            switch (StaticValues.switchState)
            {
                case StaticValues.SwitchState.YUKARI:
                    if (StaticValues.makiHP <= 0)
                    {
                        AudioManager.Instance.PlaySE("cancel"); // 仮？
                        return;
                    }
                    Switch(StaticValues.SwitchState.MAKI);
                    AudioManager.Instance.PlayExVoice("yukari_switch", true);
                    break;
                case StaticValues.SwitchState.MAKI:
                    if (StaticValues.yukariHP <= 0)
                    {
                        AudioManager.Instance.PlaySE("cancel"); // 仮？
                        return;
                    }
                    Switch(StaticValues.SwitchState.YUKARI);
                    AudioManager.Instance.PlayExVoice("maki_switch", true);
                    break;
                default:
                    AudioManager.Instance.PlaySE("cancel"); // 仮？
                    break;
            }
        }
    }
    private void Switch(StaticValues.SwitchState nextState, bool isPlaySE = true)
    {
        StaticValues.switchState = nextState;
        mpRecoverTime = 0;
        playerImageAnimator.SetInteger("switchState", (int)StaticValues.switchState);
        shotImpossibleTime = SHOT_IMPOSSIBLE_TIME;
        if (isPlaySE)
        {
            AudioManager.Instance.PlaySE("switch");
        }
    }
    private void InstantiateBullet(Bullet.BulletType bulletType, GameObject bulletObj, float angleZ, bool isSquat = false)
    {
        float addforceX = Mathf.Cos(angleZ * Mathf.Deg2Rad) * SHOT_POWER * (bulletType == Bullet.BulletType.YUKARI ? 1.5f : 1.0f);
        float addforceY = Mathf.Sin(angleZ * Mathf.Deg2Rad) * SHOT_POWER * (bulletType == Bullet.BulletType.YUKARI ? 1.5f : 1.0f);

        GameObject bullet = Instantiate(bulletObj);
        // 精密射撃
        if (isSquat && IsYukari() && StaticValues.GetSkill("y_down_2"))
        {
            bullet.GetComponent<Bullet>().damage = (int)(bullet.GetComponent<Bullet>().damage * 1.5f);
        }
        bullet.transform.position = isSquat ? squatBulletPivot.transform.position : bulletPivot.transform.position;
        bullet.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angleZ));
        bullet.GetComponent<Rigidbody2D>().AddForce(new Vector2(addforceX, addforceY));
        bullet.GetComponent<Bullet>().SetBulletType(bulletType);
        bullet.GetComponent<Bullet>().SetPlayerScript(this);
        AddBulletCount(bulletType, 1);

        if (IsYukari())
        {
            AudioManager.Instance.PlaySE("shot_yukari");
        } else if (IsMaki())
        {
            AudioManager.Instance.PlaySE("shot_maki");
        }
    }
    private void InstantiateSpecialBullet(Bullet.BulletType bulletType, GameObject bulletObj, float deadTime)
    {
        GameObject bullet = Instantiate(bulletObj);
        bullet.transform.position = transform.position;
        bullet.GetComponent<Bullet>().SetBulletType(bulletType);
        bullet.GetComponent<Bullet>().SetPlayerScript(this);
        bullet.GetComponent<Bullet>().SetDeadTime(deadTime);
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
        if (Input.GetAxisRaw("Vertical") < 0 && GetIsLanding() || IsGetOff()) // しゃがみ判定
        {
            UpdateLastStandPos();
            newAnimationState = AnimationState.SQUAT;
        } 
        else if (GetIsLanding() == false || backflipTime > 0)
        {
            newAnimationState = AnimationState.JUMP;
        } 
        else 
        {
            isUsedDash = false;
            UpdateLastStandPos();
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
                if (newAnimationState == AnimationState.SQUAT && StaticValues.GetSkill("m_down_2"))
                {
                    squatInvincibleTime = 0.5f;
                    GameObject effect = Instantiate(invincibleEffect);
                    effect.transform.position = new Vector2(transform.position.x, transform.position.y - 0.3f);

                    // しゃがみ連打でうるさかったり同時再生数の上限が越えかねなかったりで良くないので一旦消す。 音や仕組みがいい感じになったら戻すのも検討する
                    //AudioManager.Instance.PlaySE("avoidance");
                    //AudioManager.Instance.PlayExVoice("maki_hide", true);
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
    private void UpdateColliderSize()
    {
        if (animationState == AnimationState.SQUAT)
        {
            boxCollider.size = new Vector2(boxColliderDefaultSize.x, boxColliderDefaultSize.y / 3);
            boxCollider.offset = new Vector2(boxColliderDefaultOffset.x, boxColliderDefaultOffset.y - boxColliderDefaultSize.y / 3);
        }
        else
        {
            boxCollider.size = boxColliderDefaultSize;
            boxCollider.offset = boxColliderDefaultOffset;
        }
    }
    private void UpdateLastStandPos()
    {
        // 0.2秒おきに立っている位置を記録して、死亡した際に２つ前に記録した位置からスタートする。
        // 一つ前のにしないのは、崖から落ちる直前に記録されるとそのまま落ちてしまいやすいため。
        if (checkLastStandPosTime <= 0)
        {
            lastStandPos2 = lastStandPos1;
            lastStandPos1 = transform.position;
            checkLastStandPosTime = 0.2f;
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
    private bool GetIsLanding()
    {
        // 突き抜け床を登っている最中に着地判定を取られてしまわないよう、上昇中は着地扱いにしない
        return (footJudgement.GetIsLanding() && velocityY <= 0.01f);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 突き抜けを防ぐためアイテムはEnter2Dでも判定を取る
        if (collision.gameObject.tag == "Item")
        {
            Coin coin = collision.gameObject.GetComponent<Coin>();
            if (coin != null)
            {
                coin.GetCoin();
            }
            Heart heart = collision.gameObject.GetComponent<Heart>();
            if (heart != null)
            {
                heart.GetHeart();
            }
        }
    }
    
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (StaticValues.isPause || StaticValues.isTalkPause)
        {
            return;
        }

        if (collision.gameObject.tag == "Item")
        {
            Coin coin = collision.gameObject.GetComponent<Coin>();
            if (coin != null)
            {
                coin.GetCoin();
            }
            Heart heart = collision.gameObject.GetComponent<Heart>();
            if (heart != null)
            {
                heart.GetHeart();
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

            int dp = collision.gameObject.GetComponent<Damage>().damagePoint;
            StaticValues.AddHP(IsYukari(), -dp);
            
            bool isEnemyRight = transform.position.x < collision.gameObject.transform.position.x;
            velocityX = isEnemyRight ? -DAMAGE_VELOCITY_X : DAMAGE_VELOCITY_X;
            rb.velocity = new Vector2(velocityX, DAMAGE_VELOCITY_Y);
            AudioManager.Instance.PlaySE("hit_player");

            // 操作キャラが死亡した時
            if ((IsYukari() && StaticValues.yukariHP <= 0) || (!IsYukari() && StaticValues.makiHP <= 0))
            {
                StaticValues.deadCount++;
                if (IsYukari())
                {
                    AudioManager.Instance.PlayExVoice("yukari_dead", true);
                    if (StaticValues.makiHP <= 0 || StaticValues.switchState == StaticValues.SwitchState.YUKARI_ONLY)
                    {
                        isDead = true;
                        velocityX = isEnemyRight ? -DEAD_VELOCITY_X : DEAD_VELOCITY_X;
                        rb.velocity = new Vector2(velocityX, DEAD_VELOCITY_Y);
                        DOTween.Sequence()
                            .Append(yukariImage.transform.DOLocalRotate(new Vector3(0, 0, 3600), 1.0f, RotateMode.FastBeyond360)).Play();
                        GameObject.Find("ResetScript").GetComponent<ResetScript>().Reset();
                    }
                    else
                    {
                        rb.velocity = Vector2.zero;
                        transform.position = lastStandPos2;
                        Switch(StaticValues.SwitchState.MAKI);
                    }
                }
                else if (IsMaki())
                {
                    AudioManager.Instance.PlayExVoice("maki_dead", true);
                    if (StaticValues.yukariHP <= 0 || StaticValues.switchState == StaticValues.SwitchState.MAKI_ONLY)
                    {
                        isDead = true;
                        velocityX = isEnemyRight ? -DEAD_VELOCITY_X : DEAD_VELOCITY_X;
                        rb.velocity = new Vector2(velocityX, DEAD_VELOCITY_Y);
                        DOTween.Sequence()
                            .Append(makiImage.transform.DOLocalRotate(new Vector3(0, 0, 3600), 1.0f, RotateMode.FastBeyond360)).Play();
                        GameObject.Find("ResetScript").GetComponent<ResetScript>().Reset();
                    }
                    else
                    {
                        rb.velocity = Vector2.zero;
                        transform.position = lastStandPos2;
                        Switch(StaticValues.SwitchState.YUKARI);
                    }
                }
            } else
            {

                if (IsYukari())
                {
                    AudioManager.Instance.PlayExVoice("yukari_damage", true);
                }
                else
                {
                    AudioManager.Instance.PlayExVoice("maki_damage", true);
                }
            }
        }
    }
    
    private bool IsYukari()
    {
        return StaticValues.switchState == StaticValues.SwitchState.YUKARI || StaticValues.switchState == StaticValues.SwitchState.YUKARI_ONLY;
    }
    private bool IsMaki()
    {
        return StaticValues.switchState == StaticValues.SwitchState.MAKI || StaticValues.switchState == StaticValues.SwitchState.MAKI_ONLY;
    }

    public bool IsGetOff()
    {
        return getOffTime > 0;
    }
    public bool IsDead()
    {
        return isDead;
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
        else if (bulletType == Bullet.BulletType.MAKI_BARRIER)
        {
            barrierBulletCount += count;
        }
        else if (bulletType == Bullet.BulletType.MAKI_ELECTRIC_FIRE)
        {
            electricFireCount += count;
        }
    }

    public void Reset()
    {
        transform.position = firstPos;
        rb.velocity = Vector2.zero;
        StaticValues.yukariHP = StaticValues.yukariMaxHP;
        StaticValues.yukariMP = StaticValues.yukariMaxMP;
        StaticValues.makiHP = StaticValues.makiMaxHP;
        StaticValues.makiMP = StaticValues.makiMaxMP;
        yukariBulletCount = 0;
        makiBulletCount = 0;
        barrierBulletCount = 0;
        electricFireCount = 0;
        velocityX = 0;
        velocityY = 0;
        dashTime = 0;
        backflipTime = 0;
        downShotTime = 0;
        stopTime = 0;
        shotImpossibleTime = 0;
        invincibleTime = 0;
        squatInvincibleTime = 0;
        mpRecoverTime = 0;
        checkLastStandPosTime = 0;
        isUsedDash = false;
        isRight = true;
        isDead = false;
    }
}
