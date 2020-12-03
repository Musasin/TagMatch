using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Kiritan : MonoBehaviour
{
    public GameObject kiritanhouBullet;
    Animator anim;
    Boss bossScript;
    GameObject kiritanhouPos1, kiritanhouPos2;
    SpriteRenderer jetFlameSR1, jetFlameSR2;
    Vector2 upperLeftPos, upperRightPos, lowerLeftPos, lowerRightPos;
    int stateIndex;
    bool isActive = true;
    bool isPlaying = false;
    bool isRight = false;
    bool isDead;

    Sequence sequence;

    enum ActionState
    {
        IDLE,
        MOVE_TO_UPPER_LEFT,
        MOVE_TO_UPPER_RIGHT,
        MOVE_TO_LOWER_LEFT,
        MOVE_TO_LOWER_RIGHT,
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
        upperLeftPos = GameObject.Find("UpperLeftPos").transform.position;
        upperRightPos = GameObject.Find("UpperRightPos").transform.position;
        lowerLeftPos = GameObject.Find("LowerLeftPos").transform.position;
        lowerRightPos = GameObject.Find("LowerRightPos").transform.position;
        jetFlameSR1 = GameObject.Find("JetFlame1").GetComponent<SpriteRenderer>();
        jetFlameSR2 = GameObject.Find("JetFlame2").GetComponent<SpriteRenderer>();   
        kiritanhouPos1 = GameObject.Find("KiritanhouPos1");
        kiritanhouPos2 = GameObject.Find("KiritanhouPos2");   
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(kiritanhouPos1.transform.position);
        if (!isActive)
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
            JetOff();
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
                actionStateQueue.Add(ActionState.STAND_SHOT);
                actionStateQueue.Add(ActionState.MOVE_TO_LOWER_LEFT);
                actionStateQueue.Add(ActionState.STAND_SHOT);
                actionStateQueue.Add(ActionState.MOVE_TO_LOWER_RIGHT);
                actionStateQueue.Add(ActionState.STAND_SHOT);
                actionStateQueue.Add(ActionState.MOVE_TO_UPPER_RIGHT);
                actionStateQueue.Add(ActionState.STAND_SHOT);
                actionStateQueue.Add(ActionState.IDLE);
                break;
            case ActionState.MOVE_TO_UPPER_LEFT:
                isRight = false;
                isPlaying = true;
                anim.SetBool("isFloat", true);
                JetOn();
                sequence = transform.DOLocalJump(upperLeftPos, 0.3f, 1, 3.0f).OnComplete(() => { 
                    isPlaying = false;
                    isRight = true;
                    JetOff();
                });
                break;
            case ActionState.MOVE_TO_UPPER_RIGHT:
                isRight = true;
                isPlaying = true;
                anim.SetBool("isFloat", true);
                JetOn();
                sequence = transform.DOLocalJump(upperRightPos, 0.3f, 1, 3.0f).OnComplete(() => { 
                    isPlaying = false;
                    isRight = false;
                    JetOff();
                });
                break;
            case ActionState.MOVE_TO_LOWER_LEFT:
                isRight = false;
                isPlaying = true;
                anim.SetBool("isFloat", true);
                JetOn();
                sequence = transform.DOLocalJump(lowerLeftPos, 0.3f, 1, 3.0f).OnComplete(() => { 
                    isPlaying = false;
                    isRight = true;
                    JetOff();
                });
                break;
            case ActionState.MOVE_TO_LOWER_RIGHT:
                isRight = true;
                isPlaying = true;
                anim.SetBool("isFloat", true);
                JetOn();
                sequence = transform.DOLocalJump(lowerRightPos, 0.3f, 1, 3.0f).OnComplete(() => { 
                    isPlaying = false;
                    isRight = false;
                    JetOff();
                });
                break;
            case ActionState.STAND_SHOT:
                isPlaying = true;
                anim.SetBool("isReady", true);
                anim.SetBool("isFloat", false);

                sequence = DOTween.Sequence()
                    .AppendInterval(0.2f)
                    .AppendCallback(() =>
                    {
                        InstantiateTwinBullet();
                    })
                    .AppendInterval(2.0f)
                    .OnComplete(() => { 
                        anim.SetBool("isReady", false);
                        isPlaying = false;
                    })
                    .Play();
                break;

        }
        state = actionStateQueue[stateIndex];
        stateIndex++;
    }

    void JetOn()
    {
        DOTween.ToAlpha(() => jetFlameSR1.color, color => jetFlameSR1.color = color, 1.0f, 0.3f);
        DOTween.ToAlpha(() => jetFlameSR2.color, color => jetFlameSR2.color = color, 1.0f, 0.3f);
    }

    void JetOff()
    {
        DOTween.ToAlpha(() => jetFlameSR1.color, color => jetFlameSR1.color = color, 0f, 0.3f);
        DOTween.ToAlpha(() => jetFlameSR2.color, color => jetFlameSR2.color = color, 0f, 0.3f);
    }

    void InstantiateTwinBullet()
    {
        GameObject b1 = Instantiate(kiritanhouBullet);
        b1.transform.position = kiritanhouPos1.transform.position;
        if (isRight)
        {
            b1.transform.localScale = new Vector2(-1, 1);
        }

        GameObject b2 = Instantiate(kiritanhouBullet);
        b2.transform.position = kiritanhouPos2.transform.position;
        if (isRight)
        {
            b2.transform.localScale = new Vector2(-1, 1);
        }
    }
}
