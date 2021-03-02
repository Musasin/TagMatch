using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeadIconUI : MonoBehaviour
{
    public bool isYukari;
    public bool isMaki;

    Image image;

    // Start is called before the first frame update
    void Start()
    {
        image = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isYukari)
        {
            if (StaticValues.yukariHP <= 0 || StaticValues.switchState == StaticValues.SwitchState.MAKI_ONLY)
            {
                image.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
            } 
            else
            {
                image.color = new Color(1.0f, 1.0f, 1.0f, 0f);
            }
        }
        else if (isMaki)
        {
            if (StaticValues.makiHP <= 0 || StaticValues.switchState == StaticValues.SwitchState.YUKARI_ONLY)
            {
                image.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
            } 
            else
            {
                image.color = new Color(1.0f, 1.0f, 1.0f, 0f);
            }
        }
    }
}
