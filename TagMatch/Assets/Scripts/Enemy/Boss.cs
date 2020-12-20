using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Boss : EnemyBase
{

    const float BOSS_INVINCIBLE_TIME = 0.5f;

    Vector2 firstPos;
    bool isDead;
    bool isActive = false;

    public override void Start()
    {
        base.Start();
        firstPos = transform.position;
        StaticValues.bossMaxHP = hp;
        StaticValues.bossHP = hp;
    }

    public override void HitBullet(int damage, GameObject hitObject) 
    {
        base.HitBullet(damage, hitObject);
        StaticValues.bossHP = hp;

        if (hp <= 0)
        {
            isDead = true;
            transform.DOLocalJump(firstPos, 3, 1, 1.0f);
            transform.DOLocalRotate(new Vector3(0, 0, 720), 1.0f, RotateMode.FastBeyond360);
        }
        base.SetInvincible(BOSS_INVINCIBLE_TIME);
    }

    public void SetActive(bool flag)
    {
        isActive = flag;
    }
    public bool IsActive()
    {
        // 仮
        return StaticValues.isFixedCamera && !StaticValues.isPause && !StaticValues.isTalkPause;
        //return isActive;
    }
    public bool IsDead()
    {
        return isDead;
    }
}
