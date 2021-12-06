using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallBullet : MonoBehaviour
{
    public float moveX;
    public float moveY;
    public float moveTime;
    public float deadTime;
    float time;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;

        if (time < moveTime)
        {
            var posX = transform.localPosition.x + moveX * Time.deltaTime;
            var posY = transform.localPosition.y + moveY * Time.deltaTime;
            transform.localPosition = new Vector2(posX, posY);
        }

        if (time > deadTime)
        {
            var scaleX = transform.localScale.x - 2 * Time.deltaTime;
            transform.localScale = new Vector2(scaleX, transform.localScale.y);

            if (scaleX <= 0)
            {
                Destroy(gameObject);
            }
        }
    }
}
