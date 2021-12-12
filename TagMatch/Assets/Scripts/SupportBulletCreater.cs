using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class SupportBulletCreater : MonoBehaviour
{
    public GameObject kiritanBullet, zunkoBullet, itakoBullet, akaneBullet, aoiBullet;
    GameObject target, rightUpPos, rightPos, leftUpPos;
    
    enum ActionState
    {
        START,
        WAIT,
        KIRITAN,
        ZUNKO,
        ITAKO,
        AKANE,
        AOI,
    }
    ActionState state;
    List<ActionState> actionStateQueue = new List<ActionState>();
    int stateIndex;
    bool isPlaying = false;
    bool isEnabled = false;
    Sequence sequence;

    // Start is called before the first frame update
    void Start()
    {
        target = GameObject.Find("Frimomen2");
        rightUpPos = GameObject.Find("SupportBulletRightUpPos");
        rightPos = GameObject.Find("SupportBulletRightPos");
        leftUpPos = GameObject.Find("SupportBulletLeftUpPos");
        actionStateQueue.Clear();
        stateIndex = 0;
        state = ActionState.START;
    }

    // Update is called once per frame
    void Update()
    {
        if (!StaticValues.isFixedCamera || StaticValues.isPause || StaticValues.isTalkPause)
        {
            return;
        }
        
        // 有効化処理が呼ばれるまでは何もしない
        if (!isEnabled)
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

                actionStateQueue.Add(ActionState.WAIT);
                actionStateQueue.Add(ActionState.KIRITAN);
                actionStateQueue.Add(ActionState.ITAKO);
                actionStateQueue.Add(ActionState.ZUNKO);
                actionStateQueue.Add(ActionState.WAIT);
                actionStateQueue.Add(ActionState.AKANE);
                actionStateQueue.Add(ActionState.AOI);
                actionStateQueue.Add(ActionState.START);
                break;
                
            case ActionState.WAIT:
                isPlaying = true;
                sequence = DOTween.Sequence()
                    .AppendInterval(1.0f)
                    .OnComplete(() => { isPlaying = false; })
                    .Play();
                break;
                
            case ActionState.KIRITAN:
                AudioManager.Instance.PlayExVoice("kiritan_attack1");
                AudioManager.Instance.PlaySE("buon");
                GameObject kb = Instantiate(kiritanBullet, transform.parent);
                kb.transform.position = rightUpPos.transform.position;
                break;
            case ActionState.ZUNKO:
                AudioManager.Instance.PlayExVoice("zunko_attack");
                AudioManager.Instance.PlaySE("swing");
                GameObject zb = Instantiate(zunkoBullet, transform.parent);
                zb.transform.position = rightPos.transform.position;
                zb.GetComponent<Rigidbody2D>().AddForce(new Vector2(-1000, 0));
                break;
            case ActionState.ITAKO:
                AudioManager.Instance.PlayExVoice("itako_fire");
                AudioManager.Instance.PlaySE("fire");
                GameObject ib = Instantiate(itakoBullet, transform.parent);
                ib.transform.position = target.transform.position;
                break;
            case ActionState.AKANE:
                AudioManager.Instance.PlayExVoice("akane_attack");
                AudioManager.Instance.PlaySE("fire");
                GameObject akab = Instantiate(akaneBullet, transform.parent);
                akab.transform.position = target.transform.position;
                break;
            case ActionState.AOI:
                AudioManager.Instance.PlayExVoice("aoi_attack");
                AudioManager.Instance.PlaySE("buon");
                GameObject aob = Instantiate(aoiBullet, transform.parent);
                aob.transform.position = leftUpPos.transform.position;
                break;
        }
        state = actionStateQueue[stateIndex];
        stateIndex++;
    }

    public void SetEnabled(bool flag)
    {
        isEnabled = flag;
    }
}
