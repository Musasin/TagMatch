using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    const float MOVE_VELOCITY = 5.0f;
    const float JUMP_VELOCITY = 16.0f;
    float velocityX = 0, velocityY = 0;
    Rigidbody2D rb;
    bool isRight = true;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        velocityX = rb.velocity.x;
        velocityY = rb.velocity.y;
        UpdateMove();
        UpdateJump();
        rb.velocity = new Vector2(velocityX, velocityY);
    }

    private void UpdateMove()
    {
        float dx = Input.GetAxisRaw("Horizontal");
        if (dx > 0)
        {
            velocityX = MOVE_VELOCITY;
            isRight = true;
            transform.localScale = new Vector2(1, 1);
        }
        else if (dx < 0)
        {
            velocityX = -MOVE_VELOCITY;
            isRight = false;
            transform.localScale = new Vector2(-1, 1);
        }
        else
        {
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
}
