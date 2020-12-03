using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera : MonoBehaviour
{
    const float POSITION_SYNC_DISTANCE_X = 1.5f;
    const float POSITION_SYNC_DISTANCE_Y = 1.0f;
    const float FORCE_POSITION_SYNC_DISTANCE_Y = 2.0f;
    const float TITLE_SYNC_DISTANCE_X = 1.0f;
    const float MIN_POSITION_X = 0.0f;
    const float MIN_POSITION_Y = 0.0f;
    const float CAMERA_MOVE_SPEED = 10.0f;
    const float FORCE_CAMERA_MOVE_SPEED = 30.0f;
    private GameObject player;
    float targetPosX;
    float targetPosY;
    Vector3 fixedPos;
    bool isFixedPos;
    bool fixPosMoving;
    
    public bool isTitleSceneMode = false;
    public bool isEndingSceneMode = false;
    Vector2 terminationPoint;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        targetPosX = 0.0f;
        targetPosY = 0.0f;
        fixPosMoving = false;
        GameObject terminationPointObj = GameObject.Find("TerminationPoint");
        if (terminationPointObj != null)
        {
            terminationPoint = new Vector2(terminationPointObj.transform.position.x - 8, terminationPointObj.transform.position.y - 6);
        } else
        {
            terminationPoint = new Vector2(Mathf.Infinity, Mathf.Infinity);
        }

        //transform.position = GetNewPositionWithSavedPos();
    }

    void Update()
    {

    }

    void LateUpdate()
    {
        if (isFixedPos)
            UpdatePositionWithFixedPos();
        else
            UpdatePositionWithPlayerPos();
    }

    void UpdatePositionWithFixedPos()
    {
        float dis = Vector3.Distance(transform.position, fixedPos);
        if (dis <= 1.0f && fixPosMoving)
        {
            StaticValues.isPause = false;
            StaticValues.isFixedCamera = true;
            fixPosMoving = false;
        }
        transform.position = Vector3.MoveTowards(transform.position, fixedPos, CAMERA_MOVE_SPEED * Time.deltaTime);
    }

    void UpdatePositionWithPlayerPos()
    {
        float cameraSpeed = CAMERA_MOVE_SPEED;
        float disY = Mathf.Abs(player.transform.position.y - this.transform.position.y);
        if (disY >= FORCE_POSITION_SYNC_DISTANCE_Y)
        {
            cameraSpeed = FORCE_CAMERA_MOVE_SPEED;
        }

        float step = cameraSpeed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, GetNewPositionWithPlayerPos(), step);
    }

    Vector3 GetNewPositionWithPlayerPos()
    {
        float disX = Mathf.Abs(player.transform.position.x - this.transform.position.x);
        if (isTitleSceneMode && disX >= TITLE_SYNC_DISTANCE_X)
        {
            targetPosX = player.transform.position.x - TITLE_SYNC_DISTANCE_X;
        } else if(isEndingSceneMode)
        {
            targetPosX = player.transform.position.x;
        }
        else if (disX >= POSITION_SYNC_DISTANCE_X)
        {
            if (player.transform.position.x > this.transform.position.x)
                targetPosX = player.transform.position.x - POSITION_SYNC_DISTANCE_X;
            else
                targetPosX = player.transform.position.x + POSITION_SYNC_DISTANCE_X;
        }
        float disY = Mathf.Abs(player.transform.position.y - this.transform.position.y);
        if (disY >= FORCE_POSITION_SYNC_DISTANCE_Y)
        {
            if (player.transform.position.y > this.transform.position.y)
                targetPosY = player.transform.position.y - FORCE_POSITION_SYNC_DISTANCE_Y;
            else
                targetPosY = player.transform.position.y + FORCE_POSITION_SYNC_DISTANCE_Y;
        }

        Vector3 newPosition = transform.position;
        newPosition.x = Mathf.Min(Mathf.Max(targetPosX, MIN_POSITION_X), terminationPoint.x);
        newPosition.y = Mathf.Min(Mathf.Max(targetPosY, MIN_POSITION_Y), terminationPoint.y);
        newPosition.z = this.transform.position.z;
        return newPosition;
    }

    //Vector3 GetNewPositionWithSavedPos()
    //{
    //    Vector2 savedPos = StaticValues.playerPos;
    //    float disX = Mathf.Abs(savedPos.x - this.transform.position.x);
    //    if (disX >= POSITION_SYNC_DISTANCE_X)
    //    {
    //        if (savedPos.x > this.transform.position.x)
    //            targetPosX = savedPos.x - POSITION_SYNC_DISTANCE_X;
    //        else
    //            targetPosX = savedPos.x + POSITION_SYNC_DISTANCE_X;
    //    }
    //    float disY = Mathf.Abs(savedPos.y - this.transform.position.y);
    //    if (disY >= FORCE_POSITION_SYNC_DISTANCE_Y)
    //    {
    //        if (savedPos.y > this.transform.position.y)
    //            targetPosY = savedPos.y - FORCE_POSITION_SYNC_DISTANCE_Y;
    //        else
    //            targetPosY = savedPos.y + FORCE_POSITION_SYNC_DISTANCE_Y;
    //    }

    //    Vector3 newPosition = transform.position;
    //    newPosition.x = targetPosX;
    //    newPosition.y = Mathf.Max(targetPosY, MIN_POSITION_Y);
    //    newPosition.z = this.transform.position.z;
    //    return newPosition;
    //}

    public void UpdatePlayerPosY()
    {
        float disY = Mathf.Abs(player.transform.position.y - this.transform.position.y);
        if (disY >= POSITION_SYNC_DISTANCE_Y)
            targetPosY = player.transform.position.y;
    }

    public void SetFixedPos(Vector3 pos)
    {
        StaticValues.isPause = true;
        isFixedPos = true;
        fixPosMoving = true;
        fixedPos = pos;
    }
    public void CancelFixedPos()
    {
        isFixedPos = false;
    }

    public bool IsFixPosMoving()
    {
        return fixPosMoving;
    }
}