using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideSpriteRenderer : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<SpriteRenderer>().color = new Vector4(0, 0, 0, 0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
