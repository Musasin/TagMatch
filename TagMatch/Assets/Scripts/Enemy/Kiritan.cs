using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Kiritan : BossAIBase
{
    public GameObject kiritanhouBullet;
    public bool isSecondBattle;
    GameObject kiritanhouPos1, kiritanhouPos2;
    SpriteRenderer jetFlameSR1, jetFlameSR2;
    Vector2 upperLeftPos, upperRightPos, lowerLeftPos, lowerRightPos;

    enum ActionState
    {
        START,
        MOVE_TO_UPPER_LEFT,
        MOVE_TO_UPPER_RIGHT,
        MOVE_TO_LOWER_LEFT,
        MOVE_TO_LOWER_RIGHT,
        STAND_SHOT,
        LOOP
    }
    ActionState state;
    List<ActionState> actionStateQueue = new List<ActionState>();


    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponentInChildren<Animator>();
        bossScript = GetComponent<Boss>();
        state = ActionState.START;
        upperLeftPos = GameObject.Find("UpperLeftPos").transform.position;
        upperRightPos = GameObject.Find("UpperRightPos").transform.position;
        lowerLeftPos = GameObject.Find("LowerLeftPos").transform.position;
        lowerRightPos = GameObject.Find("LowerRightPos").transform.position;
        jetFlameSR1 = GameObject.Find("JetFlame1").GetComponent<SpriteRenderer>();
        jetFlameSR2 = GameObject.Find("JetFlame2").GetComponent<SpriteRenderer>();   
        kiritanhouPos1 = GameObject.Find("KiritanhouPos1");
        kiritanhouPos2 = GameObject.Find("KiritanhouPos2");   
    }
    
    public override void Reset()
    {
        base.Reset();
        anim.SetBool("isFloat", false);
        anim.SetBool("isReady", false);
        actionStateQueue.Clear();
        stateIndex = 0;
        state = ActionState.START;
    }

    // Update is called once per frame
    void Update()
    {
        transform.localScale = new Vector2(isRight ? 1 : -1, 1);
        
        if (!isDead && bossScript.IsDead())
        {
            AudioManager.Instance.PlayExVoice("kiritan_dead");
            sequence.Kill();
            isRight = false;
            anim.SetBool("isFloat", false);
            anim.SetBool("isReady", false);
            isDead = true;
            JetOff();
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
                AudioManager.Instance.PlayExVoice("kiritan_start");
                
                isPlaying = true;
                sequence = DOTween.Sequence()
                        .AppendInterval(1.0f)
                        .OnComplete(() =>
                        {
                            isPlaying = false;
                        })
                        .Play();

                actionStateQueue.Add(ActionState.STAND_SHOT);
                actionStateQueue.Add(ActionState.MOVE_TO_UPPER_LEFT);
                actionStateQueue.Add(ActionState.STAND_SHOT);
                actionStateQueue.Add(ActionState.MOVE_TO_LOWER_LEFT);
                actionStateQueue.Add(ActionState.STAND_SHOT);
                actionStateQueue.Add(ActionState.MOVE_TO_LOWER_RIGHT);
                actionStateQueue.Add(ActionState.STAND_SHOT);
                actionStateQueue.Add(ActionState.MOVE_TO_UPPER_RIGHT);
                actionStateQueue.Add(ActionState.LOOP);
                break;
            case ActionState.LOOP:
                isPlaying = false;
                stateIndex = 0;
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
                isRight = false;
                isPlaying = true;
                anim.SetBool("isFloat", true);
                JetOn();
                sequence = transform.DOLocalJump(upperRightPos, 0.3f, 1, 1.0f).OnComplete(() => { 
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
                sequence = transform.DOLocalJump(lowerLeftPos, 0.3f, 1, 1.0f).OnComplete(() => { 
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
                
                if (bossScript.hp <= 50)
                {
                    AudioManager.Instance.PlayExVoice("kiritan_attack3");
                    sequence = DOTween.Sequence()
                        .AppendInterval(0.2f)
                        .AppendCallback(() => { InstantiateTwinBullet(); })
                        .AppendInterval(1.5f)
                        .AppendCallback(() => { InstantiateTwinBullet(); })
                        .AppendInterval(1.5f)
                        .AppendCallback(() => { InstantiateTwinBullet(); })
                        .AppendInterval(1.5f)
                        .AppendCallback(() => { InstantiateTwinBullet(); })
                        .AppendInterval(1.5f)
                        .OnComplete(() =>
                        {
                            anim.SetBool("isReady", false);
                            isPlaying = false;
                        })
                        .Play();
                }
                else if (bossScript.hp <= 100)
                {
                    AudioManager.Instance.PlayExVoice("kiritan_attack2");
                    sequence = DOTween.Sequence()
                        .AppendInterval(0.2f)
                        .AppendCallback(() => { InstantiateTwinBullet(); })
                        .AppendInterval(1.7f)
                        .AppendCallback(() => { InstantiateTwinBullet(); })
                        .AppendInterval(1.7f)
                        .OnComplete(() =>
                        {
                            anim.SetBool("isReady", false);
                            isPlaying = false;
                        })
                        .Play();
                }
                else
                {
                    AudioManager.Instance.PlayExVoice("kiritan_attack1");
                    sequence = DOTween.Sequence()
                        .AppendInterval(0.2f)
                        .AppendCallback(() => { InstantiateTwinBullet(); })
                        .AppendInterval(2.0f)
                        .OnComplete(() => { 
                            anim.SetBool("isReady", false);
                            isPlaying = false;
                        })
                        .Play();
                }
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
        AudioManager.Instance.PlaySE("buon");

        GameObject b1 = Instantiate(kiritanhouBullet, transform.parent);
        b1.transform.position = kiritanhouPos1.transform.position;
        if (isRight)
        {
            b1.transform.localScale = new Vector2(-1, 1);
        }

        GameObject b2 = Instantiate(kiritanhouBullet, transform.parent);
        b2.transform.position = kiritanhouPos2.transform.position;
        if (isRight)
        {
            b2.transform.localScale = new Vector2(-1, 1);
        }
    }
}
