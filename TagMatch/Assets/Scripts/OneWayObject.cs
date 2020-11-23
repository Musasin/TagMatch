using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OneWayObject : MonoBehaviour
{
    bool isIgnore;

    // Start is called before the first frame update
    void Start()
    {
        isIgnore = false;
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        switch (collision.gameObject.tag)
        {
            case "Player":
                isIgnore = true;
                Physics2D.IgnoreCollision(collision, GetComponent<Collider2D>(), isIgnore);
                break;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        switch (collision.gameObject.tag)
        {
            case "Player":
                isIgnore = false;
                Physics2D.IgnoreCollision(collision, GetComponent<Collider2D>(), isIgnore);
                break;
        }
    }

    public bool GetIsIgnore()
    {
        return isIgnore;
    }
}