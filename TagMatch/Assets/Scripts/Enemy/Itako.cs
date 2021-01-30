using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using DG.Tweening;

public class Itako : BossAIBase
{
    public GameObject flameBullet, ghostRotationBullet;
    BoxCollider2D bc;
    Vector2 upperLeftPos, upperRightPos, upperCenterPos, lowerLeftPos, lowerRightPos, lowerCenterPos;

    enum ActionState
    {
        IDLE,
        MOVE_TO_UPPER_LEFT,
        MOVE_TO_UPPER_RIGHT,
        MOVE_TO_UPPER_CENTER,
        MOVE_TO_LOWER_LEFT,
        MOVE_TO_LOWER_RIGHT,
        MOVE_TO_LOWER_CENTER,
        CHARGE,
        FLAME_SHOT,
        FLAME_TWIN_SHOT,
        GHOST_SHOT,
    }
    ActionState state;
    List<ActionState> actionStateQueue = new List<ActionState>();


    // Start is called before the first frame update
    void Start()
    {
        bc = GetComponent<BoxCollider2D>();
        anim = GetComponentInChildren<Animator>();
        bossScript = GetComponent<Boss>();
        state = ActionState.IDLE;
        upperLeftPos = GameObject.Find("UpperLeftPos").transform.position;
        upperRightPos = GameObject.Find("UpperRightPos").transform.position;
        upperCenterPos = GameObject.Find("UpperCenterPos").transform.position;
        lowerLeftPos = GameObject.Find("LowerLeftPos").transform.position;
        lowerRightPos = GameObject.Find("LowerRightPos").transform.position;
        lowerCenterPos = GameObject.Find("LowerCenterPos").transform.position;
    }
    
    public override void Reset()
    {
        base.Reset();
        anim.SetBool("isReady", false);
        anim.SetBool("isAttack", false);
        state = ActionState.IDLE;
    }

    // Update is called once per frame
    void Update()
    {
        if (!bossScript.IsActive())
        {
            return;
        }

        transform.localScale = new Vector2(isRight ? 1 : -1, 1);
        
        if (isDead)
        {
            return;
        }

        if (bossScript.IsDead())
        {
            sequence.Kill();
            isRight = false;
            anim.SetBool("isFloat", false);
            anim.SetBool("isReady", false);
            isDead = true;
            return;
        }

        
        // アニメーション再生中は次のモードに遷移しない
        if (isPlaying)
        {
            return;
        }

        switch (state)
        {
            case ActionState.IDLE:
                isPlaying = false;
                stateIndex = 0;
                actionStateQueue.Add(ActionState.MOVE_TO_UPPER_LEFT);
                actionStateQueue.Add(ActionState.FLAME_SHOT);
                actionStateQueue.Add(ActionState.CHARGE);
                actionStateQueue.Add(ActionState.GHOST_SHOT);
                actionStateQueue.Add(ActionState.MOVE_TO_UPPER_RIGHT);
                actionStateQueue.Add(ActionState.FLAME_SHOT);
                actionStateQueue.Add(ActionState.CHARGE);
                actionStateQueue.Add(ActionState.GHOST_SHOT);
                actionStateQueue.Add(ActionState.MOVE_TO_UPPER_CENTER);
                actionStateQueue.Add(ActionState.GHOST_SHOT);
                actionStateQueue.Add(ActionState.CHARGE);
                actionStateQueue.Add(ActionState.FLAME_SHOT);
                actionStateQueue.Add(ActionState.MOVE_TO_LOWER_RIGHT);
                actionStateQueue.Add(ActionState.FLAME_TWIN_SHOT);
                actionStateQueue.Add(ActionState.MOVE_TO_LOWER_LEFT);
                actionStateQueue.Add(ActionState.FLAME_TWIN_SHOT);
                actionStateQueue.Add(ActionState.MOVE_TO_LOWER_CENTER);
                actionStateQueue.Add(ActionState.GHOST_SHOT);
                actionStateQueue.Add(ActionState.IDLE);
                break;
            case ActionState.MOVE_TO_UPPER_LEFT:
                isPlaying = true;
                PlayWarpSequence(upperLeftPos, true);
                break;
            case ActionState.MOVE_TO_UPPER_RIGHT:
                isPlaying = true;
                PlayWarpSequence(upperRightPos, false);
                break;
            case ActionState.MOVE_TO_UPPER_CENTER:
                isPlaying = true;
                PlayWarpSequence(upperCenterPos, false);
                break;
            case ActionState.MOVE_TO_LOWER_LEFT:
                isPlaying = true;
                PlayWarpSequence(lowerLeftPos, true);
                break;
            case ActionState.MOVE_TO_LOWER_RIGHT:
                isPlaying = true;
                PlayWarpSequence(lowerRightPos, false);
                break;
            case ActionState.MOVE_TO_LOWER_CENTER:
                isPlaying = true;
                PlayWarpSequence(lowerCenterPos, true);
                break;
            case ActionState.CHARGE:
                isPlaying = true;
                anim.SetBool("isReady", true);
                sequence = DOTween.Sequence()
                    .AppendInterval(1.0f)
                    .OnComplete(() =>
                    {
                        isPlaying = false;
                    })
                    .Play();
                break;
            case ActionState.FLAME_SHOT:
                isPlaying = true;
                anim.SetBool("isAttack", true);
                sequence = DOTween.Sequence()
                    .AppendInterval(0.2f)
                    .AppendCallback(() => { InstantiateFlame(isRight ? 1.5f : -1.5f); })
                    .AppendInterval(1.5f)
                    .OnComplete(() =>
                    {
                        anim.SetBool("isAttack", false);
                        isPlaying = false;
                    })
                    .Play();
                break;
            case ActionState.FLAME_TWIN_SHOT:
                isPlaying = true;
                anim.SetBool("isAttack", true);
                sequence = DOTween.Sequence()
                    .AppendInterval(0.2f)
                    .AppendCallback(() => { InstantiateFlame(isRight ? 1.5f : -1.5f); })
                    .AppendInterval(0.2f)
                    .AppendCallback(() => { InstantiateFlame(isRight ? 3.0f : -3.0f); })
                    .AppendInterval(1.5f)
                    .OnComplete(() =>
                    {
                        anim.SetBool("isAttack", false);
                        isPlaying = false;
                    })
                    .Play();
                break;
            case ActionState.GHOST_SHOT:
                isPlaying = true;
                anim.SetBool("isAttack", true);
                sequence = DOTween.Sequence()
                    .AppendInterval(0.2f)
                    .AppendCallback(() => { InstantiateGhostRotation(); })
                    .AppendInterval(1.5f)
                    .OnComplete(() =>
                    {
                        anim.SetBool("isAttack", false);
                        isPlaying = false;
                    })
                    .Play();
                break;

        }
        state = actionStateQueue[stateIndex];
        stateIndex++;
    }

    public virtual void PlayWarpSequence(Vector2 targetPos, bool movedIsRight) 
    {
        sequence = DOTween.Sequence()
            .AppendCallback(() => { 
                AudioManager.Instance.PlaySE("buon"); // TODO: ドロン系の音にしたい かくれるが更新されたらそれを当ててみる
                anim.SetBool("isReady", true);
                anim.SetBool("isDisappear", true);
                bc.enabled = false;
            })
            .AppendInterval(IsLifeHalf() ? 0.8f : 1.6f)
            .AppendCallback(() => { 
                transform.position = targetPos;
                isRight = movedIsRight;
            })
            .AppendCallback(() => { 
                anim.SetBool("isDisappear", false);
                bc.enabled = true;
            })
            .AppendInterval(2.0f)
            .OnComplete(() => { isPlaying = false; })
            .Play();
    }
    
    void InstantiateFlame(float addPosX)
    {
        AudioManager.Instance.PlaySE("fire");

        GameObject bullet = Instantiate(flameBullet, transform.parent);
        bullet.transform.position = new Vector2(transform.position.x + addPosX, transform.position.y + 1.5f);
    }

    void InstantiateGhostRotation()
    {
        AudioManager.Instance.PlaySE("buon");

        GameObject bullet = Instantiate(ghostRotationBullet, transform.parent);
        bullet.transform.position = transform.position;
        bullet.transform.localScale = new Vector2(isRight ? -1.0f : 1.0f, 1.0f);
    }

    bool IsLifeHalf()
    {
        return (StaticValues.bossHP.Sum() < (StaticValues.bossMaxHP.Sum() / 2));
    }
}
