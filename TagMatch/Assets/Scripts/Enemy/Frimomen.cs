using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using DG.Tweening;

public class Frimomen : BossAIBase
{
    public bool isAstral;
    public GameObject throwBullet;
    BoxCollider2D bc;
    GameObject bulletPivot;
    Vector2 l1Pos, l2Pos, l3Pos, r1Pos, r2Pos, r3Pos, c1Pos, jumpOutPos;
    Animator dangerPanel1, dangerPanel2, dangerPanel3;

    enum ActionState
    {
        START,
        JUMP_OUT,
        JUMP_TO_C1,
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
        THROW_BULLET_1,
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
        l2Pos = GameObject.Find("L2Pos").transform.position;
        l3Pos = GameObject.Find("L3Pos").transform.position;
        r1Pos = GameObject.Find("R1Pos").transform.position;
        r2Pos = GameObject.Find("R2Pos").transform.position;
        r3Pos = GameObject.Find("R3Pos").transform.position;
        c1Pos = GameObject.Find("C1Pos").transform.position;
        jumpOutPos = GameObject.Find("JumpOutPos").transform.position;
        dangerPanel1 = GameObject.Find("DangerPanel1").GetComponent<Animator>();
        dangerPanel2 = GameObject.Find("DangerPanel2").GetComponent<Animator>();
        dangerPanel3 = GameObject.Find("DangerPanel3").GetComponent<Animator>();
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
        transform.localScale = new Vector2(isRight ? -1 : 1, 1);

        if (!isDead && bossScript.IsDead())
        {
            AudioManager.Instance.PlayExVoice("yukari_dead");
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

        if (isAstral && !IsLifeHalf())
        {
            bc.enabled = false;
            return;
        }
        
        // アニメーション再生中は次のモードに遷移しない
        if (isPlaying)
        {
            return;
        }

        Debug.Log(state);

        switch (state)
        {
            case ActionState.START:
                //else AudioManager.Instance.PlayExVoice("itako_start"); // TODO: 開始ボイス作る
                
                isPlaying = true;
                PlayJumpSequence(jumpOutPos); // ループには含めず、初回だけジャンプでいなくなる
                
                actionStateQueue.Add(ActionState.WAIT);
                
                //actionStateQueue.Add(ActionState.WARP_TO_R1);
                //actionStateQueue.Add(ActionState.DANGER_1);
                //actionStateQueue.Add(ActionState.TUCKLE_TO_L1);
                
                //actionStateQueue.Add(ActionState.WARP_TO_R2);
                //actionStateQueue.Add(ActionState.DANGER_2);
                //actionStateQueue.Add(ActionState.TUCKLE_TO_L2);
                
                //actionStateQueue.Add(ActionState.WARP_TO_R3);
                //actionStateQueue.Add(ActionState.DANGER_3);
                //actionStateQueue.Add(ActionState.TUCKLE_TO_L3);

                actionStateQueue.Add(ActionState.WARP_TO_R1);
                actionStateQueue.Add(ActionState.JUMP_TO_C1);
                actionStateQueue.Add(ActionState.WAIT);
                actionStateQueue.Add(ActionState.WAIT);
                actionStateQueue.Add(ActionState.THROW_BULLET_1);
                actionStateQueue.Add(ActionState.THROW_BULLET_1);
                actionStateQueue.Add(ActionState.THROW_BULLET_1);
                actionStateQueue.Add(ActionState.WAIT);
                actionStateQueue.Add(ActionState.JUMP_TO_R1);
                
                actionStateQueue.Add(ActionState.LOOP);

                break;
            case ActionState.LOOP:
                isPlaying = false;
                stateIndex = 0;
                break;
            case ActionState.JUMP_OUT:
                isPlaying = true;
                PlayJumpSequence(jumpOutPos);
                break;
            case ActionState.JUMP_TO_C1:
                isPlaying = true;
                PlayJumpSequence(c1Pos);
                break;
            case ActionState.JUMP_TO_R1:
                isPlaying = true;
                PlayJumpSequence(r1Pos);
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
            case ActionState.THROW_BULLET_1:
                isPlaying = true;
                PlayThrowSequence(throwBullet);
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
    
    private void PlayDangerSequence(Animator dangerAnimator)
    {
        sequence = DOTween.Sequence()
            .AppendCallback(() => {
                dangerAnimator.SetTrigger("isOn");
            })
            .AppendInterval(IsLifeHalf() ? 1.0f : 2.5f)
            .OnComplete(() => { 
                isPlaying = false;
            })
            .Play();
    }
    private void PlayJumpSequence(Vector2 targetPos)
    {
        sequence = DOTween.Sequence()
            .AppendCallback(() => {
                anim.SetBool("isJump", true);
            })
            .Append(transform.DOLocalJump(targetPos, 5.0f, 1, 1.0f))
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
            })
            .Append(transform.DOLocalMove(targetPos, 1.0f)).SetEase(Ease.Linear)
            .OnComplete(() => { 
                anim.SetBool("isTuckle", false);
                isPlaying = false;
            })
            .Play();
    }
    private void PlayThrowSequence(GameObject bulletObj)
    {
        GameObject bullet = Instantiate(bulletObj);
        bullet.SetActive(false);
        bullet.transform.position = bulletPivot.transform.position;

        float angleZ = Random.Range(90, 180);
        float power = Random.Range(550, 800);
        float addforceX = Mathf.Cos(angleZ * Mathf.Deg2Rad) * 550.0f;
        float addforceY = Mathf.Sin(angleZ * Mathf.Deg2Rad) * 550.0f;

        sequence = DOTween.Sequence()
            .AppendCallback(() =>
            {
                anim.SetBool("isThrow", true);
            })
            .AppendInterval(0.1f)
            .AppendCallback(() =>
            {
                bullet.SetActive(true);
            })
            .AppendInterval(0.3f)
            .AppendCallback(() =>
            {
                bullet.GetComponent<Rigidbody2D>().AddForce(new Vector2(addforceX, addforceY));
            })
            .AppendInterval(1.0f)
            .OnComplete(() => { 
                anim.SetBool("isThrow", false);
                isPlaying = false;
            })
            .Play();
    }

    bool IsLifeHalf()
    {
        return (StaticValues.bossHP.Sum() < (StaticValues.bossMaxHP.Sum() / 2));
    }
}
