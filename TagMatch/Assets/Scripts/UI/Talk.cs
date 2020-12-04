using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Talk: MonoBehaviour
{
    public GameObject yukari, maki, kiritan;
    public GameObject leftWindow, rightWindow;

    // Start is called before the first frame update
    void Start()
    {
        YukariInstantiate();
        MakiInstantiate();
        //kiritan = Instantiate(kiritan, transform);
        //Instantiate(leftWindow, transform);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void YukariInstantiate()
    {
        yukari = Instantiate(yukari, transform);
    }
    public void MakiInstantiate()
    {
        maki = Instantiate(maki, transform);
    }
    //public void InstantiateCharacter(GameObject obj)
    //{
    //    Instantiate(obj, transform);
    //}
    public void SwitchYukaMaki()
    {
        Animator makiAnim = maki.GetComponent<Animator>();
        makiAnim.SetBool("isUp", !makiAnim.GetBool("isUp"));
        Animator yukariAnim = yukari.GetComponent<Animator>();
        yukariAnim.SetBool("isUp", !yukariAnim.GetBool("isUp"));
    }
    public void AddTalk()
    {

    }
}
