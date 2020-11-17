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
    public GameObject dropItem1;
    public GameObject dropItem2;
    
    const float INVINCIBLE_TIME = 0.2f;
    const float STOP_TIME = 0.2f;
    const float DAMAGE_VELOCITY_X = 4.0f;
    const float DAMAGE_VELOCITY_Y = 8.0f;
    
    int maxHp;
    float invincibleTime = 0;
    bool isKnockBack;
    bool isDead;
    Rigidbody2D rb;
    SpriteRenderer sr;
    Vector2 defaultPosition;
    Vector2 defaultScale;
    GameObject dropedItem1, dropedItem2;

    // Start is called before the first frame update
    void Start()
    {
        maxHp = hp;
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        defaultPosition = transform.localPosition;
        defaultScale = transform.localScale;
    }

    public void Reset()
    {
        hp = maxHp;
        invincibleTime = 0;
        isKnockBack = false;
        isDead = false;
        transform.localPosition = defaultPosition;
        transform.localScale = defaultScale;
        transform.rotation = new Quaternion(0, 0, 0, 0);
        transform.GetComponent<BoxCollider2D>().enabled = true;
        rb.velocity = Vector2.zero;
        if (dropedItem1 != null) Destroy(dropedItem1);
        if (dropedItem2 != null) Destroy(dropedItem2);
    }

    // Update is called once per frame
    void Update()
    {
        invincibleTime -= Time.deltaTime;
        UpdateColor();

        // 初期化処理 仮でボタンで実行
        if (Input.GetKeyDown(KeyCode.R))
        {
            Reset();
        }
        
        if (isDead)
        {
            return;
        }
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
            
            dropedItem1 = InstantiateDropItem(dropItem1, new Vector2(transform.position.x - 0.1f, transform.position.y));
            dropedItem2 = InstantiateDropItem(dropItem2, new Vector2(transform.position.x + 0.1f, transform.position.y));

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
    private GameObject InstantiateDropItem(GameObject obj, Vector2 pos)
    {
        if (obj == null)
            return null;

        GameObject item = Instantiate(obj);
        item.transform.position = pos;
        item.GetComponent<Rigidbody2D>().AddForce(new Vector2(0, 10.0f));
        return item;
    }
    public bool IsInvincible()
    {
        return (invincibleTime > 0);
    }
}
