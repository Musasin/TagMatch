using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallJudgement : MonoBehaviour
{
    Enemy enemy;

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
            enemy.HitWall();
        }
    }
}
