﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int damage;
    public GameObject hitEffect;
    public bool isTrample;
    float time;
    float deadTime;
    const float MINIMUM_TIME = 0.1f;
    Player player;

    public enum BulletType
    {
        INVALID = 0, 
        YUKARI, 
        MAKI,
        MAKI_BARRIER,
        MAKI_ELECTRIC_FIRE,
    }
    BulletType bulletType;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;
        DestroyWhenOffScreen();
        if (deadTime != 0 && time > deadTime)
        {
            Dead();
        }
    }

    private void DestroyWhenOffScreen()
    {
        if (time < MINIMUM_TIME)
            return;

        SpriteRenderer[] spriteList = GetComponentsInChildren<SpriteRenderer>();
        foreach (SpriteRenderer sprite in spriteList)
        {
            if (sprite.isVisible)
                return;
        }
        Dead();
    }

    private void PlayHitEffect(Vector2 pos)
    {
        GameObject effect = Instantiate(hitEffect);
        effect.transform.position = pos;
    }
    
    private void Dead()
    {
        player.AddBulletCount(bulletType, -1);
        Destroy(gameObject);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Map")
        {
            if (!isTrample)
            {
                PlayHitEffect(transform.position);
                Dead();
            }
        }
        else if (collision.gameObject.tag == "Enemy")
        {
            Enemy enemy = collision.gameObject.GetComponent<Enemy>();
            if (enemy != null)
            {
                if (!enemy.IsInvincible())
                {
                    enemy.HitBullet(damage, gameObject);
                    PlayHitEffect(collision.gameObject.transform.position);
                    if (!isTrample)
                    {
                        Dead();
                    }
                }
            }
            Boss boss = collision.gameObject.GetComponent<Boss>();
            if (boss != null)
            {
                if (!boss.IsInvincible())
                {
                    boss.HitBullet(damage, gameObject);
                    PlayHitEffect(collision.gameObject.transform.position);
                    if (!isTrample)
                    {
                        Dead();
                    }
                }
            }
        } 
        else if (collision.gameObject.tag == "Item")
        {
            Coin coin = collision.gameObject.GetComponent<Coin>();
            if (coin != null)
            {
                coin.GetCoin();
            }
        }
    }

    public void SetPlayerScript(Player p)
    {
        player = p;
    }
    public void SetBulletType(BulletType type) {
        bulletType = type;    
    }
    public void SetDeadTime(float dt)
    {
        deadTime = dt;
    }
}
