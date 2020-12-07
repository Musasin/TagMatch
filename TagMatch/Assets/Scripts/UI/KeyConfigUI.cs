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

    GameObject cursor;
    Vector2 cursorDefaultPos;
    Text descriptionText;

    float time;
    const float OPEN_TIME = 0.5f;

    public enum KeyConfigState
    {
        IDLE,
        INPUT_JUMP,
        INPUT_SHOT,
        INPUT_SWITCH,
        INPUT_MENU,
        RESULT,
    };
    public static KeyConfigState keyConfigState = KeyConfigState.IDLE;


    // Start is called before the first frame update
    void Start()
    {
        cursor = GameObject.Find("KeyConfigCursor");
        cursorDefaultPos = cursor.transform.localPosition;
        descriptionText = GameObject.Find("DescriptionText").GetComponent<Text>();

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

        time += Time.deltaTime;
        if (time < OPEN_TIME)
        {
            return;
        }
        
        cursor.transform.localPosition = new Vector2(cursorDefaultPos.x, cursorDefaultPos.y - (75 * ((int)keyConfigState - 1)));
        switch (keyConfigState)
        {
            case KeyConfigState.INPUT_JUMP:
                descriptionText.text = "ジャンプ / 決定 に\n使用するキーを押してください";
                break;
            case KeyConfigState.INPUT_SHOT:
                descriptionText.text = "ショット / キャンセル に\n使用するキーを押してください";
                break;
            case KeyConfigState.INPUT_SWITCH:
                descriptionText.text = "スイッチ に\n使用するキーを押してください";
                break;
            case KeyConfigState.INPUT_MENU:
                descriptionText.text = "メニュー に\n使用するキーを押してください";
                break;
            case KeyConfigState.RESULT:
                descriptionText.text = "変更を保存する場合は決定キーを、\n破棄する場合はキャンセルキーを押してください";
                break;
            default:
                break;
        }

        if (Input.anyKeyDown)
        {
            Debug.Log("SetKey");
            SetKey();
        }
    }

    void SetKey()
    {
        foreach (KeyCode code in Enum.GetValues(typeof(KeyCode)))
        {
            if (Input.GetKeyDown (code)) {
                switch (keyConfigState)
                {
                    case KeyConfigState.INPUT_JUMP:
                        GameObject.Find("JumpKey").GetComponent<Text>().text = KeyConfig.GetTextFromKeyCode(code);
                        jumpKey = code;
                        keyConfigState++;
                        return;
                    case KeyConfigState.INPUT_SHOT:
                        if (code == jumpKey) return;
                        GameObject.Find("ShotKey").GetComponent<Text>().text = KeyConfig.GetTextFromKeyCode(code);
                        shotKey = code;
                        keyConfigState++;
                        return;
                    case KeyConfigState.INPUT_SWITCH:
                        if (code == jumpKey || code == shotKey) return;
                        GameObject.Find("SwitchKey").GetComponent<Text>().text = KeyConfig.GetTextFromKeyCode(code);
                        switchKey = code;
                        keyConfigState++;
                        return;
                    case KeyConfigState.INPUT_MENU:
                        if (code == jumpKey || code == shotKey || code == switchKey) return;
                        GameObject.Find("MenuKey").GetComponent<Text>().text = KeyConfig.GetTextFromKeyCode(code);
                        menuKey = code;
                        keyConfigState++;
                        return;
                    case KeyConfigState.RESULT:
                        if (code == jumpKey)
                        {
                            SaveConfig();
                            keyConfigState = KeyConfigState.IDLE;
                        } 
                        else if (code == shotKey)
                        {
                            GameObject.Find("JumpKey").GetComponent<Text>().text = KeyConfig.GetTextFromKeyCode(KeyConfig.jumpKey);
                            GameObject.Find("ShotKey").GetComponent<Text>().text = KeyConfig.GetTextFromKeyCode(KeyConfig.shotKey);
                            GameObject.Find("SwitchKey").GetComponent<Text>().text = KeyConfig.GetTextFromKeyCode(KeyConfig.switchKey);
                            GameObject.Find("MenuKey").GetComponent<Text>().text = KeyConfig.GetTextFromKeyCode(KeyConfig.menuKey);
                            keyConfigState = KeyConfigState.IDLE;
                        }
                        return;
                }
            }
        }
    }

    public KeyConfigState GetState()
    {
        return keyConfigState;
    }

    public void StartConfig()
    {
        time = 0;
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
