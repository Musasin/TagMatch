using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System; 

public class KeyConfigUI : MonoBehaviour
{
    KeyCode jumpKey;
    KeyCode shotKey;
    KeyCode switchKey;
    KeyCode menuKey;

    public enum KeyConfigState
    {
        IDLE,
        INPUT_JUMP,
        INPUT_SHOT,
        INPUT_SWITCH,
        INPUT_MENU,
    };
    public static KeyConfigState keyConfigState = KeyConfigState.IDLE;


    // Start is called before the first frame update
    void Start()
    {
        KeyConfig.Load();
        GameObject.Find("JumpKey").GetComponent<Text>().text = KeyConfig.GetTextFromKeyCode(KeyConfig.jumpKey);
        GameObject.Find("ShotKey").GetComponent<Text>().text = KeyConfig.GetTextFromKeyCode(KeyConfig.shotKey);
        GameObject.Find("SwitchKey").GetComponent<Text>().text = KeyConfig.GetTextFromKeyCode(KeyConfig.switchKey);
        GameObject.Find("MenuKey").GetComponent<Text>().text = KeyConfig.GetTextFromKeyCode(KeyConfig.menuKey);
    }

    // Update is called once per frame
    void Update()
    {
        if (keyConfigState == KeyConfigState.IDLE)
        {
            return;
        }

        if (Input.anyKeyDown)
        {
            foreach (KeyCode code in Enum.GetValues(typeof(KeyCode)))
            {
                if (Input.GetKeyDown (code)) {
                    switch (keyConfigState)
                    {
                        case KeyConfigState.INPUT_JUMP:
                            GameObject.Find("JumpKey").GetComponent<Text>().text = KeyConfig.GetTextFromKeyCode(code);
                            jumpKey = code;
                            break;
                        case KeyConfigState.INPUT_SHOT:
                            GameObject.Find("ShotKey").GetComponent<Text>().text = KeyConfig.GetTextFromKeyCode(code);
                            shotKey = code;
                            break;
                        case KeyConfigState.INPUT_SWITCH:
                            GameObject.Find("SwitchKey").GetComponent<Text>().text = KeyConfig.GetTextFromKeyCode(code);
                            switchKey = code;
                            break;
                        case KeyConfigState.INPUT_MENU:
                            GameObject.Find("MenuKey").GetComponent<Text>().text = KeyConfig.GetTextFromKeyCode(code);
                            menuKey = code;
                            SaveConfig();
                            break;
                        default:
                            break;
                    }
                    keyConfigState++;
                    break;
                }
            }
        }
    }

    public void StartConfig()
    {
        keyConfigState = KeyConfigState.INPUT_JUMP;
    }

    public void SaveConfig()
    {
        KeyConfig.SetJumpKey(jumpKey);
        KeyConfig.SetShotKey(shotKey);
        KeyConfig.SetSwitchKey(switchKey);
        KeyConfig.SetMenuKey(menuKey);
        KeyConfig.Save();
    }

    public void ResetConfig()
    {
        KeyConfig.Reset();
    }
}
