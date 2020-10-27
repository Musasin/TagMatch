using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackGround : MonoBehaviour
{
    public float resetPositionX;
    public float speed;
    private GameObject cameraObject;
    private Vector3 cameraPos;

    // Use this for initialization
    void Start()
    {
        cameraObject = GameObject.Find("Camera");
        cameraPos = cameraObject.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        float XPos = this.gameObject.transform.localPosition.x;
        float YPos = this.gameObject.transform.localPosition.y;
        float ZPos = this.gameObject.transform.localPosition.z;

        Vector3 newcameraPos = cameraObject.transform.position;
        XPos -= (newcameraPos.x - cameraPos.x) * speed;
        YPos -= (newcameraPos.y - cameraPos.y) * speed / 5.0f;

        if (this.gameObject.transform.localPosition.x < -resetPositionX)
        {
            XPos += resetPositionX * 2;
        }
        else if (this.gameObject.transform.localPosition.x > resetPositionX)
        {
            XPos -= resetPositionX * 2;
        }
        cameraPos = newcameraPos;
        this.gameObject.transform.localPosition = new Vector3(XPos, YPos, ZPos);
    }
}