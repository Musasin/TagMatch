using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int hp;
    public string type;
    public float velocityX;
    public float velocityY;
    public bool isFall;
    public bool isRight;

    Rigidbody2D rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        rb.velocity = new Vector2(velocityX * (isRight ? 1 : -1), rb.velocity.y);
        transform.localScale = new Vector2(isRight ? -1 : 1, 1);
    }

    public void HitWall()
    {
        isRight = !isRight;
    }
    public void HitGroundEnd()
    {
        if (!isFall)
        {
            isRight = !isRight;
        }
    }
}
