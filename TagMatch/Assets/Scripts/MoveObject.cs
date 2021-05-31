using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveObject : MonoBehaviour
{
    public float moveX;
    public float moveY;
    public float distanceX;
    public float distanceY;
    float firstPosX, firstPosY;
    float posX, posY;
    bool isFront;

    Transform playerTransform;
    int ridePlayerCount;

    // Start is called before the first frame update
    void Start()
    {
        firstPosX = transform.localPosition.x;
        firstPosY = transform.localPosition.y;
        posX = transform.localPosition.x;
        posY = transform.localPosition.y;
        isFront = true;

        playerTransform = GameObject.Find("Player").GetComponent<Transform>();
        ridePlayerCount = 0;
    }

    // Update is called once per frame
    void Update()
    {
        bool newIsFront = isFront;

        if (isFront)
        {
            posX += moveX * Time.deltaTime;
            posY += moveY * Time.deltaTime;
            if (ridePlayerCount > 0)
            {
                playerTransform.localPosition = new Vector2(playerTransform.localPosition.x + moveX * Time.deltaTime, playerTransform.localPosition.y + moveY * Time.deltaTime);
            }
        } else
        {
            posX -= moveX * Time.deltaTime;
            posY -= moveY * Time.deltaTime;
            if (ridePlayerCount > 0)
            {
                playerTransform.localPosition = new Vector2(playerTransform.localPosition.x - moveX * Time.deltaTime, playerTransform.localPosition.y - moveY * Time.deltaTime);
            }
        }

        if (Mathf.Abs(posX - firstPosX) > distanceX) {
            if (moveX > 0 && isFront || moveX < 0 && !isFront)
            {
                posX = firstPosX + distanceX;
            }
            else
            {
                posX = firstPosX - distanceX;
            }
            newIsFront = !isFront;
        }
        
        if (Mathf.Abs(posY - firstPosY) > distanceY) {
            if (moveY > 0 && isFront || moveY < 0 && !isFront)
            {
                posY = firstPosY + distanceY;
            }
            else
            {
                posY = firstPosY - distanceY;
            }
            newIsFront = !isFront;
        }

        isFront = newIsFront;
        transform.localPosition = new Vector2(posX, posY);
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            ridePlayerCount--;
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            ridePlayerCount++;
        }
    }
}
