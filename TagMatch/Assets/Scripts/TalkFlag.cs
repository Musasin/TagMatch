using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TalkFlag : MonoBehaviour
{
    public string scenarioFileName;
    Talk talkScript;
    bool isPlayed;

    // Start is called before the first frame update
    void Start()
    {
        talkScript = GameObject.Find("TalkUI").GetComponent<Talk>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnTriggerStay2D(Collider2D collision)
    {
        if (isPlayed)
            return;

        if (collision.tag == "Player")
        {
            isPlayed = true;
            Rigidbody2D rb = collision.gameObject.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.velocity = Vector2.zero;
            }
            talkScript.SetScenario(scenarioFileName, false);
            Destroy(gameObject);
        }
    }
}
