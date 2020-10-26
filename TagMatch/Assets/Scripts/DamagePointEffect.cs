using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamagePointEffect : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetDamagePointAndPlay(int damage)
    {
        if (damage < 10)
        {
            transform.Find("FirstPlace").gameObject.GetComponent<DamagePointNumber>().SetNumber(damage);
            transform.Find("TenthPlace").gameObject.SetActive(false);
        }
        else
        {
            transform.Find("FirstPlace").gameObject.GetComponent<DamagePointNumber>().SetNumber(damage % 10);
            transform.Find("TenthPlace").gameObject.GetComponent<DamagePointNumber>().SetNumber(damage / 10);
        }
        GetComponent<Rigidbody2D>().velocity = new Vector2(Random.Range(-2.0f, 2.0f), 15.0f);
    }
}
