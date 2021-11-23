using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using DG.Tweening;

public class BossMaki : BossAIBase
{
    public GameObject mustangBullet, rollingBullet, barrierBuller;
    BoxCollider2D bc;
    GameObject bulletPivot;
    Vector2 l1Pos, r1Pos, c1Pos, c2Pos;
    const float BARRIER_INVINCIBLE_TIME = 3.0f;

    enum ActionState
    {
        START,
        SHOT,
        JUMP_SHOT,
        BARRIER,
        RUN_TO_L1,
        RUN_TO_R1,
        JUMP,
        ROLLING_JUMP,
        JUMP_TO_L1,
        JUMP_TO_R1,
        JUMP_TO_C2_1,
        JUMP_TO_C2_2,
        CHANGE_IS_RIGHT,
        LOOP,
        WAIT,
    }
    ActionState state;
    List<ActionState> actionStateQueue = new List<ActionState>();
    
    enum AnimationState { STAND = 0, RUN = 1, JUMP = 2, SQUAT = 3};
    AnimationState animationState;

    // Start is called before the first frame update
    void Start()
    {
        bc = GetComponent<BoxCollider2D>();
        anim = GetComponentInChildren<Animator>();
        bossScript = GetComponent<Boss>();
        state = ActionState.START;
        
        bulletPivot = transform.Find("BulletPivot").gameObject;
        l1Pos = GameObject.Find("L1Pos").transform.position;
        r1Pos = GameObject.Find("R1Pos").transform.position;
        c1Pos = GameObject.Find("C1Pos").transform.position;
        c2Pos = GameObject.Find("C2Pos").transform.position;
    }
    
    public override void Reset()
    {
        base.Reset();
        animationState = AnimationState.STAND;
        anim.SetInteger("state", (int)animationState);
        actionStateQueue.Clear();
        stateIndex = 0;
        state = ActionState.START;
        bc.enabled = true;
    }

    // Update is called once per frame
    void Update()
    {
        transform.localScale = new Vector2(isRight ? 1 : -1, 1);

        if (!isDead && bossScript.IsDead())
        {
            AudioManager.Instance.PlayExVoice("maki_dead");
            sequence.Kill();
            isRight = false;
            animationState = AnimationState.STAND;
            anim.SetInteger("state", (int)animationState);
            isDead = true;
            return;
        }

        if (isDead || !bossScript.IsActive())
        {
            return;
        }
        
        // アニメーション再生中は次のモードに遷移しない
        if (isPlaying)
        {
            return;
        }

        switch (state)
        {
            case ActionState.START:
                //else AudioManager.Instance.PlayExVoice("itako_start"); // TODO: マキ開始ボイス作る

                actionStateQueue.Add(ActionState.WAIT);
                
                // 手始めにゆっくり攻撃、ジャンプ攻撃
                actionStateQueue.Add(ActionState.SHOT);
                actionStateQueue.Add(ActionState.WAIT);
                actionStateQueue.Add(ActionState.JUMP_SHOT);
                actionStateQueue.Add(ActionState.WAIT);
                
                // (HP半分切ってたらバリアを貼りながら) 左端に移動して回転攻撃
                actionStateQueue.Add(ActionState.BARRIER);
                actionStateQueue.Add(ActionState.RUN_TO_L1);
                actionStateQueue.Add(ActionState.CHANGE_IS_RIGHT);
                actionStateQueue.Add(ActionState.ROLLING_JUMP);
                
                // 攻撃かジャンプ攻撃のどちらかをランダムに二発
                if (Random.Range(0, 1.0f) < 0.5f) actionStateQueue.Add(ActionState.SHOT);
                else actionStateQueue.Add(ActionState.JUMP_SHOT);
                actionStateQueue.Add(ActionState.WAIT);
                if (Random.Range(0, 1.0f) < 0.5f) actionStateQueue.Add(ActionState.SHOT);
                else actionStateQueue.Add(ActionState.JUMP_SHOT);
                actionStateQueue.Add(ActionState.WAIT);
                
                // 攻撃、回転、ジャンプ攻撃、攻撃のセット (ジャンプ、しゃがみ、ジャンプで躱す 
                actionStateQueue.Add(ActionState.SHOT);
                actionStateQueue.Add(ActionState.ROLLING_JUMP);
                actionStateQueue.Add(ActionState.JUMP_SHOT);
                actionStateQueue.Add(ActionState.SHOT);
                
                // (HP半分切ってたらバリアを貼りながら) 右端に移動して回転攻撃
                actionStateQueue.Add(ActionState.BARRIER);
                actionStateQueue.Add(ActionState.RUN_TO_R1);
                actionStateQueue.Add(ActionState.CHANGE_IS_RIGHT);
                actionStateQueue.Add(ActionState.ROLLING_JUMP);
                
                // ジャンプ攻撃多めの連続攻撃
                actionStateQueue.Add(ActionState.JUMP_SHOT);
                if (Random.Range(0, 1.0f) < 0.5f) actionStateQueue.Add(ActionState.SHOT);
                else actionStateQueue.Add(ActionState.JUMP_SHOT);
                actionStateQueue.Add(ActionState.JUMP_SHOT);
                if (Random.Range(0, 1.0f) < 0.5f) actionStateQueue.Add(ActionState.SHOT);
                else actionStateQueue.Add(ActionState.JUMP_SHOT);

                // 上経由で左端へ
                actionStateQueue.Add(ActionState.JUMP_TO_C2_1);
                actionStateQueue.Add(ActionState.JUMP_TO_C2_2);
                actionStateQueue.Add(ActionState.JUMP_TO_L1);
                actionStateQueue.Add(ActionState.CHANGE_IS_RIGHT);
                
                // 空中ダッシュしないと避けにくい二連続攻撃
                actionStateQueue.Add(ActionState.BARRIER);
                actionStateQueue.Add(ActionState.SHOT);
                actionStateQueue.Add(ActionState.SHOT);
                
                // 上経由で右端へ
                actionStateQueue.Add(ActionState.JUMP_TO_C2_1);
                actionStateQueue.Add(ActionState.JUMP_TO_C2_2);
                actionStateQueue.Add(ActionState.JUMP_TO_R1);
                actionStateQueue.Add(ActionState.CHANGE_IS_RIGHT);

                // 下上上下の固定攻撃
                actionStateQueue.Add(ActionState.SHOT);
                actionStateQueue.Add(ActionState.JUMP_SHOT);
                actionStateQueue.Add(ActionState.JUMP_SHOT);
                actionStateQueue.Add(ActionState.SHOT);
                
                // ただ左に行く (HP半分以下ならバリア張りながら)
                actionStateQueue.Add(ActionState.BARRIER);
                actionStateQueue.Add(ActionState.RUN_TO_L1);
                actionStateQueue.Add(ActionState.CHANGE_IS_RIGHT);
                
                // ただ右に行く (HP半分以下ならバリア張りながら)
                actionStateQueue.Add(ActionState.BARRIER);
                actionStateQueue.Add(ActionState.RUN_TO_R1);
                actionStateQueue.Add(ActionState.CHANGE_IS_RIGHT);

                // ランダム要素多めの連撃
                if (Random.Range(0, 1.0f) < 0.5f) actionStateQueue.Add(ActionState.SHOT);
                if (Random.Range(0, 1.0f) < 0.5f) actionStateQueue.Add(ActionState.JUMP_SHOT);
                else actionStateQueue.Add(ActionState.ROLLING_JUMP);
                if (Random.Range(0, 1.0f) < 0.5f) actionStateQueue.Add(ActionState.SHOT);
                else actionStateQueue.Add(ActionState.JUMP_SHOT);
                actionStateQueue.Add(ActionState.JUMP);
                if (Random.Range(0, 1.0f) < 0.5f) actionStateQueue.Add(ActionState.SHOT);
                if (Random.Range(0, 1.0f) < 0.5f) actionStateQueue.Add(ActionState.JUMP_SHOT);
                else actionStateQueue.Add(ActionState.JUMP);
                if (Random.Range(0, 1.0f) < 0.5f) actionStateQueue.Add(ActionState.SHOT);
                if (Random.Range(0, 1.0f) < 0.5f) actionStateQueue.Add(ActionState.JUMP);
                else actionStateQueue.Add(ActionState.ROLLING_JUMP);

                // 上に飛んで元いた右に戻る
                actionStateQueue.Add(ActionState.JUMP_TO_C2_1);
                actionStateQueue.Add(ActionState.JUMP_TO_C2_2);
                actionStateQueue.Add(ActionState.CHANGE_IS_RIGHT);
                actionStateQueue.Add(ActionState.JUMP_TO_R1);
                actionStateQueue.Add(ActionState.CHANGE_IS_RIGHT);
                
                // フェイントを挟みながら下段に三発
                actionStateQueue.Add(ActionState.SHOT);
                actionStateQueue.Add(ActionState.ROLLING_JUMP);
                actionStateQueue.Add(ActionState.SHOT);
                actionStateQueue.Add(ActionState.JUMP);
                actionStateQueue.Add(ActionState.SHOT);

                // 上段に五連発
                actionStateQueue.Add(ActionState.JUMP_SHOT);
                actionStateQueue.Add(ActionState.JUMP_SHOT);
                actionStateQueue.Add(ActionState.JUMP_SHOT);
                actionStateQueue.Add(ActionState.JUMP_SHOT);
                actionStateQueue.Add(ActionState.JUMP_SHOT);

                // 左に行く (HP半分以下ならバリア張りながら)
                actionStateQueue.Add(ActionState.BARRIER);
                actionStateQueue.Add(ActionState.RUN_TO_L1);
                actionStateQueue.Add(ActionState.CHANGE_IS_RIGHT);
                
                // 空中ダッシュしないと避けにくい二連続攻撃を二回
                actionStateQueue.Add(ActionState.SHOT);
                actionStateQueue.Add(ActionState.SHOT);
                actionStateQueue.Add(ActionState.WAIT);
                actionStateQueue.Add(ActionState.SHOT);
                actionStateQueue.Add(ActionState.SHOT);

                // 上経由で右端へ
                actionStateQueue.Add(ActionState.JUMP_TO_C2_1);
                actionStateQueue.Add(ActionState.JUMP_TO_C2_2);
                actionStateQueue.Add(ActionState.JUMP_TO_R1);
                actionStateQueue.Add(ActionState.CHANGE_IS_RIGHT);

                // ループ
                actionStateQueue.Add(ActionState.LOOP);

                break;
            case ActionState.LOOP:
                isPlaying = false;
                stateIndex = 0;
                break;
            case ActionState.SHOT:
                isPlaying = true;
                PlayShotSequence();
                break;
            case ActionState.JUMP:
                isPlaying = true;
                PlayJumpSequence(transform.position, false);
                break;
            case ActionState.JUMP_SHOT:
                isPlaying = true;
                PlayJumpShotSequence();
                break;
            case ActionState.BARRIER:
                isPlaying = true;
                PlayBarrierSequence();
                break;
            case ActionState.RUN_TO_L1:
                isPlaying = true;
                PlayRunSequence(l1Pos);
                break;
            case ActionState.RUN_TO_R1:
                isPlaying = true;
                PlayRunSequence(r1Pos);
                break;
            case ActionState.ROLLING_JUMP:
                isPlaying = true;
                PlayRollingJumpSequence(transform.position);
                break;
            case ActionState.JUMP_TO_L1:
                isPlaying = true;
                PlayJumpSequence(l1Pos, true);
                break;
            case ActionState.JUMP_TO_R1:
                isPlaying = true;
                PlayJumpSequence(r1Pos, true);
                break;
            case ActionState.JUMP_TO_C2_1:
                isPlaying = true;
                PlayJumpSequence(Vector2.Lerp(transform.position, c2Pos, 0.5f), false);
                break;
            case ActionState.JUMP_TO_C2_2:
                isPlaying = true;
                PlayRollingJumpSequence(c2Pos);
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
    
    private void PlayShotSequence()
    {
        sequence = DOTween.Sequence()
            .AppendCallback(() =>
            {
                InstantiateBullet(mustangBullet, isRight ? 0 : 180, animationState == AnimationState.SQUAT);
            })
            .AppendInterval(0.2f)
            .OnComplete(() => { isPlaying = false; })
            .Play();
    }

    private void PlayJumpShotSequence()
    {
        sequence = DOTween.Sequence()
            .AppendCallback(() => {
                animationState = AnimationState.JUMP;
                anim.SetInteger("state", (int)animationState);
                AudioManager.Instance.PlaySE("jump");
            })
            .Append(transform.DOLocalJump(transform.position, 1.0f, 1, 0.5f))
            .Join(
                DOTween.Sequence()
                .AppendInterval(0.25f)
                .AppendCallback(() =>
                {
                    InstantiateBullet(mustangBullet, isRight ? 0 : 180, animationState == AnimationState.SQUAT);
                })
            )
            .AppendInterval(0.2f)
            .OnComplete(() => { 
                animationState = AnimationState.STAND;
                anim.SetInteger("state", (int)animationState);
                isPlaying = false;
            })
            .Play();
    }

    private void PlayBarrierSequence()
    {
        // ライフが半分以上の時は何もせずreturn
        if (!IsLifeHalf())
        {
            isPlaying = false;
            return;
        }
        sequence = DOTween.Sequence()
            .AppendCallback(() =>
            {
                GameObject bullet = Instantiate(barrierBuller, transform);
                bullet.transform.position = transform.position;
                AudioManager.Instance.PlayExVoice("maki_barrier", true);
                bossScript.SetInvincible(BARRIER_INVINCIBLE_TIME);
            })
            .AppendInterval(0.2f)
            .OnComplete(() => { isPlaying = false; })
            .Play();
    }

    private void PlayJumpSequence(Vector2 targetPos, bool isLongJump = false)
    {
        sequence = DOTween.Sequence()
            .AppendCallback(() => { 
                animationState = AnimationState.JUMP;
                anim.SetInteger("state", (int)animationState);
                AudioManager.Instance.PlaySE("jump");
            })
            .Append(transform.DOLocalJump(targetPos, 1.0f, 1, isLongJump ? 1.0f : 0.5f))
            .OnComplete(() => { 
                animationState = AnimationState.STAND;
                anim.SetInteger("state", (int)animationState);
                isPlaying = false;
            })
            .Play();
    }

    private void PlayRollingJumpSequence(Vector2 targetPos)
    {
        sequence = DOTween.Sequence()
            .AppendCallback(() => { 
                animationState = AnimationState.JUMP;
                anim.SetInteger("state", (int)animationState);
                GameObject bullet = Instantiate(rollingBullet, transform);
                bullet.transform.position = transform.position;
                AudioManager.Instance.PlaySE("jump");
                AudioManager.Instance.PlaySE("dash");
                AudioManager.Instance.PlayExVoice("maki_jump", true);
            })
            .Append(transform.DOLocalJump(targetPos, 1.0f, 1, 0.5f))
            .Join(
                DOTween.Sequence()
                .Append(transform.DOLocalRotate(new Vector3(0, 0, 360), 0.4f, RotateMode.FastBeyond360))
            )
            .OnComplete(() => { 
                animationState = AnimationState.STAND;
                anim.SetInteger("state", (int)animationState);
                isPlaying = false;
            })
            .Play();
    }
    
    private void PlayRunSequence(Vector2 targetPos)
    {
        sequence = DOTween.Sequence()
            .AppendCallback(() => { 
                animationState = AnimationState.RUN;
                anim.SetInteger("state", (int)animationState);
            })
            .Append(transform.DOLocalMove(targetPos, 2.0f)).SetEase(Ease.Linear)
            .OnComplete(() => { 
                animationState = AnimationState.STAND;
                anim.SetInteger("state", (int)animationState);
                isPlaying = false;
            })
            .Play();
    }

    private void InstantiateBullet(GameObject bulletObj, float angleZ, bool isSquat = false)
    {
        float addforceX = Mathf.Cos(angleZ * Mathf.Deg2Rad) * 400.0f;
        float addforceY = Mathf.Sin(angleZ * Mathf.Deg2Rad) * 400.0f;

        GameObject bullet = Instantiate(bulletObj);

        bullet.transform.position = bulletPivot.transform.position;
        bullet.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angleZ));
        bullet.GetComponent<Rigidbody2D>().AddForce(new Vector2(addforceX, addforceY));

        AudioManager.Instance.PlaySE("shot_maki");
    }
}
