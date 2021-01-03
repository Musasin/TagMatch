using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using DG.Tweening;

public class Kotonoha : BossAIBase
{
    public GameObject akaneFlame, aoiShot;
    BoxCollider2D bc;
    
    protected Vector2 leftPos1, leftPos2, leftPos3, leftPos4;
    protected Vector2 rightPos1, rightPos2, rightPos3, rightPos4;
    
    public enum ActionState
    {
        IDLE,
        MOVE_TO_LEFT_1,
        MOVE_TO_RIGHT_1,
        MOVE_TO_LEFT_2,
        MOVE_TO_RIGHT_2,
        MOVE_TO_LEFT_3,
        MOVE_TO_RIGHT_3,
        MOVE_TO_LEFT_4,
        MOVE_TO_RIGHT_4,
        AKANE_FLAME,
        AOI_SHOT,
        WAIT,
    }
    protected ActionState state;
    protected List<ActionState> actionStateQueue = new List<ActionState>();

    // Start is called before the first frame update
    public virtual void Start()
    {
        bc = GetComponent<BoxCollider2D>();
        anim = GetComponentInChildren<Animator>();
        bossScript = GetComponent<Boss>();
        state = ActionState.IDLE;
        leftPos1 = GameObject.Find("LeftPos1").transform.position;
        leftPos2 = GameObject.Find("LeftPos2").transform.position;
        leftPos3 = GameObject.Find("LeftPos3").transform.position;
        leftPos4 = GameObject.Find("LeftPos4").transform.position;
        rightPos1 = GameObject.Find("RightPos1").transform.position;
        rightPos2 = GameObject.Find("RightPos2").transform.position;
        rightPos3 = GameObject.Find("RightPos3").transform.position;
        rightPos4 = GameObject.Find("RightPos4").transform.position;
    }

    public override void Reset()
    {
        base.Reset();
        anim.SetBool("isDisappear", false);
        anim.SetBool("isReady", false);
        state = ActionState.IDLE;
    }

    // Update is called once per frame
    public virtual void Update()
    {
        if (!bossScript.IsActive())
        {
            return;
        }

        transform.localScale = new Vector3(isRight ? 1 : -1, 1, 1);
        
        if (isDead)
        {
            return;
        }

        if (bossScript.IsDead())
        {
            isRight = false;
            isDead = true;
            anim.SetBool("isDisappear", false);
            anim.SetBool("isReady", false);
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
                SetSequence();
                break;
            case ActionState.MOVE_TO_LEFT_1:
                isPlaying = true;
                PlayWarpSequence(leftPos1, true);
                break;
            case ActionState.MOVE_TO_RIGHT_1:
                isPlaying = true;
                PlayWarpSequence(rightPos1, false);
                break;
            case ActionState.MOVE_TO_LEFT_2:
                isPlaying = true;
                PlayWarpSequence(leftPos2, true);
                break;
            case ActionState.MOVE_TO_RIGHT_2:
                isPlaying = true;
                PlayWarpSequence(rightPos2, false);
                break;
            case ActionState.MOVE_TO_LEFT_3:
                isPlaying = true;
                PlayWarpSequence(leftPos3, true);
                break;
            case ActionState.MOVE_TO_RIGHT_3:
                isPlaying = true;
                PlayWarpSequence(rightPos3, false);
                break;
            case ActionState.MOVE_TO_LEFT_4:
                isPlaying = true;
                PlayWarpSequence(leftPos4, true);
                break;
            case ActionState.MOVE_TO_RIGHT_4:
                isPlaying = true;
                PlayWarpSequence(rightPos4, false);
                break;
            case ActionState.AKANE_FLAME:
                isPlaying = true;
                if (IsLifeHalf())
                {
                    sequence = DOTween.Sequence()
                    .AppendCallback(() => { anim.SetBool("isReady", true); })
                    .AppendInterval(1.0f)
                    .AppendCallback(() =>
                    {
                        InstantiateAlaneFlame();
                    })
                    .AppendInterval(1.0f)
                    .AppendCallback(() =>
                    {
                        InstantiateAlaneFlame();
                    })
                    .AppendInterval(1.0f)
                    .AppendCallback(() => { anim.SetBool("isReady", false); })
                    .OnComplete(() => { isPlaying = false; })
                    .Play();
                }
                else
                {
                    sequence = DOTween.Sequence()
                    .AppendCallback(() => { anim.SetBool("isReady", true); })
                    .AppendInterval(1.0f)
                    .AppendCallback(() =>
                    {
                        InstantiateAlaneFlame();
                    })
                    .AppendInterval(2.0f)
                    .AppendCallback(() => { anim.SetBool("isReady", false); })
                    .OnComplete(() => { isPlaying = false; })
                    .Play();
                }
                
                break;
            case ActionState.AOI_SHOT:
                isPlaying = true;
                if (IsLifeHalf())
                {
                    sequence = DOTween.Sequence()
                        .AppendCallback(() => { anim.SetBool("isReady", true); })
                        .AppendInterval(1.0f)
                        .AppendCallback(() =>
                        {
                            InstantiateAoiShot();
                        })
                        .AppendInterval(1.0f)
                        .AppendCallback(() =>
                        {
                            InstantiateAoiShot();
                        })
                        .AppendInterval(1.0f)
                        .AppendCallback(() => { anim.SetBool("isReady", false); })
                        .OnComplete(() => { isPlaying = false; })
                        .Play();
                } else
                {
                    sequence = DOTween.Sequence()
                        .AppendCallback(() => { anim.SetBool("isReady", true); })
                        .AppendInterval(1.0f)
                        .AppendCallback(() => { 
                            InstantiateAoiShot();
                        })
                        .AppendInterval(2.0f)
                        .AppendCallback(() => { anim.SetBool("isReady", false); })
                        .OnComplete(() => { isPlaying = false; })
                        .Play();
                }
                break;

            case ActionState.WAIT:
                isPlaying = true;
                sequence = DOTween.Sequence()
                    .AppendInterval(3.0f)
                    .OnComplete(() => { isPlaying = false; })
                    .Play();
                break;
        }
        
        state = actionStateQueue[stateIndex];
        stateIndex++;
    }

    public virtual void  SetSequence()
    {
        // Override用
    }

    public virtual void PlayWarpSequence(Vector2 targetPos, bool movedIsRight) 
    {
        sequence = DOTween.Sequence()
            .AppendCallback(() => { 
                AudioManager.Instance.PlaySE("enemy_switch");
                anim.SetBool("isDisappear", true);
                bc.enabled = false;
            })
            .AppendInterval(IsLifeHalf() ? 0.8f : 1.6f)
            .AppendCallback(() => { 
                transform.position = targetPos;
                isRight = movedIsRight;
            })
            .AppendCallback(() => { 
                AudioManager.Instance.PlaySE("enemy_switch");
                anim.SetBool("isDisappear", false);
                bc.enabled = true;
            })
            .AppendInterval(IsLifeHalf() ? 0.8f : 1.6f)
            .OnComplete(() => { isPlaying = false; })
            .Play();
    }

    public virtual void InstantiateAlaneFlame()
    {
        AudioManager.Instance.PlaySE("akane_flame");

        GameObject bullet = Instantiate(akaneFlame, transform.parent);
        bullet.transform.localPosition = transform.localPosition;
        bullet.transform.localScale = new Vector3(isRight ? 1 : -1, 1, 1);
    }
    
    public virtual void InstantiateAoiShot()
    {
        AudioManager.Instance.PlaySE("buon");

        GameObject b1 = Instantiate(aoiShot, transform.parent);
        b1.transform.position = new Vector2(transform.position.x, transform.position.y + 2);
    }
    
    bool IsLifeHalf()
    {
        return (StaticValues.bossHP.Sum() < (StaticValues.bossMaxHP.Sum() / 2));
    }
}
