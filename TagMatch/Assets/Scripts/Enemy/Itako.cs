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
        START,
        MOVE_TO_UPPER_LEFT,
        MOVE_TO_UPPER_RIGHT,
        MOVE_TO_UPPER_CENTER,
        MOVE_TO_LOWER_LEFT,
        MOVE_TO_LOWER_RIGHT,
        MOVE_TO_LOWER_CENTER,
        CHANGE_IS_RIGHT,
        CHARGE,
        FLAME_SHOT,
        FLAME_TWIN_SHOT,
        GHOST_SHOT,
        LOOP,
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
        anim.SetBool("isDisappear", false);
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
            AudioManager.Instance.PlayExVoice("itako_dead");
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
                AudioManager.Instance.PlayExVoice("itako_start");

                // 開幕に幽霊攻撃
                actionStateQueue.Add(ActionState.CHARGE);
                actionStateQueue.Add(ActionState.GHOST_SHOT);

                // 左側で炎→幽霊
                actionStateQueue.Add(ActionState.MOVE_TO_UPPER_LEFT);
                actionStateQueue.Add(ActionState.FLAME_SHOT);
                actionStateQueue.Add(ActionState.CHARGE);
                actionStateQueue.Add(ActionState.GHOST_SHOT);

                // 下段どっちかで炎攻撃
                if (Random.Range(0, 1) < 0.5f)
                {
                    actionStateQueue.Add(ActionState.MOVE_TO_LOWER_RIGHT);
                } 
                else
                {
                    actionStateQueue.Add(ActionState.MOVE_TO_LOWER_LEFT);
                }
                actionStateQueue.Add(ActionState.FLAME_TWIN_SHOT);
                
                // 中央で幽霊攻撃
                actionStateQueue.Add(ActionState.MOVE_TO_UPPER_CENTER);
                actionStateQueue.Add(ActionState.GHOST_SHOT);

                // 下段中央で両方に炎攻撃
                actionStateQueue.Add(ActionState.MOVE_TO_LOWER_CENTER);
                actionStateQueue.Add(ActionState.FLAME_SHOT);
                actionStateQueue.Add(ActionState.CHANGE_IS_RIGHT);
                actionStateQueue.Add(ActionState.CHARGE);
                actionStateQueue.Add(ActionState.FLAME_SHOT);

                
                // 上段どっちかで幽霊→炎攻撃
                if (Random.Range(0, 1) < 0.5f)
                {
                    actionStateQueue.Add(ActionState.MOVE_TO_UPPER_RIGHT);
                } 
                else
                {
                    actionStateQueue.Add(ActionState.MOVE_TO_UPPER_LEFT);
                }
                actionStateQueue.Add(ActionState.GHOST_SHOT);
                actionStateQueue.Add(ActionState.CHARGE);
                actionStateQueue.Add(ActionState.FLAME_SHOT);

                
                // 下段どっちかで幽霊攻撃
                if (Random.Range(0, 1) < 0.5f)
                {
                    actionStateQueue.Add(ActionState.MOVE_TO_LOWER_RIGHT);
                } 
                else
                {
                    actionStateQueue.Add(ActionState.MOVE_TO_LOWER_LEFT);
                }
                actionStateQueue.Add(ActionState.GHOST_SHOT);

                // 中央で炎攻撃2回
                actionStateQueue.Add(ActionState.MOVE_TO_UPPER_CENTER);
                actionStateQueue.Add(ActionState.FLAME_TWIN_SHOT);
                actionStateQueue.Add(ActionState.CHANGE_IS_RIGHT);
                actionStateQueue.Add(ActionState.CHARGE);
                actionStateQueue.Add(ActionState.FLAME_TWIN_SHOT);
                
                // 下段中央で幽霊攻撃2回
                actionStateQueue.Add(ActionState.MOVE_TO_LOWER_CENTER);
                actionStateQueue.Add(ActionState.GHOST_SHOT);
                actionStateQueue.Add(ActionState.CHANGE_IS_RIGHT);
                actionStateQueue.Add(ActionState.CHARGE);
                actionStateQueue.Add(ActionState.GHOST_SHOT);
                
                // 右上に戻って最初から
                actionStateQueue.Add(ActionState.MOVE_TO_LOWER_RIGHT);
                actionStateQueue.Add(ActionState.LOOP);

                break;
            case ActionState.LOOP:
                isPlaying = false;
                stateIndex = 0;
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
            case ActionState.CHANGE_IS_RIGHT:
                isRight = !isRight;
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
                AudioManager.Instance.PlayExVoice("itako_fire");
                anim.SetBool("isAttack", true);
                sequence = DOTween.Sequence()
                    .AppendInterval(0.5f)
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
                AudioManager.Instance.PlayExVoice("itako_fire");
                anim.SetBool("isAttack", true);
                sequence = DOTween.Sequence()
                    .AppendInterval(0.5f)
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
                AudioManager.Instance.PlayExVoice("itako_ghost");
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
        AudioManager.Instance.PlaySE("ghost_sound");

        GameObject bullet = Instantiate(ghostRotationBullet, transform.parent);
        bullet.transform.position = transform.position;
        bullet.transform.localScale = new Vector2(isRight ? -1.0f : 1.0f, 1.0f);

        // HP半分以下だと2個出す
        if (IsLifeHalf())
        {
            GameObject bullet2 = Instantiate(ghostRotationBullet, transform.parent);
            bullet2.transform.position = transform.position;
            bullet2.transform.localScale = new Vector2(isRight ? 1.0f : -1.0f, 1.0f);
        }
    }

    bool IsLifeHalf()
    {
        return (StaticValues.bossHP.Sum() < (StaticValues.bossMaxHP.Sum() / 2));
    }
}
