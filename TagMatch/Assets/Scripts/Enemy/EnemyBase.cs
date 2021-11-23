using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBase : MonoBehaviour
{
    public int hp;
    public GameObject damagePointEffect, healPointEffect;
    public Vector2 defaultPosition;

    const float INVINCIBLE_TIME = 0.2f;
    
    int maxHp;
    float invincibleTime = 0;
    SpriteRenderer sr;

    Color defaultColor;

    // Start is called before the first frame update
    public virtual void Start()
    {
        maxHp = hp;
        sr = GetComponentInChildren<SpriteRenderer>();
        defaultPosition = transform.localPosition;
        defaultColor = sr.color;
    }

    public virtual void Reset()
    {
        hp = maxHp;
        invincibleTime = 0;
        transform.localPosition = defaultPosition;
        transform.rotation = new Quaternion(0, 0, 0, 0);
    }

    // Update is called once per frame
    public virtual void Update()
    {
        invincibleTime -= Time.deltaTime;
        UpdateColor();
    }
    
    public void UpdateColor()
    {
        if (invincibleTime > 0) sr.color = new Color(1.0f, 0.5f, 0.5f, 0.5f);
        else sr.color = defaultColor;
    }

    public virtual void HitBullet(int damage, GameObject hitObject, bool ignoreInvincible = false)
    {
        if (invincibleTime > 0 && !ignoreInvincible)
        {
            return;
        }

        hp = Mathf.Min(hp - damage, maxHp);
        GameObject dp = damage < 0 ? Instantiate(healPointEffect) : Instantiate(damagePointEffect);
        dp.transform.position = transform.position;
        dp.GetComponent<DamagePointEffect>().SetDamagePointAndPlay(damage);
        if (damage >= 0)
        {
            AudioManager.Instance.PlaySE("hit_enemy");
        }
        if (!ignoreInvincible)
        {
            invincibleTime = INVINCIBLE_TIME;
        }
    }
    public bool IsInvincible()
    {
        return (invincibleTime > 0);
    }

    public void SetInvincible(float time)
    {
        invincibleTime = time;
    }
}
