using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SupportBullet : MonoBehaviour
{
    public int damage;
    float time;
    float deadTime;
    const float MINIMUM_TIME = 0.1f;
    Player player;
    bool isDead;


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;
        if (deadTime != 0 && time > deadTime)
        {
            Dead();
        }
    }

    
    private void Dead()
    {
        if (isDead) return;
        isDead = true;
        Destroy(gameObject);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag != "Enemy") return;
            
        EnemyBase enemy = collision.gameObject.GetComponent<EnemyBase>();
        if (enemy != null)
        {
            int dmg = damage;
            enemy.HitBullet(dmg, gameObject, true);
            GetComponent<BoxCollider2D>().enabled = false; // サポートキャラの弾は消さずに当たり判定だけ無効化する
        }
    }

    public void SetDeadTime(float dt)
    {
        deadTime = dt;
    }
}
