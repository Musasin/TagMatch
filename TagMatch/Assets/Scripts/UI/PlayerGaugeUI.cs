using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGaugeUI : MonoBehaviour
{
    Animator anim;
    GameObject yukariGauge;
    GameObject makiGauge;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        yukariGauge = GameObject.Find("YukariGauge");
        makiGauge = GameObject.Find("MakiGauge");
    }

    // Update is called once per frame
    void Update()
    {
        anim.SetInteger("switchState", (int)StaticValues.switchState);
        
        switch (StaticValues.switchState)
        {
            case StaticValues.SwitchState.YUKARI:
            case StaticValues.SwitchState.MAKI:
                yukariGauge.SetActive(true);
                makiGauge.SetActive(true);
                break;
            case StaticValues.SwitchState.YUKARI_ONLY:
                yukariGauge.SetActive(true);
                makiGauge.SetActive(false);
                break;
            case StaticValues.SwitchState.MAKI_ONLY:
                yukariGauge.SetActive(false);
                makiGauge.SetActive(true);
                break;
        }
    }
}
