﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using DG.Tweening;

public class Frimomen : BossAIBase
{
    // 青:平均的, 緑:ふんわり落ちてくる, 赤:ダメージ1.5倍, 紫:落下が速い
    public GameObject throwBulletBlue, throwBulletRed, throwBulletGreen, throwBulletPurple;
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
        THROW_BULLET_NORMAL,
        THROW_BULLET_SLOW,
        THROW_BULLET_LIGHT,
        THROW_BULLET_HEAVY,
        CHANGE_IS_RIGHT,
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
            AudioManager.Instance.PlayExVoice("frimomen_dead");
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

        // アニメーション再生中は次のモードに遷移しない
        if (isPlaying)
        {
            return;
        }

        Debug.Log(state);

        switch (state)
        {
            case ActionState.START:
                AudioManager.Instance.PlayExVoice("frimomen_start");
                
                actionStateQueue.Add(ActionState.WAIT);
                actionStateQueue.Add(ActionState.JUMP_OUT);

                actionStateQueue.Add(ActionState.WARP_TO_R1);
                actionStateQueue.Add(ActionState.DANGER_1);
                actionStateQueue.Add(ActionState.TUCKLE_TO_L1);
                

                actionStateQueue.Add(ActionState.WARP_TO_R1);
                actionStateQueue.Add(ActionState.JUMP_TO_C1);
                
                actionStateQueue.Add(ActionState.THROW_BULLET_NORMAL);
                actionStateQueue.Add(ActionState.THROW_BULLET_NORMAL);
                actionStateQueue.Add(ActionState.THROW_BULLET_NORMAL);

                actionStateQueue.Add(ActionState.JUMP_TO_R1);

                actionStateQueue.Add(ActionState.WARP_TO_R1);
                actionStateQueue.Add(ActionState.DANGER_1);
                actionStateQueue.Add(ActionState.TUCKLE_TO_L1);
                
                actionStateQueue.Add(ActionState.WARP_TO_R2);
                actionStateQueue.Add(ActionState.DANGER_2);
                actionStateQueue.Add(ActionState.TUCKLE_TO_L2);

                actionStateQueue.Add(ActionState.WARP_TO_R3);
                actionStateQueue.Add(ActionState.DANGER_3);
                actionStateQueue.Add(ActionState.TUCKLE_TO_L3);
                
                actionStateQueue.Add(ActionState.WARP_TO_R1);
                actionStateQueue.Add(ActionState.JUMP_TO_C1);
                
                actionStateQueue.Add(ActionState.THROW_BULLET_LIGHT);
                actionStateQueue.Add(ActionState.THROW_BULLET_NORMAL);
                actionStateQueue.Add(ActionState.THROW_BULLET_HEAVY);

                actionStateQueue.Add(ActionState.LOOP);

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
            .Append(transform.DOLocalJump(targetPos, jumpPower, 1, 0.7f))
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
        float power = Random.Range(700, 1200);
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
                AudioManager.Instance.PlayExVoice("frimomen_throw");
                AudioManager.Instance.PlaySE("swing");
                bullet.GetComponent<Rigidbody2D>().AddForce(new Vector2(addforceX, addforceY));
            })
            .AppendInterval(0.5f)
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
