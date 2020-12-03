﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossStageEntrance : MonoBehaviour
{
    Camera cameraScript;
    Transform cameraMarker;
    bool isTrigger;

    void Start()
    {
        isTrigger = false;
        cameraScript = GameObject.Find("Camera").GetComponent<Camera>();
        cameraMarker = transform.Find("CameraMarker");
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        switch (collision.gameObject.tag)
        {
            case "Player":
                cameraScript.SetFixedPos(cameraMarker.position);
                Physics2D.IgnoreCollision(collision, GetComponent<Collider2D>(), true);
                isTrigger = true;
                break;
        }
    }

    public bool GetTrigger()
    {
        return isTrigger;
    }
}