using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class Frimomen : BossAIBase
{
    // 青:平均的, 緑:ふんわり落ちてくる, 赤:ダメージ1.5倍, 紫:落下が速い
    public GameObject throwBulletBlue, throwBulletRed, throwBulletGreen, throwBulletPurple, summonBat, wallBullet, afterimageEffect;
    public bool isSecondBattle;
    BoxCollider2D bc;
    GameObject bulletPivot;
    Vector2 l1Pos, l2Pos, l3Pos, l4Pos, r1Pos, r2Pos, r3Pos, r4Pos, c1Pos, c2Pos, c3Pos, jumpOutPos;
    Animator dangerPanel1, dangerPanel2, dangerPanel3;

    const float AUTO_HEAL_INTERVAL = 2.0f;
    const float AFTERIMAGE_INTERVAL = 0.1f;
    const float INSTANTIATE_WALL_INTERVAL = 2.0f;
    float autoHealTime = 0;
    float afterimageTime = 0;
    float instantiateWallTime = 0;
    bool isLeftCannonSummoned, isRightCannonSummoned, isCenterCannonSummoned;
    GameObject playerObject;

    enum ActionState
    {
        START,
        JUMP_OUT,
        JUMP_TO_C1,
        JUMP_TO_C2,
        JUMP_TO_C3,
        JUMP_TO_R1,
        WARP_TO_R1,
        WARP_TO_R2,
        WARP_TO_R3,
        DANGER_1,
        DANGER_2,
        DANGER_3,
        TUCKLE_TO_L1,
        TUCKLE_TO_L2,
        TUCKLE_TO_L3,
        THROW_BULLET_NORMAL,
        THROW_BULLET_SLOW,
        THROW_BULLET_LIGHT,
        THROW_BULLET_HEAVY,
        SUMMON_BUT,
        CHANGE_IS_RIGHT,
        
        MOVE_R1,
        MOVE_R2,
        MOVE_R3,
        MOVE_L1,
        MOVE_L2,
        MOVE_L3,
        FAST_MOVE_R1,
        FAST_MOVE_R2,
        FAST_MOVE_R3,
        FAST_MOVE_L1,
        FAST_MOVE_L2,
        FAST_MOVE_L3,
        BEAM,

        LOOP,
        WAIT,
    }
    ActionState state;
    List<ActionState> actionStateQueue = new List<ActionState>();
    
    // Start is called before the first frame update
    void Start()
    {
        bc = GetComponent<BoxCollider2D>();
        anim = GetComponentInChildren<Animator>();
        bossScript = GetComponent<Boss>();
        state = ActionState.START;
        
        bulletPivot = transform.Find("BulletPivot").gameObject;
        l1Pos = GameObject.Find("L1Pos").transform.position;
        l2Pos = GameObject.Find("L2Pos").transform.position;
        l3Pos = GameObject.Find("L3Pos").transform.position;
        l4Pos = GameObject.Find("L4Pos").transform.position;
        r1Pos = GameObject.Find("R1Pos").transform.position;
        r2Pos = GameObject.Find("R2Pos").transform.position;
        r3Pos = GameObject.Find("R3Pos").transform.position;
        r4Pos = GameObject.Find("R4Pos").transform.position;
        c1Pos = GameObject.Find("C1Pos").transform.position;
        c2Pos = GameObject.Find("C2Pos").transform.position;
        c3Pos = GameObject.Find("C3Pos").transform.position;

        if (isSecondBattle)
        {
            l1Pos = new Vector2(l1Pos.x + 4, l1Pos.y);
            l2Pos = new Vector2(l2Pos.x + 4, l2Pos.y);
            l3Pos = new Vector2(l3Pos.x + 4, l3Pos.y);
            r1Pos = new Vector2(r1Pos.x - 4, r1Pos.y);
            r2Pos = new Vector2(r2Pos.x - 4, r2Pos.y);
            r3Pos = new Vector2(r3Pos.x - 4, r3Pos.y);
        }
        jumpOutPos = GameObject.Find("JumpOutPos").transform.position;
        dangerPanel1 = GameObject.Find("DangerPanel1").GetComponent<Animator>();
        dangerPanel2 = GameObject.Find("DangerPanel2").GetComponent<Animator>();
        dangerPanel3 = GameObject.Find("DangerPanel3").GetComponent<Animator>();
        playerObject = GameObject.Find("Player");
    }
    
    public override void Reset()
    {
        base.Reset();
        anim.SetBool("isTuckle", false);
        anim.SetBool("isThrow", false);
        anim.SetBool("isJump", false);
        actionStateQueue.Clear();
        stateIndex = 0;
        state = ActionState.START;
        isLeftCannonSummoned = false;
        isRightCannonSummoned = false;
        isCenterCannonSummoned = false;
    }

    // Update is called once per frame
    void Update()
    {
        transform.localScale = new Vector2(isRight ? -1 : 1, 1);
        if (!isDead && bossScript.IsDead())
        {
            if (isSecondBattle) AudioManager.Instance.PlayExVoice("frimomen_dead_2");
            else AudioManager.Instance.PlayExVoice("frimomen_dead");
            sequence.Kill();
            isRight = false;
            anim.SetBool("isTuckle", false);
            anim.SetBool("isThrow", false);
            anim.SetBool("isJump", false);
            isDead = true;
            return;
        }

        if (isDead || !bossScript.IsActive())
        {
            return;
        }

        if (isSecondBattle)
        {
            // 向きを自動補正
            transform.localScale = new Vector2(transform.position.x < playerObject.transform.position.x ? -1 : 1, 1);

            // 自動回復
            autoHealTime += Time.deltaTime;
            if (autoHealTime >= AUTO_HEAL_INTERVAL)
            {
                autoHealTime = 0;
                bossScript.HitBullet(-99, null, true);
                AudioManager.Instance.PlaySE("restore");
            }

            // 壁オブジェクトを生成
            instantiateWallTime += Time.deltaTime;
            if (instantiateWallTime >= INSTANTIATE_WALL_INTERVAL)
            {
                instantiateWallTime = 0;
                InstantiateWallBullet();
            }

            // 残像を生成
            afterimageTime += Time.deltaTime;
            if (afterimageTime >= AFTERIMAGE_INTERVAL)
            {
                afterimageTime = 0;
                var afterimage = Instantiate(afterimageEffect, transform.parent);
                afterimage.transform.position = transform.position;
                afterimage.transform.localScale = transform.localScale;
            }

            // HPが減ったら大砲を出す
            if (IsLifeThreeQuarter() && !isRightCannonSummoned)
            {
                isRightCannonSummoned = true;
                GameObject.Find("cannonR").GetComponent<Cannon>().In();
                AudioManager.Instance.PlaySE("summon_canon_R");
            }
            if (IsLifeTwoThirds() && !isLeftCannonSummoned)
            {
                isLeftCannonSummoned = true;
                GameObject.Find("cannonL").GetComponent<Cannon>().In();
                AudioManager.Instance.PlaySE("summon_canon_L");
            }
            if (IsLifeHalf() && !isCenterCannonSummoned)
            {
                isCenterCannonSummoned = true;
                GameObject.Find("cannonT").GetComponent<Cannon>().In();
                AudioManager.Instance.PlaySE("summon_canon_C");
            }
        }

        // アニメーション再生中は次のモードに遷移しない
        if (isPlaying)
        {
            return;
        }

        switch (state)
        {
            case ActionState.START:
                if (isSecondBattle)
                {
                    // 第二形態
                    actionStateQueue.Add(ActionState.MOVE_R1);

                    // 下段か中段でビーム  その後左側へ移動
                    actionStateQueue.Add(ActionState.FAST_MOVE_R2);
                    actionStateQueue.Add(ActionState.FAST_MOVE_R3);
                    actionStateQueue.Add(ActionState.FAST_MOVE_R2);
                    actionStateQueue.Add(ActionState.FAST_MOVE_R1);
                    if (Random.Range(0, 1.0f) < 0.5f)
                    {
                        actionStateQueue.Add(ActionState.FAST_MOVE_R2);
                        actionStateQueue.Add(ActionState.BEAM);
                        actionStateQueue.Add(ActionState.MOVE_L2);
                    }
                    else
                    {
                        actionStateQueue.Add(ActionState.BEAM);
                        actionStateQueue.Add(ActionState.MOVE_L1);
                    }

                    // 右回りに一回転
                    actionStateQueue.Add(ActionState.MOVE_L3);
                    actionStateQueue.Add(ActionState.MOVE_R3);
                    actionStateQueue.Add(ActionState.MOVE_R1);
                    actionStateQueue.Add(ActionState.MOVE_L1);

                    // 下段と中段でビーム 順番はランダム
                    actionStateQueue.Add(ActionState.FAST_MOVE_L2);
                    actionStateQueue.Add(ActionState.FAST_MOVE_L1);
                    actionStateQueue.Add(ActionState.FAST_MOVE_L2);
                    actionStateQueue.Add(ActionState.FAST_MOVE_L3);
                    actionStateQueue.Add(ActionState.FAST_MOVE_L2);
                    if (Random.Range(0, 1.0f) < 0.5f)
                    {
                        actionStateQueue.Add(ActionState.BEAM);
                        actionStateQueue.Add(ActionState.FAST_MOVE_L1);
                        actionStateQueue.Add(ActionState.BEAM);
                    }
                    else
                    {
                        actionStateQueue.Add(ActionState.FAST_MOVE_L1);
                        actionStateQueue.Add(ActionState.BEAM);
                        actionStateQueue.Add(ActionState.FAST_MOVE_L2);
                        actionStateQueue.Add(ActionState.BEAM);
                        actionStateQueue.Add(ActionState.FAST_MOVE_L1);
                    }

                    // そのまま右に行くか、三連続ビームかのランダム
                    if (Random.Range(0, 1.0f) < 0.5f)
                    {
                        actionStateQueue.Add(ActionState.WAIT);
                        actionStateQueue.Add(ActionState.MOVE_R1);
                    }
                    else
                    {
                        actionStateQueue.Add(ActionState.FAST_MOVE_L2);
                        actionStateQueue.Add(ActionState.FAST_MOVE_L3);
                        actionStateQueue.Add(ActionState.FAST_MOVE_L2);
                        actionStateQueue.Add(ActionState.FAST_MOVE_L3);
                        actionStateQueue.Add(ActionState.BEAM);
                        actionStateQueue.Add(ActionState.FAST_MOVE_L2);
                        actionStateQueue.Add(ActionState.BEAM);
                        actionStateQueue.Add(ActionState.FAST_MOVE_L1);
                        actionStateQueue.Add(ActionState.BEAM);
                        actionStateQueue.Add(ActionState.MOVE_R1);
                    }

                    // 一発ビーム撃ってから左回りに一回転
                    actionStateQueue.Add(ActionState.BEAM);
                    actionStateQueue.Add(ActionState.MOVE_R3);
                    actionStateQueue.Add(ActionState.MOVE_L3);
                    actionStateQueue.Add(ActionState.MOVE_L1);
                    actionStateQueue.Add(ActionState.MOVE_R1);

                    // ビーム撃ってから左に行くかそのまま左に行くかのランダム
                    actionStateQueue.Add(ActionState.FAST_MOVE_R2);
                    actionStateQueue.Add(ActionState.FAST_MOVE_R3);
                    actionStateQueue.Add(ActionState.FAST_MOVE_R2);
                    actionStateQueue.Add(ActionState.FAST_MOVE_R1);
                    if (Random.Range(0, 1.0f) < 0.5f) actionStateQueue.Add(ActionState.BEAM);
                    else actionStateQueue.Add(ActionState.WAIT);
                    actionStateQueue.Add(ActionState.MOVE_L1);

                    // 高速で上下した後、ビーム撃ってからまたはそのまま移動 その後またビーム
                    actionStateQueue.Add(ActionState.FAST_MOVE_L3);
                    actionStateQueue.Add(ActionState.FAST_MOVE_L1);
                    actionStateQueue.Add(ActionState.FAST_MOVE_L3);
                    actionStateQueue.Add(ActionState.FAST_MOVE_L1);
                    if (Random.Range(0, 1.0f) < 0.5f) actionStateQueue.Add(ActionState.BEAM);
                    else actionStateQueue.Add(ActionState.WAIT);
                    actionStateQueue.Add(ActionState.MOVE_R1);
                    actionStateQueue.Add(ActionState.BEAM);

                    // ここで初めて中段で移動
                    actionStateQueue.Add(ActionState.MOVE_R2);
                    actionStateQueue.Add(ActionState.MOVE_L2);

                    // 下段でビーム二連射
                    actionStateQueue.Add(ActionState.FAST_MOVE_L1);
                    actionStateQueue.Add(ActionState.BEAM);
                    actionStateQueue.Add(ActionState.BEAM);


                    // 上段か下段で移動、その後すぐ戻る
                    if (Random.Range(0, 1.0f) < 0.5f) { 
                        actionStateQueue.Add(ActionState.MOVE_L3);
                        actionStateQueue.Add(ActionState.MOVE_R3);
                        actionStateQueue.Add(ActionState.MOVE_L3);
                    } else
                    {
                        actionStateQueue.Add(ActionState.MOVE_R1);
                        actionStateQueue.Add(ActionState.MOVE_L1);
                    }

                    // 中段でビーム ランダムで二回撃つ その後右に移動
                    actionStateQueue.Add(ActionState.MOVE_L2);
                    actionStateQueue.Add(ActionState.BEAM);
                    if (Random.Range(0, 1.0f) < 0.5f) actionStateQueue.Add(ActionState.BEAM);
                    actionStateQueue.Add(ActionState.MOVE_R2);
                    
                    // 左回りに一回転
                    actionStateQueue.Add(ActionState.FAST_MOVE_R3);
                    actionStateQueue.Add(ActionState.MOVE_L3);
                    actionStateQueue.Add(ActionState.MOVE_L1);

                    actionStateQueue.Add(ActionState.LOOP);

                }
                else
                {
                    // 第一形態

                    AudioManager.Instance.PlayExVoice("frimomen_start");

                    actionStateQueue.Add(ActionState.WAIT);
                    actionStateQueue.Add(ActionState.JUMP_OUT);

                    // 下段タックル
                    AddTuckleQueue(1);

                    // 下段か中段に現れて三回投げる
                    actionStateQueue.Add(ActionState.WARP_TO_R1);
                    if (Random.Range(0, 1.0f) < 0.5f) actionStateQueue.Add(ActionState.JUMP_TO_C1);
                    else actionStateQueue.Add(ActionState.JUMP_TO_C2);
                    actionStateQueue.Add(ActionState.THROW_BULLET_NORMAL);
                    actionStateQueue.Add(ActionState.THROW_BULLET_NORMAL);
                    actionStateQueue.Add(ActionState.THROW_BULLET_NORMAL);

                    // その場または上段に跳ぶかしてから召喚
                    if (Random.Range(0, 1.0f) < 0.5f) actionStateQueue.Add(ActionState.JUMP_TO_C3);
                    actionStateQueue.Add(ActionState.SUMMON_BUT);

                    // 下段+中段か上段どちらかの二連続タックル
                    actionStateQueue.Add(ActionState.JUMP_TO_R1);
                    AddTuckleQueue(1);
                    if (Random.Range(0, 1.0f) < 0.5f) AddTuckleQueue(2);
                    else AddTuckleQueue(3);

                    // 下段か上段に現れて召喚
                    actionStateQueue.Add(ActionState.WARP_TO_R1);
                    if (Random.Range(0, 1.0f) < 0.5f) actionStateQueue.Add(ActionState.JUMP_TO_C1);
                    else actionStateQueue.Add(ActionState.JUMP_TO_C3);
                    actionStateQueue.Add(ActionState.SUMMON_BUT);

                    // 中段で二回投げる
                    actionStateQueue.Add(ActionState.JUMP_TO_C2);
                    actionStateQueue.Add(ActionState.THROW_BULLET_NORMAL);
                    actionStateQueue.Add(ActionState.THROW_BULLET_NORMAL);

                    // 上段で二回投げる
                    actionStateQueue.Add(ActionState.JUMP_TO_C3);
                    actionStateQueue.Add(ActionState.THROW_BULLET_HEAVY);
                    actionStateQueue.Add(ActionState.THROW_BULLET_HEAVY);

                    // ランダム要素強めに三連続タックル
                    actionStateQueue.Add(ActionState.JUMP_TO_R1);
                    if (Random.Range(0, 1.0f) < 0.5f) AddTuckleQueue(3);
                    else AddTuckleQueue(1);
                    if (Random.Range(0, 1.0f) < 0.5f) AddTuckleQueue(2);
                    else AddTuckleQueue(3);
                    if (Random.Range(0, 1.0f) < 0.5f) AddTuckleQueue(1);
                    else AddTuckleQueue(2);

                    // 下段中段上段でそれぞれ二回ずつ投げる
                    actionStateQueue.Add(ActionState.WARP_TO_R1);
                    actionStateQueue.Add(ActionState.JUMP_TO_C1);
                    actionStateQueue.Add(ActionState.THROW_BULLET_LIGHT);
                    actionStateQueue.Add(ActionState.THROW_BULLET_LIGHT);
                    actionStateQueue.Add(ActionState.JUMP_TO_C2);
                    actionStateQueue.Add(ActionState.THROW_BULLET_NORMAL);
                    actionStateQueue.Add(ActionState.THROW_BULLET_NORMAL);
                    actionStateQueue.Add(ActionState.JUMP_TO_C3);
                    actionStateQueue.Add(ActionState.THROW_BULLET_HEAVY);
                    actionStateQueue.Add(ActionState.THROW_BULLET_HEAVY);

                    // 中段+下段または上段タックル
                    actionStateQueue.Add(ActionState.JUMP_TO_R1);
                    AddTuckleQueue(2);
                    if (Random.Range(0, 1.0f) < 0.5f) AddTuckleQueue(1);
                    else AddTuckleQueue(3);

                    // 下段で三回投げる
                    actionStateQueue.Add(ActionState.WARP_TO_R1);
                    actionStateQueue.Add(ActionState.JUMP_TO_C1);
                    actionStateQueue.Add(ActionState.THROW_BULLET_LIGHT);
                    actionStateQueue.Add(ActionState.THROW_BULLET_NORMAL);
                    actionStateQueue.Add(ActionState.THROW_BULLET_HEAVY);

                    // その場で召喚
                    actionStateQueue.Add(ActionState.SUMMON_BUT);

                    actionStateQueue.Add(ActionState.LOOP);
                }
                break;
            case ActionState.LOOP:
                isPlaying = false;
                stateIndex = 0;
                break;
            case ActionState.JUMP_OUT:
                isPlaying = true;
                PlayJumpSequence(jumpOutPos, 5);
                break;
            case ActionState.JUMP_TO_C1:
                isPlaying = true;
                PlayJumpSequence(c1Pos, 3);
                break;
            case ActionState.JUMP_TO_C2:
                isPlaying = true;
                PlayJumpSequence(c2Pos, 3);
                break;
            case ActionState.JUMP_TO_C3:
                isPlaying = true;
                PlayJumpSequence(c3Pos, 3);
                break;
            case ActionState.JUMP_TO_R1:
                isPlaying = true;
                PlayJumpSequence(r1Pos, 3);
                break;
            case ActionState.WARP_TO_R1:
                isPlaying = false;
                transform.position = r1Pos;
                break;
            case ActionState.WARP_TO_R2:
                isPlaying = false;
                transform.position = r2Pos;
                break;
            case ActionState.WARP_TO_R3:
                isPlaying = false;
                transform.position = r3Pos;
                break;
            case ActionState.DANGER_1:
                isPlaying = true;
                PlayDangerSequence(dangerPanel1);
                break;
            case ActionState.DANGER_2:
                isPlaying = true;
                PlayDangerSequence(dangerPanel2);
                break;
            case ActionState.DANGER_3:
                isPlaying = true;
                PlayDangerSequence(dangerPanel3);
                break;
            case ActionState.TUCKLE_TO_L1:
                isPlaying = true;
                PlayTuckleSequence(l1Pos);
                break;
            case ActionState.TUCKLE_TO_L2:
                isPlaying = true;
                PlayTuckleSequence(l2Pos);
                break;
            case ActionState.TUCKLE_TO_L3:
                isPlaying = true;
                PlayTuckleSequence(l3Pos);
                break;
            case ActionState.THROW_BULLET_NORMAL:
                isPlaying = true;
                PlayThrowSequence(IsLifeHalf()? throwBulletRed : throwBulletBlue);
                break;
            case ActionState.THROW_BULLET_LIGHT:
                isPlaying = true;
                PlayThrowSequence(throwBulletGreen);
                break;
            case ActionState.THROW_BULLET_HEAVY:
                isPlaying = true;
                PlayThrowSequence(throwBulletPurple);
                break;
            case ActionState.SUMMON_BUT:
                isPlaying = true;
                PlaySummonSequence();
                break;
                
            case ActionState.MOVE_R1:
                isPlaying = true;
                PlayMoveSequence(r1Pos, 2.0f);
                break;
            case ActionState.MOVE_R2:
                isPlaying = true;
                PlayMoveSequence(r2Pos, 2.0f);
                break;
            case ActionState.MOVE_R3:
                isPlaying = true;
                PlayMoveSequence(r3Pos, 2.0f);
                break;
            case ActionState.MOVE_L1:
                isPlaying = true;
                PlayMoveSequence(l1Pos, 2.0f);
                break;
            case ActionState.MOVE_L2:
                isPlaying = true;
                PlayMoveSequence(l2Pos, 2.0f);
                break;
            case ActionState.MOVE_L3:
                isPlaying = true;
                PlayMoveSequence(l3Pos, 2.0f);
                break;
            case ActionState.FAST_MOVE_R1:
                isPlaying = true;
                PlayMoveSequence(r1Pos, 0.3f);
                break;
            case ActionState.FAST_MOVE_R2:
                isPlaying = true;
                PlayMoveSequence(r2Pos, 0.3f);
                break;
            case ActionState.FAST_MOVE_R3:
                isPlaying = true;
                PlayMoveSequence(r3Pos, 0.3f);
                break;
            case ActionState.FAST_MOVE_L1:
                isPlaying = true;
                PlayMoveSequence(l1Pos, 0.3f);
                break;
            case ActionState.FAST_MOVE_L2:
                isPlaying = true;
                PlayMoveSequence(l2Pos, 0.3f);
                break;
            case ActionState.FAST_MOVE_L3:
                isPlaying = true;
                PlayMoveSequence(l3Pos, 0.3f);
                break;
            case ActionState.BEAM:
                isPlaying = true;
                sequence = DOTween.Sequence()
                    .AppendCallback(() => { 
                        anim.SetBool("isBeam", true);
                        AudioManager.Instance.PlaySE("frimomen_beam");
                    })
                    .AppendInterval(2.0f)
                    .OnComplete(() => { 
                        anim.SetBool("isBeam", false);
                        isPlaying = false;
                    })
                    .Play();
                break;

            case ActionState.CHANGE_IS_RIGHT:
                isRight = !isRight;
                break;
            case ActionState.WAIT:
                isPlaying = true;
                sequence = DOTween.Sequence()
                    .AppendInterval(1.0f)
                    .OnComplete(() => { isPlaying = false; })
                    .Play();
                break;

        }
        state = actionStateQueue[stateIndex];
        stateIndex++;
    }
    
    private void AddTuckleQueue(int pos)
    {
        switch (pos)
        {
            case 1:
                actionStateQueue.Add(ActionState.WARP_TO_R1);
                actionStateQueue.Add(ActionState.DANGER_1);
                actionStateQueue.Add(ActionState.TUCKLE_TO_L1);
                break;
            case 2:
                actionStateQueue.Add(ActionState.WARP_TO_R2);
                actionStateQueue.Add(ActionState.DANGER_2);
                actionStateQueue.Add(ActionState.TUCKLE_TO_L2);
                break;
            case 3:
                actionStateQueue.Add(ActionState.WARP_TO_R3);
                actionStateQueue.Add(ActionState.DANGER_3);
                actionStateQueue.Add(ActionState.TUCKLE_TO_L3);
                break;
        }
    }

    private void PlayDangerSequence(Animator dangerAnimator)
    {
        sequence = DOTween.Sequence()
            .AppendCallback(() => {
                if (IsLifeHalf())
                {
                    dangerAnimator.SetTrigger("isFastOn");
                } else
                {
                    dangerAnimator.SetTrigger("isOn");
                    AudioManager.Instance.PlayExVoice("frimomen_danger");
                }
            })
            .AppendInterval(IsLifeHalf() ? 1.0f : 2.5f)
            .OnComplete(() => { 
                isPlaying = false;
            })
            .Play();
    }
    private void PlayJumpSequence(Vector2 targetPos, float jumpPower)
    {
        sequence = DOTween.Sequence()
            .AppendCallback(() => {
                anim.SetBool("isJump", true);
                AudioManager.Instance.PlaySE("enemy_jump");
            })
            .Append(transform.DOJump(targetPos, jumpPower, 1, 0.7f))
            .OnComplete(() => { 
                anim.SetBool("isJump", false);
                isPlaying = false;
            })
            .Play();
    }
    private void PlayTuckleSequence(Vector2 targetPos)
    {
        sequence = DOTween.Sequence()
            .AppendCallback(() => { 
                anim.SetBool("isTuckle", true);
                AudioManager.Instance.PlayExVoice("frimomen_tuckle");
                AudioManager.Instance.PlaySE("shot_kiritanhou");
            })
            .Append(transform.DOMove(targetPos, 1.0f)).SetEase(Ease.Linear)
            .OnComplete(() => { 
                anim.SetBool("isTuckle", false);
                isPlaying = false;
            })
            .Play();
    }
    private void PlayMoveSequence(Vector2 targetPos, float time)
    {
        sequence = DOTween.Sequence()
            .AppendCallback(() => {
                AudioManager.Instance.PlaySE("buon2");
            })
            .Append(transform.DOMove(targetPos, time))
            .OnComplete(() => { 
                isPlaying = false;
            })
            .Play();
    }
    private void PlayThrowSequence(GameObject bulletObj)
    {
        GameObject bullet = null, bullet2 = null, bullet3 = null;
        Vector2 addForceVector = Vector2.zero, addForceVector2 = Vector2.zero, addForceVector3 = Vector2.zero;
        
        bullet = Instantiate(bulletObj);
        bullet.SetActive(false);
        bullet.transform.position = bulletPivot.transform.position;
        addForceVector = CalcAddForceVector(Random.Range(90, 180), Random.Range(500, 1000));

        if (IsLifeTwoThirds())
        {
            bullet2 = Instantiate(bulletObj, transform.parent);
            bullet2.SetActive(false);
            bullet2.transform.position = bulletPivot.transform.position;
            addForceVector2 = CalcAddForceVector(Random.Range(90, 180), Random.Range(700, 1000));
        }
        if (IsLifeOneThirds())
        {
            bullet3 = Instantiate(bulletObj, transform.parent);
            bullet3.SetActive(false);
            bullet3.transform.position = bulletPivot.transform.position;
            addForceVector3 = CalcAddForceVector(Random.Range(90, 180), Random.Range(500, 700));
        }

        sequence = DOTween.Sequence()
            .AppendCallback(() =>
            {
                anim.SetBool("isThrow", true);
            })
            .AppendInterval(0.1f)
            .AppendCallback(() =>
            {
                bullet.SetActive(true);
                if (bullet2 != null) bullet2.SetActive(true);
                if (bullet3 != null) bullet3.SetActive(true);
            })
            .AppendInterval(0.3f)
            .AppendCallback(() =>
            {
                AudioManager.Instance.PlayExVoice("frimomen_throw");
                AudioManager.Instance.PlaySE("frimomen_throw_se");
                bullet.GetComponent<Rigidbody2D>().AddForce(addForceVector);
            })
            .AppendInterval(0.1f)
            .AppendCallback(() =>
            {
                if (bullet2 != null) bullet2.GetComponent<Rigidbody2D>().AddForce(addForceVector2);
            })
            .AppendInterval(0.1f)
            .AppendCallback(() =>
            {
                if (bullet3 != null) bullet3.GetComponent<Rigidbody2D>().AddForce(addForceVector3);
            })
            .AppendInterval(0.3f)
            .OnComplete(() => { 
                anim.SetBool("isThrow", false);
                isPlaying = false;
            })
            .Play();
    }

    private void PlaySummonSequence()
    {
        Instantiate(summonBat, transform.parent).transform.position = r1Pos;
        Instantiate(summonBat, transform.parent).transform.position = l1Pos;

        if (IsLifeHalf())
        {
            Instantiate(summonBat, transform.parent).transform.position = r4Pos;
            Instantiate(summonBat, transform.parent).transform.position = l4Pos;
        }

        sequence = DOTween.Sequence()
            .AppendCallback(() => { 
                anim.SetBool("isThrow", true);
                AudioManager.Instance.PlayExVoice("frimomen_summon");
                AudioManager.Instance.PlaySE("buon2");
            })
            .Append(transform.DOJump(transform.position, 1.0f, 1, 0.5f))
            .AppendInterval(2.0f)
            .OnComplete(() => { 
                anim.SetBool("isThrow", false);
                isPlaying = false;
            })
            .Play();
    }
    
    private void InstantiateWallBullet()
    {
        var wallObject = Instantiate(wallBullet, transform.parent);
        var wallScript = wallObject.GetComponent<WallBullet>();

        wallObject.transform.localScale = new Vector2(wallObject.transform.localScale.x / 4 * Random.Range(1, 8), wallObject.transform.localScale.y);
        wallScript.moveTime = Random.Range(1, 3);
        wallScript.deadTime = wallScript.moveTime + Random.Range(0, 2);

        switch(Random.Range(0, 3))
        {
            case 0: // 左に生成
                wallObject.transform.localPosition = new Vector2(6, Random.Range(-5, 3));
                wallObject.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 90));
                wallScript.moveX = 6;
                wallScript.moveY = 0;
                break;
                
            case 1: // 右に生成
                wallObject.transform.localPosition = new Vector2(25, Random.Range(-5, 3));
                wallObject.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 90));
                wallScript.moveX = -6;
                wallScript.moveY = 0;
                break;
                
            case 2:
            case 3: // 下に生成
                wallObject.transform.localPosition = new Vector2(Random.Range(8, 24), -6);
                wallObject.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
                wallObject.transform.localScale = new Vector2(wallObject.transform.localScale.x * 1.5f, wallObject.transform.localScale.y);
                wallScript.moveX = 0;
                wallScript.moveY = 6;
                break;
        }
    }

    Vector2 CalcAddForceVector(float angleZ, float power)
    {
        float addforceX = Mathf.Cos(angleZ * Mathf.Deg2Rad) * power;
        float addforceY = Mathf.Sin(angleZ * Mathf.Deg2Rad) * power;
        return new Vector2(addforceX, addforceY);
    }
}
