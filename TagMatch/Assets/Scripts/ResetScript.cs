using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetScript : MonoBehaviour
{
    Player player;
    WipePanel wipePanel;
    GameObject cameraObject;
    GameObject enemiesObject;
    Vector3 defaultCameraPos;

    bool isResetPlaying;
    float time;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player").GetComponent<Player>();
        wipePanel = GameObject.Find("WipePanel").GetComponent<WipePanel>();
        cameraObject = GameObject.Find("Camera");
        enemiesObject = GameObject.Find("Enemies");
        defaultCameraPos = cameraObject.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (isResetPlaying)
        {
            time += Time.deltaTime;
            if (time > 1.0f)
            {
                wipePanel.WipeIn();
                player.Reset();
                EnemiesReset();
                cameraObject.transform.position = defaultCameraPos;
                GameObject bossStageEntrance = GameObject.Find("BossStageEntrance");
                if (bossStageEntrance != null)
                    bossStageEntrance.GetComponent<BossStageEntrance>().Reset();
                isResetPlaying = false;
            }
        }
    }

    void EnemiesReset()
    {
        StaticValues.ResetBossHP();
        EnemyBase[] enemyList = enemiesObject.GetComponentsInChildren<EnemyBase>();
        foreach (EnemyBase enemy in enemyList)
        {
            enemy.Reset();
        }
        BossAIBase[] bossList = enemiesObject.GetComponentsInChildren<BossAIBase>();
        foreach (BossAIBase boss in bossList)
        {
            boss.Reset();
        }
        Cannon[] cannonList = enemiesObject.GetComponentsInChildren<Cannon>();
        foreach (Cannon cannon in cannonList)
        {
            cannon.Reset();
        }
        AimForPlayerBullet[] aimForPlayerBulletList = enemiesObject.GetComponentsInChildren<AimForPlayerBullet>();
        foreach (AimForPlayerBullet aimForPlayerBullet in aimForPlayerBulletList)
        {
            Destroy(aimForPlayerBullet.gameObject);
        }
        EffectAutoRelease[] effectList = enemiesObject.GetComponentsInChildren<EffectAutoRelease>();
        foreach (EffectAutoRelease effect in effectList)
        {
            Destroy(effect.gameObject);
        }
        DestroyWhenPlayerDead[] destroyObjList = enemiesObject.GetComponentsInChildren<DestroyWhenPlayerDead>();
        foreach (DestroyWhenPlayerDead destroyObj in destroyObjList)
        {
            Destroy(destroyObj.gameObject);
        }
    }

    public void Reset()
    {
        time = 0;
        isResetPlaying = true;
        wipePanel.WipeOut();
    }
}
