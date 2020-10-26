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

    const float INVINCIBLE_TIME = 0.2f;
    
    float invincibleTime = 0;
    Rigidbody2D rb;
    SpriteRenderer sr;
    Vector2 defaultScale;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        defaultScale = transform.localScale;
    }

    // Update is called once per frame
    void Update()
    {
        invincibleTime -= Time.deltaTime;
        rb.velocity = new Vector2(velocityX * (isRight ? 1 : -1), rb.velocity.y);
        transform.localScale = new Vector2(defaultScale.x * (isRight ? -1 : 1), defaultScale.y); // デフォルトは左向き
        UpdateColor();
    }
    
    private void UpdateColor()
    {
        float alpha = (invincibleTime > 0) ? 0.5f : 1.0f;
        sr.color = new Color(1.0f, 1.0f, 1.0f, alpha);
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
    public void HitBullet(int damage)
    {
        if (invincibleTime > 0)
        {
            return;
        }
        hp -= damage;
        invincibleTime = INVINCIBLE_TIME;
    }
    public bool IsInvincible()
    {
        return (invincibleTime > 0);
    }
}
