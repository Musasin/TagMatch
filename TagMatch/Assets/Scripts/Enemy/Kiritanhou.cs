using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Kiritanhou : MonoBehaviour
{
    Rigidbody2D rb;
    GameObject player;
    RotateWithDirection rotateScript;
    
    public bool isAimForPlayer;
    public float aimForPlayerPower;
    public float deadTime;
    public float delayTime;


    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player");
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        deadTime -= Time.deltaTime;
        if (deadTime <= 0)
        {
            Destroy(gameObject);
        }

        if (isAimForPlayer)
        {
            delayTime -= Time.deltaTime;
            if (delayTime <= 0)
            {
                AimForPlayerShot();
            } else
            {
                Vector2 v = GetAimForPlayerShotVector();
                float atan = Mathf.Atan2(v.y, v.x);
                transform.rotation = Quaternion.Euler(0, 0, atan * Mathf.Rad2Deg + 180);
            }
        }
    }

    Vector2 GetAimForPlayerShotVector()
    {
        float px = player.transform.position.x;
        float py = player.transform.position.y;
        float x = transform.position.x;
        float y = transform.position.y;

        float disX = Mathf.Abs(px - x);
        float disY = Mathf.Abs(py - y);

        float rateX = 1;
        float rateY = 1;
        if (disX > disY)
            rateY = disY / disX;
        else
            rateX = disX / disY;

        rateX = (px < x) ? -rateX : rateX;
        rateY = (py < y) ? -rateY : rateY;

        return new Vector2(rateX * aimForPlayerPower, rateY * aimForPlayerPower);
    }

    void AimForPlayerShot()
    {
        AudioManager.Instance.PlaySE("shot_kiritanhou");
        rb.AddForce(GetAimForPlayerShotVector());

        isAimForPlayer = false;
    }
}
