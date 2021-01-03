using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossStageEntrance : MonoBehaviour
{
    public string scenarioFileName;
    BoxCollider2D bc;
    Camera cameraScript;
    Transform cameraMarker;
    bool isTrigger, isTalked;
    Talk talkScript;

    void Start()
    {
        bc = GetComponent<BoxCollider2D>();
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
                bc.enabled = false;
                cameraScript.SetFixedPos(cameraMarker.position);
                Rigidbody2D rb = collision.gameObject.GetComponent<Rigidbody2D>();
                if (rb != null)
                {
                    rb.velocity = Vector2.zero;
                }
                if (!isTalked)
                {
                    talkScript.SetScenario(scenarioFileName, true);
                    isTalked = true;
                }
                isTrigger = true;
                break;
        }
    }

    public bool GetTrigger()
    {
        return isTrigger;
    }

    public void Reset()
    {
        cameraScript.CancelFixedPos();
        StaticValues.isFixedCamera = false;
        bc.enabled = true;
        isTrigger = false;
    }
}