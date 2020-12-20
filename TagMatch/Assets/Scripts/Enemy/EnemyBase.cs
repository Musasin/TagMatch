using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBase : MonoBehaviour
{
    public int hp;
    public GameObject damagePointEffect;

    const float INVINCIBLE_TIME = 0.2f;
    
    int maxHp;
    float invincibleTime = 0;
    SpriteRenderer sr;
    Vector2 defaultPosition;
    Vector2 defaultScale;

    // Start is called before the first frame update
    public virtual void Start()
    {
        maxHp = hp;
        sr = GetComponentInChildren<SpriteRenderer>();
        defaultPosition = transform.localPosition;
        defaultScale = transform.localScale;
    }

    public virtual void Reset()
    {
        hp = maxHp;
        invincibleTime = 0;
        transform.localPosition = defaultPosition;
        transform.localScale = defaultScale;
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
        float f = (invincibleTime > 0) ? 0.5f : 1.0f;
        sr.color = new Color(1.0f, f, f, f);
    }

    public virtual void HitBullet(int damage, GameObject hitObject)
    {
        if (invincibleTime > 0)
        {
            return;
        }

        hp -= damage;
        GameObject dp = Instantiate(damagePointEffect);
        dp.transform.position = transform.position;
        dp.GetComponent<DamagePointEffect>().SetDamagePointAndPlay(damage);
        invincibleTime = INVINCIBLE_TIME;
        AudioManager.Instance.PlaySE("hit_enemy");
    }
    public bool IsInvincible()
    {
        return (invincibleTime > 0);
    }
}
