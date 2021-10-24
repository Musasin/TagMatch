using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using DG.Tweening;

public class BossYukari : BossAIBase
{
    public GameObject starBullet;
    BoxCollider2D bc;
    Vector2 l1Pos, l2Pos, l3Pos, l4Pos, r1Pos, r2Pos, r3Pos, r4Pos, c1Pos;

    enum ActionState
    {
        START,
        MOVE_TO_L1,
        MOVE_TO_L2,
        MOVE_TO_L3,
        MOVE_TO_L4,
        MOVE_TO_R1,
        MOVE_TO_R2,
        MOVE_TO_R3,
        MOVE_TO_R4,
        JUMP_TO_C1,
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
        
        l1Pos = GameObject.Find("L1Pos").transform.position;
        l2Pos = GameObject.Find("L2Pos").transform.position;
        l3Pos = GameObject.Find("L3Pos").transform.position;
        l4Pos = GameObject.Find("L4Pos").transform.position;
        r1Pos = GameObject.Find("R1Pos").transform.position;
        r2Pos = GameObject.Find("R2Pos").transform.position;
        r3Pos = GameObject.Find("R3Pos").transform.position;
        r4Pos = GameObject.Find("R4Pos").transform.position;
        c1Pos = GameObject.Find("C1Pos").transform.position;
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

        
        // アニメーション再生中は次のモードに遷移しない
        if (isPlaying)
        {
            return;
        }

        switch (state)
        {
            case ActionState.START:
                //AudioManager.Instance.PlayExVoice("itako_start");

                actionStateQueue.Add(ActionState.WAIT);
                
                actionStateQueue.Add(ActionState.JUMP_TO_C1);
                actionStateQueue.Add(ActionState.MOVE_TO_L1);
                actionStateQueue.Add(ActionState.MOVE_TO_L2);
                actionStateQueue.Add(ActionState.MOVE_TO_L3);
                actionStateQueue.Add(ActionState.MOVE_TO_L4);
                actionStateQueue.Add(ActionState.MOVE_TO_R1);
                actionStateQueue.Add(ActionState.MOVE_TO_R2);
                actionStateQueue.Add(ActionState.MOVE_TO_R3);
                actionStateQueue.Add(ActionState.MOVE_TO_R4);

                actionStateQueue.Add(ActionState.LOOP);

                break;
            case ActionState.LOOP:
                isPlaying = false;
                stateIndex = 0;
                break;
            case ActionState.MOVE_TO_L1:
                isPlaying = true;
                PlayWarpSequence(l1Pos, true);
                break;
            case ActionState.MOVE_TO_L2:
                isPlaying = true;
                PlayWarpSequence(l2Pos, true);
                break;
            case ActionState.MOVE_TO_L3:
                isPlaying = true;
                PlayWarpSequence(l3Pos, true);
                break;
            case ActionState.MOVE_TO_L4:
                isPlaying = true;
                PlayWarpSequence(l4Pos, true);
                break;
            case ActionState.MOVE_TO_R1:
                isPlaying = true;
                PlayWarpSequence(r1Pos, false);
                break;
            case ActionState.MOVE_TO_R2:
                isPlaying = true;
                PlayWarpSequence(r2Pos, false);
                break;
            case ActionState.MOVE_TO_R3:
                isPlaying = true;
                PlayWarpSequence(r3Pos, false);
                break;
            case ActionState.MOVE_TO_R4:
                isPlaying = true;
                PlayWarpSequence(r4Pos, false);
                break;
            case ActionState.JUMP_TO_C1:
                isPlaying = true;
                PlayJumpSequence(c1Pos);
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

    public virtual void PlayWarpSequence(Vector2 targetPos, bool movedIsRight) 
    {
        bool isNowRightPos = c1Pos.x < transform.position.x;

        sequence = DOTween.Sequence()
            .AppendCallback(() => {
                AudioManager.Instance.PlaySE("buon"); // TODO: ドロン系の音にしたい かくれるが更新されたらそれを当ててみる
                bc.enabled = false;
                animationState = AnimationState.SQUAT;
                anim.SetInteger("state", (int)animationState);
            })
            .Append(transform.DOLocalMoveX(transform.localPosition.x + (isNowRightPos ? 2.0f : -2.0f), 0.3f))
            .AppendInterval(IsLifeHalf() ? 0.5f : 1.0f)
            .AppendCallback(() => {
                transform.position = targetPos;
                isRight = movedIsRight;
            })
            .Append(transform.DOLocalMoveX(targetPos.x + (movedIsRight ? 2.0f : -2.0f), 0.3f))
            .AppendCallback(() => {
                bc.enabled = true;
            })
            .AppendInterval(2.0f)
            .OnComplete(() => { isPlaying = false; })
            .Play();
    }

    public void PlayJumpSequence(Vector2 targetPos)
    {
        bool isNowRightPos = c1Pos.x < transform.position.x;

        // 飛びながら回転するだけ
        sequence = DOTween.Sequence()
            .Append(transform.DOLocalJump(targetPos, 1.0f, 1, 1.0f))
            .Join(transform.DORotate(new Vector3(1, 1, isNowRightPos ? 720 : -720), 1.0f, RotateMode.WorldAxisAdd))
            .OnComplete(() => { isPlaying = false; })
            .Play();

        // ジャンプショットの移植中
        //Sequence sequence = DOTween.Sequence()
        //    .Append(transform.DOLocalRotate(new Vector3(0, 0, -45), 0.4f / 8))
        //    .AppendCallback(() => { InstantiateBullet(Bullet.BulletType.YUKARI, starBullet, isRight ? -45 : -135); })
        //    .AppendInterval(0.4f / 8)
        //    .Append(transform.DOLocalRotate(new Vector3(0, 0, -60), 0.4f / 8))
        //    .AppendCallback(() => { InstantiateBullet(Bullet.BulletType.YUKARI, starBullet, isRight ? -80 : -100); })
        //    .AppendInterval(0.4f / 8)
        //    .AppendCallback(() => { InstantiateBullet(Bullet.BulletType.YUKARI, starBullet, isRight ? -120 : -60); })
        //    .Append(transform.DOLocalRotate(new Vector3(0, 0, -360), 0.4f * 4 / 8, RotateMode.FastBeyond360));
        //sequence.Play();
        //AudioManager.Instance.PlayExVoice("yukari_down_shot", true);


    }
        
    // ジャンプショットの移植中
    //private void InstantiateBullet(Bullet.BulletType bulletType, GameObject bulletObj, float angleZ, bool isSquat = false)
    //{
    //    float addforceX = Mathf.Cos(angleZ * Mathf.Deg2Rad) * 400.0f * (bulletType == Bullet.BulletType.YUKARI ? 1.5f : 1.0f);
    //    float addforceY = Mathf.Sin(angleZ * Mathf.Deg2Rad) * 400.0f * (bulletType == Bullet.BulletType.YUKARI ? 1.5f : 1.0f);

    //    GameObject bullet = Instantiate(bulletObj);
        
    //    bullet.transform.position = isSquat ? squatBulletPivot.transform.position : bulletPivot.transform.position;
    //    bullet.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angleZ));
    //    bullet.GetComponent<Rigidbody2D>().AddForce(new Vector2(addforceX, addforceY));
    //    bullet.GetComponent<Bullet>().SetBulletType(bulletType);
    //    bullet.GetComponent<Bullet>().SetPlayerScript(this);
    //    AddBulletCount(bulletType, 1);

    //    if (IsYukari())
    //    {
    //        AudioManager.Instance.PlaySE("shot_yukari");
    //    } else if (IsMaki())
    //    {
    //        AudioManager.Instance.PlaySE("shot_maki");
    //    }
    //}

    bool IsLifeHalf()
    {
        return (StaticValues.bossHP.Sum() < (StaticValues.bossMaxHP.Sum() / 2));
    }
}
