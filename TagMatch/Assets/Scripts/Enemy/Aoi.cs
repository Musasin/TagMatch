using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Aoi : Kotonoha
{
    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();

        // 立ち位置を若干後ろにずらす
        leftPos1 = new Vector2 (leftPos1.x - 0.8f, leftPos1.y);
        leftPos2 = new Vector2 (leftPos2.x - 0.8f, leftPos2.y);
        leftPos3 = new Vector2 (leftPos3.x - 0.8f, leftPos3.y);
        leftPos4 = new Vector2 (leftPos4.x - 0.8f, leftPos4.y);
        rightPos1 = new Vector2 (rightPos1.x + 0.8f, rightPos1.y);
        rightPos2 = new Vector2 (rightPos2.x + 0.8f, rightPos2.y);
        rightPos3 = new Vector2 (rightPos3.x + 0.8f, rightPos3.y);
        rightPos4 = new Vector2 (rightPos4.x + 0.8f, rightPos4.y);
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();
    }

    public override void  SetSequence()
    {
        actionStateQueue.Add(ActionState.WAIT);
        actionStateQueue.Add(ActionState.MOVE_TO_LEFT_2);
        actionStateQueue.Add(ActionState.AOI_SHOT);
        actionStateQueue.Add(ActionState.MOVE_TO_RIGHT_3);
        actionStateQueue.Add(ActionState.AOI_SHOT);
        actionStateQueue.Add(ActionState.MOVE_TO_LEFT_4);
        actionStateQueue.Add(ActionState.AOI_SHOT);
        
        if (Random.Range(0, 1) < 0.5f)
        {
            actionStateQueue.Add(ActionState.MOVE_TO_RIGHT_4);
            actionStateQueue.Add(ActionState.MOVE_TO_LEFT_3);
            actionStateQueue.Add(ActionState.MOVE_TO_RIGHT_3);
            actionStateQueue.Add(ActionState.AOI_SHOT);
            actionStateQueue.Add(ActionState.MOVE_TO_LEFT_1);
            actionStateQueue.Add(ActionState.AOI_SHOT);
        } else
        {
            actionStateQueue.Add(ActionState.MOVE_TO_LEFT_4);
            actionStateQueue.Add(ActionState.MOVE_TO_RIGHT_3);
            actionStateQueue.Add(ActionState.MOVE_TO_LEFT_3);
            actionStateQueue.Add(ActionState.AOI_SHOT);
            actionStateQueue.Add(ActionState.MOVE_TO_RIGHT_1);
            actionStateQueue.Add(ActionState.AOI_SHOT);
        }
        
        actionStateQueue.Add(ActionState.MOVE_TO_RIGHT_1);
        actionStateQueue.Add(ActionState.MOVE_TO_RIGHT_4);

        if (Random.Range(0, 1) < 0.5f)
        {
            actionStateQueue.Add(ActionState.MOVE_TO_LEFT_1);
            actionStateQueue.Add(ActionState.AOI_SHOT);
            actionStateQueue.Add(ActionState.MOVE_TO_LEFT_2);
            actionStateQueue.Add(ActionState.AOI_SHOT);
            actionStateQueue.Add(ActionState.MOVE_TO_LEFT_3);
            actionStateQueue.Add(ActionState.AOI_SHOT);
            actionStateQueue.Add(ActionState.MOVE_TO_LEFT_4);
            actionStateQueue.Add(ActionState.AOI_SHOT);
        }
        else
        {
            actionStateQueue.Add(ActionState.MOVE_TO_LEFT_4);
            actionStateQueue.Add(ActionState.AOI_SHOT);
            actionStateQueue.Add(ActionState.MOVE_TO_LEFT_3);
            actionStateQueue.Add(ActionState.AOI_SHOT);
            actionStateQueue.Add(ActionState.MOVE_TO_LEFT_2);
            actionStateQueue.Add(ActionState.AOI_SHOT);
            actionStateQueue.Add(ActionState.MOVE_TO_LEFT_1);
            actionStateQueue.Add(ActionState.AOI_SHOT);
        }

        actionStateQueue.Add(ActionState.IDLE);
    }
}
