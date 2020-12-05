using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossStageEntrance : MonoBehaviour
{
    public string scenarioFileName;
    Camera cameraScript;
    Transform cameraMarker;
    bool isTrigger;
    Talk talkScript;

    void Start()
    {
        isTrigger = false;
        cameraScript = GameObject.Find("Camera").GetComponent<Camera>();
        cameraMarker = transform.Find("CameraMarker");
        talkScript = GameObject.Find("TalkUI").GetComponent<Talk>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (isTrigger)
            return;

        switch (collision.gameObject.tag)
        {
            case "Player":
                cameraScript.SetFixedPos(cameraMarker.position);
                Physics2D.IgnoreCollision(collision, GetComponent<Collider2D>(), true);
                talkScript.SetScenario(scenarioFileName, true);
                Rigidbody2D rb = collision.gameObject.GetComponent<Rigidbody2D>();
                if (rb != null)
                {
                    rb.velocity = Vector2.zero;
                }
                isTrigger = true;
                break;
        }
    }

    public bool GetTrigger()
    {
        return isTrigger;
    }
}