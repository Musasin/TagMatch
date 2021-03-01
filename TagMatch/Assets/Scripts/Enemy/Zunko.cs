using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using DG.Tweening;

public class Zunko : BossAIBase
{
    public GameObject zundaArrowBullet;
    Transform arrowPosTransform;
    GameObject holdingArrow1;
    GameObject holdingArrow2;
    GameObject holdingArrow3;
    GameObject holdingArrow4;

    enum ActionState
    {
        START,
        READY,
        INSTANTIATE_ARROW_1,
        INSTANTIATE_ARROW_2,
        INSTANTIATE_ARROW_3,
        INSTANTIATE_ARROW_4,
        ATTACK,
        ARROW_SHOT_1_1,
        ARROW_SHOT_1_2,
        ARROW_SHOT_1_3,
        ARROW_SHOT_2_1,
        ARROW_SHOT_2_2,
        ARROW_SHOT_2_3,
        ARROW_SHOT_3_1,
        ARROW_SHOT_3_2,
        ARROW_SHOT_3_3,
        ARROW_SHOT_4_1,
        ARROW_SHOT_4_2,
        LOOP,
    }
    ActionState state;
    List<ActionState> actionStateQueue = new List<ActionState>();


    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponentInChildren<Animator>();
        bossScript = GetComponent<Boss>();
        state = ActionState.START;
        arrowPosTransform = transform.Find("ArrowPos");
        arrowPosTransform = GameObject.Find("ArrowPos").transform;
    }
    
    public override void Reset()
    {
        base.Reset();
        anim.SetBool("isReady", false);
        anim.SetBool("isAttack", false);
        state = ActionState.START;
    }

    // Update is called once per frame
    void Update()
    {
        transform.localScale = new Vector2(isRight ? 1 : -1, 1);

        if (!isDead && bossScript.IsDead())
        {
            AudioManager.Instance.PlayExVoice("Zunko_dead");
            sequence.Kill();
            isRight = false;
            anim.SetBool("isReady", false);
            anim.SetBool("isAttack", false);
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
                actionStateQueue.Add(ActionState.INSTANTIATE_ARROW_1);
                actionStateQueue.Add(ActionState.INSTANTIATE_ARROW_2);
                actionStateQueue.Add(ActionState.INSTANTIATE_ARROW_3);
                actionStateQueue.Add(ActionState.INSTANTIATE_ARROW_4);
                actionStateQueue.Add(ActionState.READY);
                actionStateQueue.Add(ActionState.ARROW_SHOT_1_1);
                actionStateQueue.Add(ActionState.ARROW_SHOT_2_1);
                actionStateQueue.Add(ActionState.ARROW_SHOT_3_1);
                actionStateQueue.Add(ActionState.ARROW_SHOT_4_1);
                actionStateQueue.Add(ActionState.ATTACK);
                
                actionStateQueue.Add(ActionState.INSTANTIATE_ARROW_1);
                actionStateQueue.Add(ActionState.INSTANTIATE_ARROW_2);
                actionStateQueue.Add(ActionState.INSTANTIATE_ARROW_3);
                actionStateQueue.Add(ActionState.INSTANTIATE_ARROW_4);
                actionStateQueue.Add(ActionState.READY);
                actionStateQueue.Add(ActionState.ARROW_SHOT_1_2);
                actionStateQueue.Add(ActionState.ARROW_SHOT_2_2);
                actionStateQueue.Add(ActionState.ARROW_SHOT_3_2);
                actionStateQueue.Add(ActionState.ARROW_SHOT_4_2);
                actionStateQueue.Add(ActionState.ATTACK);
                
                actionStateQueue.Add(ActionState.INSTANTIATE_ARROW_1);
                actionStateQueue.Add(ActionState.READY);
                actionStateQueue.Add(ActionState.ARROW_SHOT_1_3);
                actionStateQueue.Add(ActionState.ATTACK);

                actionStateQueue.Add(ActionState.LOOP);
                break;
            case ActionState.LOOP:
                isPlaying = false;
                stateIndex = 0;
                break;
            case ActionState.READY:
                PlayReadySequence();
                break;
            case ActionState.ATTACK:
                PlayAttackSequence();
                break;
            case ActionState.INSTANTIATE_ARROW_1:
                holdingArrow1 = InstantiateArrow(0);
                break;
            case ActionState.INSTANTIATE_ARROW_2:
                holdingArrow2 = InstantiateArrow(15);
                break;
            case ActionState.INSTANTIATE_ARROW_3:
                holdingArrow3 = InstantiateArrow(30);
                break;
            case ActionState.INSTANTIATE_ARROW_4:
                holdingArrow4 = InstantiateArrow(45);
                break;
            case ActionState.ARROW_SHOT_1_1:
                ShotArrow(holdingArrow1, new Vector2(-700, 0));
                break;
            case ActionState.ARROW_SHOT_1_2:
                ShotArrow(holdingArrow1, new Vector2(-800, 0));
                break;
            case ActionState.ARROW_SHOT_1_3:
                ShotArrow(holdingArrow1, new Vector2(-900, 0));
                break;
            case ActionState.ARROW_SHOT_2_1:
                ShotArrow(holdingArrow2, new Vector2(-600, 60));
                break;
            case ActionState.ARROW_SHOT_2_2:
                ShotArrow(holdingArrow2, new Vector2(-700, 70));
                break;
            case ActionState.ARROW_SHOT_3_1:
                ShotArrow(holdingArrow3, new Vector2(-500, 150));
                break;
            case ActionState.ARROW_SHOT_3_2:
                ShotArrow(holdingArrow3, new Vector2(-600, 165));
                break;
            case ActionState.ARROW_SHOT_4_1:
                ShotArrow(holdingArrow4, new Vector2(-200, 200));
                break;
            case ActionState.ARROW_SHOT_4_2:
                ShotArrow(holdingArrow4, new Vector2(-220, 220));
                break;
        }
        state = actionStateQueue[stateIndex];
        stateIndex++;
    }
    
    void PlayReadySequence()
    {
        isPlaying = true;
        anim.SetBool("isReady", true);
        sequence = DOTween.Sequence()
            .AppendInterval(1.5f)
            .OnComplete(() =>
            {
                isPlaying = false;
            })
            .Play();
    }
    void PlayAttackSequence()
    {
        isPlaying = true;
        anim.SetBool("isAttack", true);
        sequence = DOTween.Sequence()
            .AppendInterval(1.5f)
            .OnComplete(() =>
            {
                anim.SetBool("isAttack", false);
                isPlaying = false;
            })
            .Play();
    }
    
    GameObject InstantiateArrow(float rotateZ)
    {
        GameObject holdingArrow = Instantiate(zundaArrowBullet, arrowPosTransform);
        holdingArrow.transform.localPosition = Vector2.zero;
        holdingArrow.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, rotateZ));
        holdingArrow.transform.localScale = new Vector2(1, 1);
        return holdingArrow;
    }

    void ShotArrow(GameObject arrow, Vector2 force)
    {
        arrow.GetComponent<RotateWithDirection>().SetIsActive(true);
        arrow.transform.SetParent(transform.parent);
        Rigidbody2D rb = arrow.GetComponent<Rigidbody2D>();
        rb.constraints = RigidbodyConstraints2D.None;
        rb.AddForce(force);
    }


    bool IsLifeHalf()
    {
        return (StaticValues.bossHP.Sum() < (StaticValues.bossMaxHP.Sum() / 2));
    }
}
