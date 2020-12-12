using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TutorialBoard : MonoBehaviour
{
    TextMeshProUGUI text;
    public enum KeyType
    {
        JUMP, SHOT, DASH
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
        }
        text.text = str + "\n" + backText;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
