using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamagePointNumber : MonoBehaviour
{
    public Sprite d0, d1, d2, d3, d4, d5, d6, d7, d8, d9;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void SetNumber(int num)
    {
        switch (num)
        {
            case 0:
                GetComponent<SpriteRenderer>().sprite = d0;
                break;
            case 1:
                GetComponent<SpriteRenderer>().sprite = d1;
                break;
            case 2:
                GetComponent<SpriteRenderer>().sprite = d2;
                break;
            case 3:
                GetComponent<SpriteRenderer>().sprite = d3;
                break;
            case 4:
                GetComponent<SpriteRenderer>().sprite = d4;
                break;
            case 5:
                GetComponent<SpriteRenderer>().sprite = d5;
                break;
            case 6:
                GetComponent<SpriteRenderer>().sprite = d6;
                break;
            case 7:
                GetComponent<SpriteRenderer>().sprite = d7;
                break;
            case 8:
                GetComponent<SpriteRenderer>().sprite = d8;
                break;
            case 9:
                GetComponent<SpriteRenderer>().sprite = d9;
                break;
        }
    }
}
