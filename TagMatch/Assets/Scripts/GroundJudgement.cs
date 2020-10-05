using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundJudgement : MonoBehaviour
{
    Enemy enemy;
    int hitObjectCount = 0;

    // Start is called before the first frame update
    void Start()
    {
        enemy = GetComponentInParent<Enemy>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Map")
        {
            hitObjectCount++;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Map")
        {
            hitObjectCount--;
        }
        if (hitObjectCount <= 0)
        {
            enemy.HitGroundEnd();
        }
    }
}
