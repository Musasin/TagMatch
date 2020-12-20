using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Enemy : EnemyBase
{
    public string type;
    public float velocityX;
    public float velocityY;
    public float attackInterval;
    public float shotPower;
    public bool isFall;
    public bool isSuperArmor;
    public bool isRight;
    public GameObject bullet;
    public GameObject dropItem1;
    public GameObject dropItem2;
    
    const float STOP_TIME = 0.5f;
    const float DAMAGE_VELOCITY_X = 4.0f;
    const float DAMAGE_VELOCITY_Y = 8.0f;
    
    float afterAttackTime = 0;
    bool isKnockBack;
    bool isAttacking;
    bool isDead;
    Rigidbody2D rb;
    Animator anim;
    BoxCollider2D bc;
    Vector2 defaultScale;
    GameObject dropedItem1, dropedItem2;
    Sequence attackSequence;

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<Animator>();
        bc = transform.GetComponent<BoxCollider2D>();
        defaultScale = transform.localScale;
    }

    public override void Reset()
    {
        base.Reset();
        isKnockBack = false;
        isAttacking = false;
        isDead = false;
        afterAttackTime = 0;
        if (dropedItem1 != null) Destroy(dropedItem1);
        if (dropedItem2 != null) Destroy(dropedItem2);
        bc.enabled = true;
        rb.velocity = Vector2.zero;
        transform.localScale = defaultScale;
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();
        
        if (isDead)
        {
            anim.SetBool("isKnockBack", true);
            return;
        }
        
        anim.SetBool("isKnockBack", isKnockBack);
        if (isKnockBack) // 被ダメージ硬直
        {
            return;
        }

        if (isAttacking)
        {
            return;
        }

        afterAttackTime += Time.deltaTime;
        if (attackInterval != 0 && afterAttackTime > attackInterval)
        {
            rb.velocity = Vector2.zero;
            isAttacking = true;
            
            if (type == "kinoko")
            {
                KinokoAttack();
            }
            else if (type == "slime")
            {
                SlimeAttack();
            }
            else
            {
                isAttacking = false;
                attackInterval = 0;
            }
        } else
        {
            if (type == "kinoko")
            {
                rb.velocity = new Vector2(velocityX * (isRight ? 1 : -1), rb.velocity.y);
            }
            transform.localScale = new Vector2(defaultScale.x * (isRight ? -1 : 1), defaultScale.y); // デフォルトは左向き
        }
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

        // スライムは着地で攻撃終了
        if (type == "slime" && !isDead)
        {
            rb.velocity = Vector2.zero;
            isAttacking = false;
            afterAttackTime = 0;
            anim.SetBool("isCharge", false);
            anim.SetBool("isAttack", false);
        }
    }
    public void HitGroundEnd()
    {
        if (isKnockBack) // ダメージを受けて足が離れた時は判定しない
        {
            return;
        }
        if (type == "slime") // スライムは横からぶつかった時以外は反転しない
        {
            return;
        }
        if (!isFall)
        {
            isRight = !isRight;
        }
    }
    public override void HitBullet(int damage, GameObject hitObject) 
    {
        base.HitBullet(damage, hitObject);

        if (hp <= 0)
        {
            base.SetInvincible(0);
            isDead = true;
            isKnockBack = true;
            dropedItem1 = InstantiateDropItem(dropItem1, new Vector2(transform.position.x - 0.1f, transform.position.y));
            dropedItem2 = InstantiateDropItem(dropItem2, new Vector2(transform.position.x + 0.1f, transform.position.y));

            bc.enabled = false;
            bool isBulletRight= hitObject.GetComponent<Rigidbody2D>().velocity.x < 0;
            rb.velocity = new Vector2(isBulletRight ? -DAMAGE_VELOCITY_X : DAMAGE_VELOCITY_X, DAMAGE_VELOCITY_Y);
            transform.Rotate(new Vector3(0, 0, 180.0f));
        } 
        else
        {
            if (!isSuperArmor)
            {
                isKnockBack = true;
                bool isBulletRight= hitObject.GetComponent<Rigidbody2D>().velocity.x < 0; // vecolicyがマイナスなら右から左に向かっている
                rb.velocity = new Vector2(isBulletRight ? -DAMAGE_VELOCITY_X : DAMAGE_VELOCITY_X, DAMAGE_VELOCITY_Y);

                // 攻撃を中断する
                isAttacking = false;
                afterAttackTime = 0;
                anim.SetBool("isCharge", false);
                anim.SetBool("isAttack", false);
                if (attackSequence != null)
                {
                    attackSequence.Kill();
                }
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


    private void KinokoAttack()
    {
        attackSequence = DOTween.Sequence()
            .AppendCallback(() => {
                anim.SetBool("isCharge", true);
            })
            .AppendInterval(1.0f)
            .AppendCallback(() => { 
                anim.SetBool("isAttack", true);
                InstantiateKinokoBullet(60);
                InstantiateKinokoBullet(75);
                InstantiateKinokoBullet(105);
                InstantiateKinokoBullet(120);
            })
            .AppendInterval(1.0f)
            .AppendCallback(() => {
                isAttacking = false;
                afterAttackTime = 0;
                anim.SetBool("isCharge", false); 
                anim.SetBool("isAttack", false);
            }).Play();
    }
    private void InstantiateKinokoBullet(float angleZ)
    {
        float addforceX = Mathf.Cos(angleZ * Mathf.Deg2Rad) * shotPower;
        float addforceY = Mathf.Sin(angleZ * Mathf.Deg2Rad) * shotPower;

        GameObject instantiateBullet = Instantiate(bullet);
        instantiateBullet.transform.position = transform.position;
        instantiateBullet.GetComponent<Rigidbody2D>().AddForce(new Vector2(addforceX, addforceY));
    }

    private void SlimeAttack()
    {
        attackSequence = DOTween.Sequence()
            .AppendCallback(() => {
                anim.SetBool("isCharge", true);
            })
            .AppendInterval(0.5f)
            .AppendCallback(() => { 
                anim.SetBool("isAttack", true);
                rb.AddForce(new Vector2(velocityX * (isRight ? 1 : -1), velocityY));
            }).Play();
    }
}
