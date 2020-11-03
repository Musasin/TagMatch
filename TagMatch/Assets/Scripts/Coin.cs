using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    public int point;
    public int score;

    Animator anim;
    bool isGot;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GetCoin()
    {
        if (isGot)
            return;

        isGot = true;
        anim.SetBool("isGot", isGot);
        StaticValues.score += score;
        StaticValues.coinCount += point;
    }
}
