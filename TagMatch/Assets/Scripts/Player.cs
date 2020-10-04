using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    const float MOVE_VELOCITY = 6.0f;
    const float JUMP_VELOCITY = 15.0f;

    Rigidbody2D rb;
    Animator anim;

    float velocityX = 0;
    float velocityY = 0;
    bool isRight = true;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        velocityX = rb.velocity.x;
        velocityY = rb.velocity.y;
        UpdateMove();
        UpdateJump();
        UpdateDirection();
        rb.velocity = new Vector2(velocityX, velocityY);
    }

    private void UpdateMove()
    {
        float dx = Input.GetAxisRaw("Horizontal");
        if (dx > 0)
        {
            anim.SetInteger("state", 1);
            velocityX = MOVE_VELOCITY;
            isRight = true;
        }
        else if (dx < 0)
        {
            anim.SetInteger("state", 1);
            velocityX = -MOVE_VELOCITY;
            isRight = false;
        }
        else
        {
            anim.SetInteger("state", 0);
            velocityX = 0;
        }
    }
    private void UpdateJump()
    {
        if (Input.GetButtonDown("Jump"))
        {
            velocityY = JUMP_VELOCITY;
        }
    }
    private void UpdateDirection()
    {
        if (isRight)
        {
            transform.localScale = new Vector2(1, 1);
        }
        else
        {
            transform.localScale = new Vector2(-1, 1);
        }
    }
}
