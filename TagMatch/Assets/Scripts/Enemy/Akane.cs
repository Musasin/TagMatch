using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Akane : Kotonoha
{
    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();
    }

    public override void SetSequence()
    {
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
