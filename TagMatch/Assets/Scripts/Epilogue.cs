using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Epilogue : MonoBehaviour
{
    Animator anim;
    AutoMoveCamera autoMoveCamera;
    int stepCount = 0;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        autoMoveCamera = GetComponent<AutoMoveCamera>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Step()
    {
        stepCount++;
        anim.SetTrigger("step");

        if (stepCount >= 3)
        {
            autoMoveCamera.CAMERA_MOVE_SPEED = 0;
        } else
        {
            autoMoveCamera.CAMERA_MOVE_SPEED = -2;
        }
    }
}
