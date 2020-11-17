using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Heart : MonoBehaviour
{
    public int hpPoint;
    public int mpPoint;
    
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

    public void GetHeart()
    {
        if (isGot)
            return;

        isGot = true;
        anim.SetBool("isGot", isGot);
        StaticValues.AddHP(true, hpPoint);
        StaticValues.AddHP(false, hpPoint);
        StaticValues.AddMP(true, mpPoint);
        StaticValues.AddMP(false, mpPoint);
    }
}
