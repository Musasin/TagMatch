using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TalkFlag : MonoBehaviour
{
    public string scenarioFileName;
    public bool isForcePlay;
    Talk talkScript;
    bool isPlayed;

    float time;
    float delayTime = 0.5f;

    // Start is called before the first frame update
    void Start()
    {
        talkScript = GameObject.Find("TalkUI").GetComponent<Talk>();
        if (isForcePlay)
        {
            // 発火前に動かれないように止めておく
            StaticValues.isTalkPause = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isForcePlay)
        {
            // 強制実行の場合、ワイプとタイミングが重ならないように少し待ってから発火
            time += Time.deltaTime;
            if (time >= delayTime)
            {
                talkScript.SetScenario(scenarioFileName, false);
                Destroy(gameObject);
            }
        }
    }

    public void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            Rigidbody2D rb = collision.gameObject.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.velocity = Vector2.zero;
            }
            if (!isPlayed)
            {
                talkScript.SetScenario(scenarioFileName, false);
            }
            isPlayed = true;
            Destroy(gameObject);
        }
    }
}
