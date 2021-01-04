using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OneWayObject : MonoBehaviour
{
    bool isIgnore;
    bool forceIgnore;
    Player playerScript;
    BoxCollider2D bc;

    // Start is called before the first frame update
    void Start()
    {
        isIgnore = false;
        forceIgnore = false;
        playerScript = GameObject.Find("Player").GetComponent<Player>();
        bc = GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        forceIgnore = playerScript.IsGetOff();
        if (!forceIgnore)
        {
            // 降りる処理の解除は毎フレーム行う
            bc.enabled = true;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        switch (collision.gameObject.tag)
        {
            case "Player":
                // プレイヤーが乗っているかつ降りるモードの時だけ降りる
                if (forceIgnore)
                {
                    bc.enabled = false;
                }
                break;
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        switch (collision.gameObject.tag)
        {
            case "Player":
            case "PlayerBullet":
                Debug.Log(collision.gameObject + "enter");
                isIgnore = true;
                Physics2D.IgnoreCollision(collision, GetComponent<Collider2D>(), isIgnore);
                break;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        switch (collision.gameObject.tag)
        {
            case "Player":
            case "PlayerBullet":
                Debug.Log(collision.gameObject + "exit");
                isIgnore = false;
                Physics2D.IgnoreCollision(collision, GetComponent<Collider2D>(), isIgnore);
                break;
        }
    }

    public bool GetIsIgnore()
    {
        return isIgnore;
    }
}