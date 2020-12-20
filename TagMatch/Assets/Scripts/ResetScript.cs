using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetScript : MonoBehaviour
{
    Player player;
    WipePanel wipePanel;
    GameObject cameraObject;
    Vector3 defaultCameraPos;

    bool isResetPlaying;
    float time;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player").GetComponent<Player>();
        wipePanel = GameObject.Find("WipePanel").GetComponent<WipePanel>();
        cameraObject = GameObject.Find("Camera");
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
                isResetPlaying = false;
            }
        }
    }

    void EnemiesReset()
    {
        EnemyBase[] enemyList = GameObject.Find("Enemies").GetComponentsInChildren<EnemyBase>();
        foreach (EnemyBase enemy in enemyList)
        {
            enemy.Reset();
        }
    }

    public void Reset()
    {
        isResetPlaying = true;
        wipePanel.WipeOut();
    }
}
