using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Akane : Kotonoha
{
    bool playedStartVoice = false;

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    public override void Update()
    {
        // 一回だけ呼ばれるように茜の方でだけ再生
        if (!isDead && bossScript.IsDead())
        {
            AudioManager.Instance.PlayExVoice("kotonoha_dead");
        }
        base.Update();
    }

    public override void Reset()
    {
        base.Reset();
        playedStartVoice = false;
    }

    public override void SetSequence()
    {
        // 開幕に一回だけ呼ばれるように茜の方でだけ再生
        if (!playedStartVoice)
        {
            AudioManager.Instance.PlayExVoice("kotonoha_start");
            playedStartVoice = true;
        }

        actionStateQueue.Add(ActionState.AKANE_FLAME);
        actionStateQueue.Add(ActionState.MOVE_TO_LEFT_2);
        actionStateQueue.Add(ActionState.WAIT);
        actionStateQueue.Add(ActionState.MOVE_TO_RIGHT_3);
        actionStateQueue.Add(ActionState.AKANE_FLAME);
        actionStateQueue.Add(ActionState.MOVE_TO_LEFT_4);
        actionStateQueue.Add(ActionState.AKANE_FLAME);

        if (Random.Range(0, 1) < 0.5f)
        {
            actionStateQueue.Add(ActionState.MOVE_TO_RIGHT_4);
            actionStateQueue.Add(ActionState.MOVE_TO_LEFT_3);
            actionStateQueue.Add(ActionState.MOVE_TO_RIGHT_3);
            actionStateQueue.Add(ActionState.AKANE_FLAME);
            actionStateQueue.Add(ActionState.MOVE_TO_LEFT_1);
            actionStateQueue.Add(ActionState.AKANE_FLAME);
        } else
        {
            actionStateQueue.Add(ActionState.MOVE_TO_LEFT_4);
            actionStateQueue.Add(ActionState.MOVE_TO_RIGHT_3);
            actionStateQueue.Add(ActionState.MOVE_TO_LEFT_3);
            actionStateQueue.Add(ActionState.AKANE_FLAME);
            actionStateQueue.Add(ActionState.MOVE_TO_RIGHT_1);
            actionStateQueue.Add(ActionState.AKANE_FLAME);
        }
        
        actionStateQueue.Add(ActionState.MOVE_TO_LEFT_4);
        actionStateQueue.Add(ActionState.MOVE_TO_LEFT_1);

        if (Random.Range(0, 1) < 0.5f)
        {
            actionStateQueue.Add(ActionState.MOVE_TO_RIGHT_1);
            actionStateQueue.Add(ActionState.AKANE_FLAME);
            actionStateQueue.Add(ActionState.MOVE_TO_RIGHT_2);
            actionStateQueue.Add(ActionState.AKANE_FLAME);
            actionStateQueue.Add(ActionState.MOVE_TO_RIGHT_3);
            actionStateQueue.Add(ActionState.AKANE_FLAME);
            actionStateQueue.Add(ActionState.MOVE_TO_RIGHT_4);
            actionStateQueue.Add(ActionState.AKANE_FLAME);
        }
        {
            actionStateQueue.Add(ActionState.MOVE_TO_RIGHT_4);
            actionStateQueue.Add(ActionState.AKANE_FLAME);
            actionStateQueue.Add(ActionState.MOVE_TO_RIGHT_3);
            actionStateQueue.Add(ActionState.AKANE_FLAME);
            actionStateQueue.Add(ActionState.MOVE_TO_RIGHT_2);
            actionStateQueue.Add(ActionState.AKANE_FLAME);
            actionStateQueue.Add(ActionState.MOVE_TO_RIGHT_1);
            actionStateQueue.Add(ActionState.AKANE_FLAME);
        }

        actionStateQueue.Add(ActionState.IDLE);
    }
}
