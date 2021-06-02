using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TalkFlag : MonoBehaviour
{
    public string scenarioFileName;
    public bool isForcePlay;
    Talk talkScript;
    bool isPlayed;

    // Start is called before the first frame update
    void Start()
    {
        talkScript = GameObject.Find("TalkUI").GetComponent<Talk>();
        if (isForcePlay)
        {
            talkScript.SetScenario(scenarioFileName, false);
            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
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
