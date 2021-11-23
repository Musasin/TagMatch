using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamagePointEffect : MonoBehaviour
{
    float time;
    const float MINIMUM_TIME = 0.2f;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;
        DestroyWhenOffScreen();
    }

    public void SetDamagePointAndPlay(int damage)
    {
        int absDamage = Mathf.Abs(damage);
        if (absDamage < 10)
        {
            transform.Find("FirstPlace").gameObject.GetComponent<DamagePointNumber>().SetNumber(absDamage);
            transform.Find("TenthPlace").gameObject.SetActive(false);
        }
        else
        {
            transform.Find("FirstPlace").gameObject.GetComponent<DamagePointNumber>().SetNumber(absDamage % 10);
            transform.Find("TenthPlace").gameObject.GetComponent<DamagePointNumber>().SetNumber(absDamage / 10);
        }
        GetComponent<Rigidbody2D>().velocity = new Vector2(Random.Range(-2.0f, 2.0f), 15.0f);
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
        Destroy(gameObject);
    }
}
