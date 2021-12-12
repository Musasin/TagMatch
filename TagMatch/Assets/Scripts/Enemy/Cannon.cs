using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Linq;

public class Cannon : MonoBehaviour
{
    // 青:平均的, 緑:ふんわり落ちてくる, 赤:ダメージ1.5倍, 紫:落下が速い
    public GameObject bulletBlue, bulletRed, bulletGreen, bulletPurple;

    public enum CannonType { RIGHT, LEFT, TOP }
    public CannonType cannonType;

    GameObject bulletPivot;
    Animator anim;
    Vector2 firstPos;
    bool isIn;
    bool isCharging;
    float chargeTime;
    float maxChargeTime;
    float intervalTime;
    float maxIntervalTime;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponentInChildren<Animator>();
        bulletPivot = transform.Find("CannonPivot").gameObject;
        firstPos = transform.position;
        maxChargeTime = 1.0f;
        maxIntervalTime = Random.Range(5.0f, 10.0f);
        
        if (cannonType == CannonType.TOP)
        {
            DOTween.Sequence()
                .Append(transform.DOMove(new Vector2(firstPos.x + 16, firstPos.y), 3.0f))
                .SetLoops(-1, LoopType.Yoyo)
                .Play();
        }
    }

    public void Reset()
    {
        anim.SetTrigger("reset");
        transform.position = firstPos;
        isCharging = false;
        maxChargeTime = 1.0f;
        maxIntervalTime = Random.Range(5.0f, 10.0f);
    }

    // Update is called once per frame
    void Update()
    {
        if (!isIn) return;
        if (StaticValues.isPause || StaticValues.isTalkPause) return;

        if (isCharging)
        {
            chargeTime += Time.deltaTime;
            if (chargeTime > maxChargeTime)
            {
                Shot();
                isCharging = false;
                chargeTime = 0;
            }
        } else
        {
            intervalTime += Time.deltaTime;
            if (intervalTime > maxIntervalTime)
            {
                anim.SetTrigger("charge");
                isCharging = true;
                intervalTime = 0;
                maxIntervalTime = Random.Range(5.0f, 10.0f);
            }
        }
    }

    public void In()
    {
        anim.SetTrigger("in");
        isIn = true;
    }

    private void Shot()
    {
        GameObject bullet = null;
        Vector2 addForceVector = Vector2.zero;
        
        switch (Random.Range(0, 3))
        {
            case 0:
                bullet = Instantiate(bulletBlue);
                break;
            case 1:
                bullet = Instantiate(bulletRed);
                break;
            case 2:
                bullet = Instantiate(bulletPurple);
                break;
            case 3:
                bullet = Instantiate(bulletGreen);
                break;
        }
        
        switch (cannonType)
        {
            case CannonType.RIGHT:
                addForceVector = CalcAddForceVector(135, Random.Range(500, 1000));
                break;

            case CannonType.LEFT:
                addForceVector = CalcAddForceVector(45, Random.Range(500, 1000));
                break;

            case CannonType.TOP:
                addForceVector = CalcAddForceVector(-90, Random.Range(500, 1000));
                break;
        }

        bullet.transform.position = bulletPivot.transform.position;
        bullet.GetComponent<Rigidbody2D>().AddForce(addForceVector);
        anim.SetTrigger("shot");
    }
    
    Vector2 CalcAddForceVector(float angleZ, float power)
    {
        float addforceX = Mathf.Cos(angleZ * Mathf.Deg2Rad) * power;
        float addforceY = Mathf.Sin(angleZ * Mathf.Deg2Rad) * power;
        return new Vector2(addforceX, addforceY);
    }
}
