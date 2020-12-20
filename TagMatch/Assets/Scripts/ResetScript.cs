using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetScript : MonoBehaviour
{
    Player player;
    WipePanel wipePanel;

    bool isResetPlaying;
    float time;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player").GetComponent<Player>();
        wipePanel = GameObject.Find("WipePanel").GetComponent<WipePanel>();
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
                // enemies reset
                isResetPlaying = false;
            }
        }
    }

    public void Reset()
    {
        isResetPlaying = true;
        wipePanel.WipeOut();
    }
}
