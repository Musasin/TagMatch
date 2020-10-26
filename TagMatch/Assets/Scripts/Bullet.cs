using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int damage;
    public GameObject deadEffect;
    public bool isTrample;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }
    
    private void Dead()
    {
        GameObject effect = Instantiate(deadEffect);
        effect.transform.position = transform.position;
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Map")
        {
            if (!isTrample)
            {
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
                    enemy.HitBullet(damage);
                    if (!isTrample)
                    {
                        Dead();
                    }
                }
            }
        }
    }
}
