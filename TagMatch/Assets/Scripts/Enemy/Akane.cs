using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Akane : MonoBehaviour
{
    Animator anim;
    Boss bossScript;

    Vector2 leftPos1, leftPos2, leftPos3, leftPos4;
    Vector2 rightPos1, rightPos2, rightPos3, rightPos4;
    int stateIndex;
    bool isPlaying = false;
    bool isRight = false;
    bool isDead;
    
    Sequence sequence;

    enum ActionState
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
        STAND_SHOT
    }
    ActionState state;
    List<ActionState> actionStateQueue = new List<ActionState>();


    // Start is called before the first frame update
    void Start()
    {
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
            anim.SetBool("isDisappear", false);
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
                actionStateQueue.Add(ActionState.MOVE_TO_LEFT_1);
                actionStateQueue.Add(ActionState.MOVE_TO_RIGHT_2);
                actionStateQueue.Add(ActionState.MOVE_TO_LEFT_3);
                actionStateQueue.Add(ActionState.MOVE_TO_RIGHT_4);
                actionStateQueue.Add(ActionState.MOVE_TO_LEFT_2);
                actionStateQueue.Add(ActionState.MOVE_TO_RIGHT_3);
                actionStateQueue.Add(ActionState.MOVE_TO_LEFT_4);
                actionStateQueue.Add(ActionState.MOVE_TO_RIGHT_1);
                actionStateQueue.Add(ActionState.IDLE);
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
        }
        
        state = actionStateQueue[stateIndex];
        stateIndex++;
    }

    void PlayWarpSequence(Vector2 targetPos, bool movedIsRight) 
    {
        sequence = DOTween.Sequence()
            .AppendCallback(() => { anim.SetBool("isDisappear", true); })
            .AppendInterval(1.0f)
            .AppendCallback(() => { 
                transform.position = targetPos;
                isRight = movedIsRight;
            })
            .AppendCallback(() => { anim.SetBool("isDisappear", false); })
            .AppendInterval(1.0f)
            .OnComplete(() => { isPlaying = false; })
            .Play();
    }
}
