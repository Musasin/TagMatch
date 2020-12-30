using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootJudgement : MonoBehaviour
{
    Rigidbody2D rb;
    int hitObjectCount = 0;

    // Start is called before the first frame update
    void Start()
    {
        rb = transform.parent.GetComponent<Rigidbody2D>();
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
    }

    public bool GetIsLanding()
    {
        return hitObjectCount > 0;
    }
}
