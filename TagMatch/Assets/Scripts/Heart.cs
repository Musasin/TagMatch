using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Heart : MonoBehaviour
{
    public int hpPoint;
    public int mpPoint;
    public bool isGot;
    
    Animator anim;
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
        
        if (hpPoint >= 50 || mpPoint >= 50)
        {
            AudioManager.Instance.PlaySE("heal_big");
        } else
        {
            AudioManager.Instance.PlaySE("heal");
        }
        isGot = true;
        anim.SetBool("isGot", isGot);
        StaticValues.AddHP(true, hpPoint);
        StaticValues.AddHP(false, hpPoint);
        StaticValues.AddMP(true, mpPoint);
        StaticValues.AddMP(false, mpPoint);
    }
}
