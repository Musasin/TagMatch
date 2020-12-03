using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Boss : EnemyBase
{
    Vector2 firstPos;
    bool isDead;

    public override void Start()
    {
        base.Start();
        firstPos = transform.position;
    }

    public override void HitBullet(int damage, GameObject hitObject) 
    {
        base.HitBullet(damage, hitObject);

        if (hp <= 0)
        {
            isDead = true;
            transform.DOLocalJump(firstPos, 3, 1, 1.0f);
            transform.DOLocalRotate(new Vector3(0, 0, 720), 1.0f, RotateMode.FastBeyond360);
        } 
    }

    public bool IsDead()
    {
        return isDead;
    }
}
