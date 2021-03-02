using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyWhenOffScreen : MonoBehaviour
{
    float time = 0;
    public float minimumTime = 0.1f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;
        if (time < minimumTime)
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
