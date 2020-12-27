using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Linq;

public class Boss : EnemyBase
{
    public int bossNumber = 0;
    const float BOSS_INVINCIBLE_TIME = 0.5f;

    Vector2 firstPos;
    bool isDead;
    bool isActive = false;

    public override void Start()
    {
        base.Start();
        firstPos = transform.position;

        // TODO 今のままだと、複数ボスのステージをクリア後、ボス1体のステージに行くと2体目のMaxHPがそのまま残ってしまうので、シーン読み込み時等に初期化する処理を入れる
        StaticValues.bossMaxHP[bossNumber] = hp;
        StaticValues.bossHP[bossNumber] = hp;
    }

    public override void Update()
    {
        base.Update();
        if (!isDead && StaticValues.bossHP.Sum() <= 0)
        {
            isDead = true;
            transform.DOLocalJump(firstPos, 3, 1, 1.0f);
            transform.DOLocalRotate(new Vector3(0, 0, 720), 1.0f, RotateMode.FastBeyond360);
        }
    }

    public override void HitBullet(int damage, GameObject hitObject) 
    {
        base.HitBullet(damage, hitObject);
        StaticValues.bossHP[bossNumber] = hp;
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
