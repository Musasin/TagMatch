using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TutorialBoard : MonoBehaviour
{
    TextMeshProUGUI text;
    public enum KeyType
    {
        JUMP, SHOT, DASH, MENU, SWITCH, GET_OFF
    }
    public KeyType keyType;
    public string backText;

    // Start is called before the first frame update
    void Start()
    {
        string str = "";
        text = GetComponent<TextMeshProUGUI>();
        switch (keyType)
        {
            case KeyType.JUMP:
                str = "[" + KeyConfig.GetTextFromKeyCode(KeyConfig.jumpKey) + "]";
                break;
            case KeyType.SHOT:
                str = "[" + KeyConfig.GetTextFromKeyCode(KeyConfig.shotKey) + "]";
                break;
            case KeyType.DASH:
                str = "[" + KeyConfig.GetTextFromKeyCode(KeyConfig.jumpKey) + "]→[" + KeyConfig.GetTextFromKeyCode(KeyConfig.jumpKey) + "]";
                break;
            case KeyType.MENU:
                str = "[" + KeyConfig.GetTextFromKeyCode(KeyConfig.menuKey) + "]";
                break;
            case KeyType.SWITCH:
                str = "[" + KeyConfig.GetTextFromKeyCode(KeyConfig.switchKey) + "]";
                break;
            case KeyType.GET_OFF:
                str = "↓+[" + KeyConfig.GetTextFromKeyCode(KeyConfig.jumpKey) + "]";
                break;
        }
        text.text = str + "\n" + backText;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
