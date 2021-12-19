using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using DG.Tweening;

public class BossYukari : BossAIBase
{
    public bool isAstral;
    public GameObject starBullet;
    BoxCollider2D bc;
    GameObject bulletPivot, squatBulletPivot;
    Vector2 l1Pos, l2Pos, l3Pos, l4Pos, r1Pos, r2Pos, r3Pos, r4Pos, c1Pos, c4Pos;

    enum ActionState
    {
        START,
        SHOT,
        MOVE_TO_L1,
        MOVE_TO_L2,
        MOVE_TO_L3,
        MOVE_TO_L4,
        MOVE_TO_R1,
        MOVE_TO_R2,
        MOVE_TO_R3,
        MOVE_TO_R4,
        JUMP_TO_C1,
        JUMP_TO_L1,
        JUMP_TO_R1,
        RUN_TO_C4,
        RUN_TO_L4,
        RUN_TO_R4,
        WARP_TO_L4,
        WARP_TO_R1,
        WARP_TO_R4,
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
        squatBulletPivot = transform.Find("SquatBulletPivot").gameObject;
        l1Pos = GameObject.Find("L1Pos").transform.position;
        l2Pos = GameObject.Find("L2Pos").transform.position;
        l3Pos = GameObject.Find("L3Pos").transform.position;
        l4Pos = GameObject.Find("L4Pos").transform.position;
        r1Pos = GameObject.Find("R1Pos").transform.position;
        r2Pos = GameObject.Find("R2Pos").transform.position;
        r3Pos = GameObject.Find("R3Pos").transform.position;
        r4Pos = GameObject.Find("R4Pos").transform.position;
        c1Pos = GameObject.Find("C1Pos").transform.position;
        c4Pos = GameObject.Find("C4Pos").transform.position;
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
            AudioManager.Instance.PlayExVoice("yukari_dead", true);
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

        switch (state)
        {
            case ActionState.START:
                if (isAstral) bc.enabled = true;
                else AudioManager.Instance.PlayExVoice("yukari_start", true);

                actionStateQueue.Add(ActionState.WAIT);
                
                
                if (isAstral)
                {
                    // 分身体は開幕右下からのショットで登場
                    actionStateQueue.Add(ActionState.MOVE_TO_R4);
                    actionStateQueue.Add(ActionState.SHOT);
                } else
                {
                    // 開幕右上から中央上、左上にジャンプしながら下方向に攻撃
                    actionStateQueue.Add(ActionState.JUMP_TO_C1);
                    actionStateQueue.Add(ActionState.JUMP_TO_L1);
                }
                
                // Y軸に若干ランダムを入れつつ、左右にワープして攻撃
                actionStateQueue.Add(Random.Range(0, 1.0f) < 0.5f ? ActionState.MOVE_TO_R2 : ActionState.MOVE_TO_R3);
                actionStateQueue.Add(ActionState.SHOT);
                actionStateQueue.Add(Random.Range(0, 1.0f) < 0.5f ? ActionState.MOVE_TO_L3 : ActionState.MOVE_TO_L4);
                actionStateQueue.Add(ActionState.SHOT);
                actionStateQueue.Add(Random.Range(0, 1.0f) < 0.5f ? ActionState.MOVE_TO_R1 : ActionState.MOVE_TO_R4);
                actionStateQueue.Add(ActionState.SHOT);
                actionStateQueue.Add(Random.Range(0, 1.0f) < 0.5f ? ActionState.MOVE_TO_L1 : ActionState.MOVE_TO_L2);
                actionStateQueue.Add(ActionState.SHOT);

                // 左右どちらかの二段目から中央上->反対側の上に向かってジャンプ攻撃
                if (Random.Range(0, 1.0f) < 0.5f)
                {
                    actionStateQueue.Add(ActionState.MOVE_TO_R2);
                    actionStateQueue.Add(ActionState.SHOT);
                    actionStateQueue.Add(ActionState.JUMP_TO_C1);
                    actionStateQueue.Add(ActionState.JUMP_TO_L1);
                } else
                {
                    actionStateQueue.Add(ActionState.MOVE_TO_L2);
                    actionStateQueue.Add(ActionState.SHOT);
                    actionStateQueue.Add(ActionState.JUMP_TO_C1);
                    actionStateQueue.Add(ActionState.JUMP_TO_R1);
                }

                // ランダムでフェイントを挟む
                if (Random.Range(0, 1.0f) < 0.5f)
                {
                    actionStateQueue.Add(Random.Range(0, 1.0f) < 0.5f ? ActionState.MOVE_TO_R1 : ActionState.MOVE_TO_R4);
                    actionStateQueue.Add(Random.Range(0, 1.0f) < 0.5f ? ActionState.MOVE_TO_R2 : ActionState.MOVE_TO_R3);
                }

                // 左右どちらかから端まで走り抜ける
                if (Random.Range(0, 1.0f) < 0.5f)
                {
                    AddLRRunStateQueue();
                } else
                {
                    AddRLRunStateQueue();
                }
                
                // ランダムでフェイントを挟む
                if (Random.Range(0, 1.0f) < 0.5f)
                {
                    actionStateQueue.Add(Random.Range(0, 1.0f) < 0.5f ? ActionState.MOVE_TO_L1 : ActionState.MOVE_TO_L4);
                    actionStateQueue.Add(Random.Range(0, 1.0f) < 0.5f ? ActionState.MOVE_TO_L2 : ActionState.MOVE_TO_L3);
                }

                // 左上でショット、ジャンプして右端まで
                actionStateQueue.Add(ActionState.MOVE_TO_L1);
                actionStateQueue.Add(ActionState.SHOT);
                actionStateQueue.Add(ActionState.JUMP_TO_C1);
                actionStateQueue.Add(ActionState.JUMP_TO_R1);
                
                // すぐ右端からジャンプして中央に戻ってショット、そしてまた右に消える
                actionStateQueue.Add(ActionState.CHANGE_IS_RIGHT);
                actionStateQueue.Add(ActionState.JUMP_TO_C1);
                actionStateQueue.Add(ActionState.SHOT);
                actionStateQueue.Add(ActionState.CHANGE_IS_RIGHT);
                actionStateQueue.Add(ActionState.JUMP_TO_R1);

                // ランダムでフェイントを挟む
                if (Random.Range(0, 1.0f) < 0.5f)
                {
                    actionStateQueue.Add(Random.Range(0, 1.0f) < 0.5f ? ActionState.MOVE_TO_R4 : ActionState.MOVE_TO_L4);
                }
                
                // Y軸に若干ランダムを入れつつ、左右にワープして攻撃
                actionStateQueue.Add(Random.Range(0, 1.0f) < 0.5f ? ActionState.MOVE_TO_R2 : ActionState.MOVE_TO_R3);
                actionStateQueue.Add(ActionState.SHOT);
                actionStateQueue.Add(Random.Range(0, 1.0f) < 0.5f ? ActionState.MOVE_TO_L3 : ActionState.MOVE_TO_L4);
                actionStateQueue.Add(ActionState.SHOT);
                
                // ランダムでフェイントを挟む
                if (Random.Range(0, 1.0f) < 0.5f)
                {
                    actionStateQueue.Add(Random.Range(0, 1.0f) < 0.5f ? ActionState.MOVE_TO_R2 : ActionState.MOVE_TO_L2);
                }

                // 左から右、または左から左で走る
                if (Random.Range(0, 1.0f) < 0.5f)
                {
                    AddLRRunStateQueue();
                } else
                {
                    AddLLRunStateQueue();
                }
                
                // 左上、中央、左上
                actionStateQueue.Add(ActionState.MOVE_TO_L1);
                actionStateQueue.Add(ActionState.JUMP_TO_C1);
                actionStateQueue.Add(ActionState.CHANGE_IS_RIGHT);
                actionStateQueue.Add(ActionState.JUMP_TO_L1);
                
                // 右か左の縦連
                if (Random.Range(0, 1.0f) < 0.5f)
                {
                    actionStateQueue.Add(ActionState.MOVE_TO_R1);
                    actionStateQueue.Add(ActionState.MOVE_TO_R2);
                    actionStateQueue.Add(ActionState.SHOT);
                    actionStateQueue.Add(ActionState.MOVE_TO_R3);
                    actionStateQueue.Add(ActionState.MOVE_TO_R4);
                    actionStateQueue.Add(ActionState.SHOT);
                } else
                {
                    actionStateQueue.Add(ActionState.MOVE_TO_L1);
                    actionStateQueue.Add(ActionState.SHOT);
                    actionStateQueue.Add(ActionState.MOVE_TO_L2);
                    actionStateQueue.Add(ActionState.MOVE_TO_L3);
                    actionStateQueue.Add(ActionState.SHOT);
                    actionStateQueue.Add(ActionState.MOVE_TO_L4);
                }

                // 左右どちらかの二段目から中央上->反対側の上に向かってジャンプ攻撃
                if (Random.Range(0, 1.0f) < 0.5f)
                {
                    actionStateQueue.Add(ActionState.MOVE_TO_R2);
                    actionStateQueue.Add(ActionState.SHOT);
                    actionStateQueue.Add(ActionState.JUMP_TO_C1);
                    actionStateQueue.Add(ActionState.JUMP_TO_L1);
                } else
                {
                    actionStateQueue.Add(ActionState.MOVE_TO_L2);
                    actionStateQueue.Add(ActionState.SHOT);
                    actionStateQueue.Add(ActionState.JUMP_TO_C1);
                    actionStateQueue.Add(ActionState.JUMP_TO_R1);
                }
                
                // ランダムでフェイントを挟む
                if (Random.Range(0, 1.0f) < 0.5f)
                {
                    actionStateQueue.Add(Random.Range(0, 1.0f) < 0.5f ? ActionState.MOVE_TO_L1 : ActionState.MOVE_TO_L4);
                    actionStateQueue.Add(Random.Range(0, 1.0f) < 0.5f ? ActionState.MOVE_TO_L2 : ActionState.MOVE_TO_L3);
                }

                // 右側二段目でショットして最初に戻る
                actionStateQueue.Add(ActionState.MOVE_TO_R2);
                actionStateQueue.Add(ActionState.SHOT);
                

                actionStateQueue.Add(ActionState.LOOP);

                break;
            case ActionState.LOOP:
                isPlaying = false;
                stateIndex = 0;
                break;
            case ActionState.SHOT:
                isPlaying = true;
                PlayShotSequence();
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
            case ActionState.JUMP_TO_L1:
                isPlaying = true;
                PlayJumpSequence(l1Pos);
                break;
            case ActionState.JUMP_TO_R1:
                isPlaying = true;
                PlayJumpSequence(r1Pos);
                break;
            case ActionState.RUN_TO_C4:
                isPlaying = true;
                PlayRunSequence(c4Pos);
                break;
            case ActionState.RUN_TO_L4:
                isPlaying = true;
                PlayRunSequence(l4Pos);
                break;
            case ActionState.RUN_TO_R4:
                isPlaying = true;
                PlayRunSequence(r4Pos);
                break;
            case ActionState.WARP_TO_L4:
                isPlaying = true;
                PlayWarpSequenceOutOnly(l4Pos, true);
                break;
            case ActionState.WARP_TO_R1:
                isPlaying = true;
                PlayWarpSequenceOutOnly(r1Pos, false);
                break;
            case ActionState.WARP_TO_R4:
                isPlaying = true;
                PlayWarpSequenceOutOnly(r4Pos, false);
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

    
    private void AddLRRunStateQueue()
    {
        // 左下から右下へ走りながら攻撃
        actionStateQueue.Add(ActionState.WARP_TO_L4);
        actionStateQueue.Add(ActionState.RUN_TO_C4);
        actionStateQueue.Add(ActionState.WAIT);
        actionStateQueue.Add(ActionState.RUN_TO_R4);
    }
    private void AddRLRunStateQueue()
    {
        // 右下から左下へ走りながら攻撃
        actionStateQueue.Add(ActionState.WARP_TO_R4);
        actionStateQueue.Add(ActionState.RUN_TO_C4);
        actionStateQueue.Add(ActionState.WAIT);
        actionStateQueue.Add(ActionState.RUN_TO_L4);
    }
    private void AddLLRunStateQueue()
    {
        // 左下、中央、左下の順で走りながら攻撃
        actionStateQueue.Add(ActionState.WARP_TO_L4);
        actionStateQueue.Add(ActionState.RUN_TO_C4);
        actionStateQueue.Add(ActionState.WAIT);
        actionStateQueue.Add(ActionState.CHANGE_IS_RIGHT);
        actionStateQueue.Add(ActionState.RUN_TO_L4);
    }
    private void AddRRRunStateQueue()
    {
        // 右下、中央、右下の順で走りながら攻撃
        actionStateQueue.Add(ActionState.WARP_TO_R4);
        actionStateQueue.Add(ActionState.RUN_TO_C4);
        actionStateQueue.Add(ActionState.WAIT);
        actionStateQueue.Add(ActionState.CHANGE_IS_RIGHT);
        actionStateQueue.Add(ActionState.RUN_TO_R4);
    }

    private void PlayWarpSequence(Vector2 targetPos, bool movedIsRight) 
    {
        bool isNowRightPos = c1Pos.x < transform.position.x;
        Debug.Log(c1Pos);
        Debug.Log(transform.position.x);

        sequence = DOTween.Sequence()
            .AppendCallback(() => {
                AudioManager.Instance.PlaySE("buon");
                bc.enabled = false;
                animationState = AnimationState.SQUAT;
                anim.SetInteger("state", (int)animationState);
            })
            .Append(transform.DOMoveX(transform.position.x + (isNowRightPos ? 2.0f : -2.0f), 0.3f))
            .AppendInterval(IsLifeHalf() ? 0.5f : 1.0f)
            .AppendCallback(() => {
                transform.position = targetPos;
                isRight = movedIsRight;
            })
            .Append(transform.DOMoveX(targetPos.x + (movedIsRight ? 2.0f : -2.0f), 0.3f))
            .AppendCallback(() => {
                bc.enabled = true;
            })
            .AppendInterval(IsLifeHalf() ? 0.1f : 0.3f)
            .OnComplete(() => { isPlaying = false; })
            .Play();
    }
    
    // PlayWarpSequenceから出てくる部分をカットした版
    private void PlayWarpSequenceOutOnly(Vector2 targetPos, bool movedIsRight) 
    {
        bool isNowRightPos = c1Pos.x < transform.position.x;

        sequence = DOTween.Sequence()
            .AppendCallback(() => {
                AudioManager.Instance.PlaySE("buon");
                bc.enabled = false;
                animationState = AnimationState.SQUAT;
                anim.SetInteger("state", (int)animationState);
            })
            .Append(transform.DOMoveX(transform.position.x + (isNowRightPos ? 2.0f : -2.0f), 0.3f))
            .AppendInterval(IsLifeHalf() ? 0.5f : 1.0f)
            .AppendCallback(() => {
                transform.position = targetPos;
                isRight = movedIsRight;
                bc.enabled = true;
            })
            .OnComplete(() => { isPlaying = false; })
            .Play();
    }

    private void PlayShotSequence()
    {
        sequence = DOTween.Sequence()
            .AppendCallback(() =>
            {
                InstantiateBullet(starBullet, isRight ? 0 : 180, animationState == AnimationState.SQUAT);
            })
            .AppendInterval(0.2f)
            .AppendCallback(() =>
            {
                if(IsLifeHalf()) InstantiateBullet(starBullet, isRight ? 0 : 180, animationState == AnimationState.SQUAT);
            })
            .AppendInterval(0.2f)
            .AppendCallback(() =>
            {
                InstantiateBullet(starBullet, isRight ? 0 : 180, animationState == AnimationState.SQUAT);
            })
            .AppendInterval(1.0f)
            .OnComplete(() => { isPlaying = false; })
            .Play();
    }

    private void PlayJumpSequence(Vector2 targetPos)
    {
        bool isNowRightPos = c1Pos.x < transform.position.x;
        Debug.Log(c1Pos);
        Debug.Log(transform.position.x);

        sequence = DOTween.Sequence()
            .AppendCallback(() => { 
                animationState = AnimationState.JUMP;
                anim.SetInteger("state", (int)animationState);
                AudioManager.Instance.PlaySE("jump");
            })
            .Append(transform.DOJump(targetPos, 1.0f, 1, 1.0f))
            .Join(
                DOTween.Sequence()
                .AppendInterval(0.1f)
                .AppendCallback(() => { AudioManager.Instance.PlayExVoice("yukari_down_shot", true);})
                .Append(transform.DOLocalRotate(new Vector3(0, 0, 45), 0.4f / 8))
                .AppendCallback(() => { InstantiateBullet(starBullet, isRight ? -45 : -135); })
                .AppendInterval(0.4f / 8)
                .Append(transform.DOLocalRotate(new Vector3(0, 0, 60), 0.4f / 8))
                .AppendCallback(() => { InstantiateBullet(starBullet, isRight ? -80 : -100); })
                .AppendInterval(0.4f / 8)
                .AppendCallback(() => { InstantiateBullet(starBullet, isRight ? -120 : -60); })
                .Append(transform.DOLocalRotate(new Vector3(0, 0, 360), 0.4f * 4 / 8, RotateMode.FastBeyond360))
                .AppendInterval(0.3f)
            )
            .OnComplete(() => { 
                animationState = AnimationState.STAND;
                anim.SetInteger("state", (int)animationState);
                isPlaying = false;
            })
            .Play();
    }
    
    private void PlayRunSequence(Vector2 targetPos)
    {
        sequence = DOTween.Sequence()
            .AppendCallback(() => { 
                animationState = AnimationState.RUN;
                anim.SetInteger("state", (int)animationState);
            })
            .Append(transform.DOMove(targetPos, 2.0f)).SetEase(Ease.Linear)
            .Join(
                DOTween.Sequence()
                .AppendInterval(0.2f)
                .AppendCallback(() =>
                {
                    InstantiateBullet(starBullet, isRight ? 0 : 180, false);
                })
                .AppendInterval(0.5f)
                .AppendCallback(() =>
                {
                    InstantiateBullet(starBullet, isRight ? 0 : 180, false);
                })
            )
            .OnComplete(() => { 
                animationState = AnimationState.STAND;
                anim.SetInteger("state", (int)animationState);
                isPlaying = false;
            })
            .Play();
    }

    private void InstantiateBullet(GameObject bulletObj, float angleZ, bool isSquat = false)
    {
        float addforceX = Mathf.Cos(angleZ * Mathf.Deg2Rad) * 550.0f;
        float addforceY = Mathf.Sin(angleZ * Mathf.Deg2Rad) * 550.0f;

        GameObject bullet = Instantiate(bulletObj, transform.parent);

        bullet.transform.position = isSquat ? squatBulletPivot.transform.position : bulletPivot.transform.position;
        bullet.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angleZ));
        bullet.GetComponent<Rigidbody2D>().AddForce(new Vector2(addforceX, addforceY));

        AudioManager.Instance.PlaySE("shot_yukari");
    }
}
