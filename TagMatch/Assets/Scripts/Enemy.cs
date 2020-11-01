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
    public bool isSuperArmor;
    public bool isRight;
    public GameObject damagePointEffect;
    
    const float INVINCIBLE_TIME = 0.2f;
    const float STOP_TIME = 0.2f;
    const float DAMAGE_VELOCITY_X = 4.0f;
    const float DAMAGE_VELOCITY_Y = 8.0f;
    
    float invincibleTime = 0;
    bool isKnockBack;
    bool isDead;
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
        UpdateColor();
        
        if (isKnockBack) // 被ダメージ硬直
        {
            return;
        }
        rb.velocity = new Vector2(velocityX * (isRight ? 1 : -1), rb.velocity.y);
        transform.localScale = new Vector2(defaultScale.x * (isRight ? -1 : 1), defaultScale.y); // デフォルトは左向き
    }
    
    private void UpdateColor()
    {
        float f = (invincibleTime > 0) ? 0.5f : 1.0f;
        sr.color = new Color(1.0f, f, f, f);
    }

    public void HitWall()
    {
        // 向いてる方向と進んでいる方向が一致している時のみ方向転換する (ノックバックで背中から当たる場合があるため)
        if (rb.velocity.x > 0 && isRight || rb.velocity.x < 0 && !isRight)
        {
            isRight = !isRight;
        }
        if (isKnockBack) // 壁にぶつかってもノックバック状態を解除
        {
            isKnockBack = false;
        }
    }
    public void HitGround()
    {
        if (isKnockBack) // 着地でノックバック状態を解除
        {
            isKnockBack = false;
        }
    }
    public void HitGroundEnd()
    {
        if (isKnockBack) // ダメージを受けて足が離れた時は判定しない
        {
            return;
        }
        if (!isFall)
        {
            isRight = !isRight;
        }
    }
    public void HitBullet(int damage, GameObject hitObject)
    {
        if (invincibleTime > 0)
        {
            return;
        }

        hp -= damage;
        GameObject dp = Instantiate(damagePointEffect);
        dp.transform.position = transform.position;
        dp.GetComponent<DamagePointEffect>().SetDamagePointAndPlay(damage);

        if (hp <= 0)
        {
            isDead = true;
            transform.GetComponent<BoxCollider2D>().enabled = false;
            bool isBulletRight= hitObject.GetComponent<Rigidbody2D>().velocity.x < 0;
            rb.velocity = new Vector2(isBulletRight ? -DAMAGE_VELOCITY_X : DAMAGE_VELOCITY_X, DAMAGE_VELOCITY_Y);
            transform.Rotate(new Vector3(0, 0, 180.0f));
        } 
        else
        {
            invincibleTime = INVINCIBLE_TIME;
            if (!isSuperArmor)
            {
                isKnockBack = true;
                bool isBulletRight= hitObject.GetComponent<Rigidbody2D>().velocity.x < 0; // vecolicyがマイナスなら右から左に向かっている
                rb.velocity = new Vector2(isBulletRight ? -DAMAGE_VELOCITY_X : DAMAGE_VELOCITY_X, DAMAGE_VELOCITY_Y);
            }
        }
    }
    public bool IsInvincible()
    {
        return (invincibleTime > 0);
    }
}
