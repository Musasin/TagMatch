using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour
{
    public int hp;
    public GameObject damagePointEffect;

    SpriteRenderer sr;
    const float INVINCIBLE_TIME = 0.2f;

    float invincibleTime = 0;

    // Start is called before the first frame update
    void Start()
    {
        sr = GetComponentInChildren<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        invincibleTime -= Time.deltaTime;
        UpdateColor();
    }
    
    private void UpdateColor()
    {
        float f = (invincibleTime > 0) ? 0.5f : 1.0f;
        sr.color = new Color(1.0f, f, f, f);
    }

    public void HitBullet(int damage, GameObject hitObject)
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
    }
    public bool IsInvincible()
    {
        return (invincibleTime > 0);
    }
}
