using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyConfig : MonoBehaviour
{
    public static KeyCode jumpKey;
    public static KeyCode shotKey;
    public static KeyCode switchKey;
    public static KeyCode menuKey;


    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }

    public static void LoadKeyConfig()
    {
        LoadKey(ref jumpKey, "JumpKey", KeyCode.C);
        LoadKey(ref shotKey, "ShotKey", KeyCode.X);
        LoadKey(ref switchKey, "SwitchKey", KeyCode.Z);
        LoadKey(ref menuKey, "MenuKey", KeyCode.Escape);
    }

    public static void LoadKey(ref KeyCode key, string prefs, KeyCode defaultCode)
    {
        if (PlayerPrefs.HasKey(prefs))
        {
            key = (KeyCode)PlayerPrefs.GetInt(prefs);
        }
        else
        {
            key = defaultCode;
        }
    }

    public static void SaveKeyConfig()
    {
        PlayerPrefs.SetInt("JumpKey", (int)jumpKey);
        PlayerPrefs.SetInt("ShotKey", (int)shotKey);
        PlayerPrefs.SetInt("SwitchKey", (int)switchKey);
        PlayerPrefs.SetInt("MenuKey", (int)menuKey);
    }

    public static void SetJumpKey(KeyCode code)
    {
        jumpKey = code;
    }
    public static void SetShotKey(KeyCode code)
    {
        shotKey = code;
    }
    public static void SetSwitchKey(KeyCode code)
    {
        switchKey = code;
    }
    public static void SetMenuKey(KeyCode code)
    {
        menuKey = code;
    }
    
    public static bool GetJumpKeyDown()
    {
        return Input.GetKeyDown(jumpKey);
    }
    public static bool GetShotKeyDown()
    {
        return Input.GetKeyDown(shotKey);
    }
    public static bool GetSwitchKeyDown()
    {
        return Input.GetKeyDown(switchKey);
    }
    public static bool GetMenuKeyDown()
    {
        return Input.GetKeyDown(menuKey);
    }
}
