using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Kiritan : MonoBehaviour
{
    public GameObject kiritanhouBullet;
    GameObject kiritanhouPos1, kiritanhouPos2;
    Vector2 upperLeftPos, upperRightPos;
    int stateIndex;
    bool isActive = true;
    bool isPlaying = false;
    bool isRight = false;

    const float BULLET_INTERVAL_TIME = 1.5f;
    float bulletTime;

    enum ActionState
    {
        IDLE,
        MOVE_TO_UPPER_LEFT,
        MOVE_TO_UPPER_RIGHT
    }
    ActionState state;
    List<ActionState> actionStateQueue = new List<ActionState>();


    // Start is called before the first frame update
    void Start()
    {
        state = ActionState.IDLE;
        upperLeftPos = GameObject.Find("UpperLeftPos").transform.position;
        upperRightPos = GameObject.Find("UpperRightPos").transform.position;
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

        bulletTime += Time.deltaTime;
        if (bulletTime > BULLET_INTERVAL_TIME)
        {
            bulletTime = 0;
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
                actionStateQueue.Add(ActionState.MOVE_TO_UPPER_RIGHT);
                actionStateQueue.Add(ActionState.MOVE_TO_UPPER_LEFT);
                actionStateQueue.Add(ActionState.MOVE_TO_UPPER_RIGHT);
                actionStateQueue.Add(ActionState.MOVE_TO_UPPER_LEFT);
                actionStateQueue.Add(ActionState.MOVE_TO_UPPER_RIGHT);
                actionStateQueue.Add(ActionState.IDLE);
                break;
            case ActionState.MOVE_TO_UPPER_LEFT:
                transform.DOLocalMove(upperLeftPos, 3.0f).OnComplete(() => { isPlaying = false; });
                isRight = false;
                transform.localScale = new Vector2(-1, 1);
                isPlaying = true;
                break;
            case ActionState.MOVE_TO_UPPER_RIGHT:
                transform.DOLocalMove(upperRightPos, 3.0f).OnComplete(() => { isPlaying = false; });
                isRight = true;
                transform.localScale = new Vector2(1, 1);
                isPlaying = true;
                break;
        }
        state = actionStateQueue[stateIndex];
        stateIndex++;
    }
}
