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
    public bool isUp;
    public GameObject bullet;
    public GameObject dropItem1;
    public GameObject dropItem2;
    public GameObject skeletonBullet;
    
    const float STOP_TIME = 0.8f;
    const float DAMAGE_VELOCITY_X = 4.0f;
    const float DAMAGE_VELOCITY_Y = 8.0f;

    float stopTime = 0;
    float afterAttackTime = 0;
    bool isKnockBack;
    bool isAttacking;
    bool isDead;
    Rigidbody2D rb;
    Animator anim;
    BoxCollider2D bc;
    BoxCollider2D damageCollider;
    Vector2 defaultScale;
    GameObject dropedItem1, dropedItem2;
    Sequence attackSequence;
    GameObject playerObject;
    GameObject bulletPivot;

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<Animator>();
        bc = transform.GetComponent<BoxCollider2D>();
        damageCollider = transform.Find("Damage").gameObject.GetComponent<BoxCollider2D>();
        defaultScale = transform.localScale;
        playerObject = GameObject.Find("Player");
        bulletPivot = transform.Find("BulletPivot")?.gameObject;
    }

    public override void Reset()
    {
        base.Reset();
        attackSequence.Kill();
        anim.SetBool("isCharge", false);
        anim.SetBool("isAttack", false);
        isKnockBack = false;
        isAttacking = false;
        isDead = false;
        afterAttackTime = 0;
        if (dropedItem1 != null) Destroy(dropedItem1);
        if (dropedItem2 != null) Destroy(dropedItem2);
        bc.enabled = true;
        damageCollider.enabled = true;
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
            if (type != "bat" && type != "move_bat")
            {
                return;
            }

            stopTime -= Time.deltaTime;
            if (stopTime >= 0)
            {
                return;
            }
            isKnockBack = false;
            anim.SetBool("isKnockBack", isKnockBack);
        }

        if (isAttacking)
        {
            return;
        }
        
        afterAttackTime += Time.deltaTime;
        switch (type)
        {
            case "kinoko":
                if (attackInterval != 0 && afterAttackTime > attackInterval)
                {
                    rb.velocity = Vector2.zero;
                    isAttacking = true;
                    KinokoAttack();
                } else
                {
                    rb.velocity = new Vector2(velocityX * (isRight ? 1 : -1), rb.velocity.y);
                }
                break;
            case "slime":
                // スライムは攻撃中以外静止
                rb.velocity = Vector2.zero;
                if (attackInterval != 0 && afterAttackTime > attackInterval)
                {
                    isAttacking = true;
                    SlimeAttack();
                }
                break;
            case "bat":
                Vector2 playerPos = playerObject.transform.position;
                float playerDistance = Vector2.Distance(playerPos, transform.position);
                float defaultPosDistance = Vector2.Distance(base.defaultPosition, transform.position);
                if (playerDistance < attackInterval)
                {
                    MoveToPos(playerPos);
                    anim.SetBool("isAttack", true);
                }
                else if (defaultPosDistance > 0.2f) // 初期位置からある程度離れていたら戻る
                {
                    MoveToPos(base.defaultPosition);
                    anim.SetBool("isAttack", true);
                } else
                {
                    rb.velocity = Vector2.zero;
                    anim.SetBool("isAttack", false);
                }
                break;
            case "move_bat":
                anim.SetBool("isAttack", true);
                rb.velocity = new Vector2(velocityX * (isRight ? 1 : -1), velocityY * (isUp ? 1 : -1));
                break;
            case "skeleton":
                if (attackInterval != 0 && afterAttackTime > attackInterval)
                {
                    rb.velocity = Vector2.zero;
                    isAttacking = true;
                    SkeletonAttack();
                } else
                {
                    rb.velocity = new Vector2(velocityX * (isRight ? 1 : -1), rb.velocity.y);
                }
                break;

        }
        
        // 向きの更新 デフォルトは左向き
        transform.localScale = new Vector2(defaultScale.x * (isRight ? -1 : 1), defaultScale.y);
    }
    
    public void HitWall()
    {
        // 向いてる方向と進んでいる方向が一致している時のみ方向転換する (ノックバックで背中から当たる場合があるため)
        if (rb.velocity.x > 0 && isRight || rb.velocity.x < 0 && !isRight)
        {
            isRight = !isRight;
            // スライムは壁接触で加速度を反転
            if (type == "slyme")
            {
                rb.velocity = new Vector2(-rb.velocity.x, rb.velocity.y);
            }
        }
        if (rb.velocity.y > 0 && isUp || rb.velocity.y < 0 && !isUp)
        {
            isUp = !isUp;
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
            damageCollider.enabled = false;
            bool isBulletRight= hitObject.GetComponent<Rigidbody2D>().velocity.x < 0;
            rb.velocity = new Vector2(isBulletRight ? -DAMAGE_VELOCITY_X : DAMAGE_VELOCITY_X, DAMAGE_VELOCITY_Y);
            transform.Rotate(new Vector3(0, 0, 180.0f));
            if (attackSequence != null)
            {
                attackSequence.Kill();
            }
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

        // コウモリだけ着地が存在しないため、ノックバックを時間管理する
        if (type == "bat" || type == "move_bat")
        {
            stopTime = STOP_TIME;
            if (isDead)
            {
                rb.velocity = new Vector2(0, -8);
            } else { 
                rb.velocity = Vector2.zero;
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

        GameObject instantiateBullet = Instantiate(bullet, transform.parent);
        instantiateBullet.transform.position = transform.position;
        instantiateBullet.GetComponent<Rigidbody2D>().AddForce(new Vector2(addforceX, addforceY));
    }
    
    private void SkeletonAttack()
    {
        attackSequence = DOTween.Sequence()
            .AppendCallback(() => {
                anim.SetBool("isCharge", true);
            })
            .AppendInterval(0.3f)
            .AppendCallback(() => { 
                anim.SetBool("isAttack", true);
                InstantiateSkeletonBullet();
            })
            .AppendInterval(0.3f)
            .AppendCallback(() => {
                isAttacking = false;
                afterAttackTime = 0;
                anim.SetBool("isCharge", false); 
                anim.SetBool("isAttack", false);
            }).Play();
    }
    private void InstantiateSkeletonBullet()
    {
        float addforceX = isRight ? shotPower : -shotPower;
        
        GameObject instantiateBullet = Instantiate(skeletonBullet, transform.parent);
        instantiateBullet.transform.position = bulletPivot.transform.position;
        instantiateBullet.transform.localScale = new Vector3(isRight ? -1 : 1, 1, 1);
        instantiateBullet.GetComponent<Rigidbody2D>().AddForce(new Vector2(addforceX, 0));
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

    private void MoveToPos(Vector2 targetPos)
    {
        float x, y;
        if (Mathf.Abs(targetPos.x - transform.position.x) < 0.1f)
        {
            x = 0;
        }
        else if (targetPos.x < transform.position.x)
        {
            x = -velocityX;
            isRight = false;
        }
        else
        {
            x = velocityX;
            isRight = true;
        }
        if (Mathf.Abs(targetPos.y - transform.position.y) < 0.05f)
            y = 0;
        else if (targetPos.y < transform.position.y)
            y = -velocityY;
        else
            y = velocityY;

        rb.velocity = new Vector2(x, y);
    }
}
