using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System;


public class StaticValues : MonoBehaviour
{
    public static float time;
    public static int score;
    public static int coinCount = 2000; // デバッグ用
    public static int deadCount;

    public enum SwitchState { YUKARI = 0, YUKARI_ONLY = 1, MAKI = 2, MAKI_ONLY = 3};
    public static SwitchState switchState = SwitchState.YUKARI;
    
    public static bool isPause;
    public static bool isTalkPause;
    
    public static int yukariHP = 100;
    public static int yukariMaxHP = 100;
    public static int yukariMP = 100;
    public static int yukariMaxMP = 100;
    public static int makiHP = 100;
    public static int makiMaxHP = 100;
    public static int makiMP = 100;
    public static int makiMaxMP = 100;
    public static float yukariAttackRatio = 1.0f;
    public static float makiAttackRatio = 1.0f;
    
    public static int[] bossHP = { 0, 0, 0 };
    public static int[] bossMaxHP = { 0, 0, 0 };

    public static bool isFixedCamera;

    public static string scene;

    public static bool isReloadACB = true;

    public static Dictionary<string, bool> skills = new Dictionary<string, bool>();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static void Load()
    {
        LoadFloat(ref time, "Time");
        LoadInt(ref score, "Score");
        LoadInt(ref coinCount, "CoinCount");
        LoadInt(ref deadCount, "DeadCount");

        int intSwitchState = 0;
        LoadInt(ref intSwitchState, "SwitchState");
        switchState = (SwitchState)intSwitchState;

        LoadInt(ref yukariHP, "YukariHP");
        LoadInt(ref yukariMaxHP, "YukariMaxHP");
        LoadInt(ref yukariMP, "YukariMP");
        LoadInt(ref yukariMaxMP, "YukariMaxMP");
        LoadInt(ref makiHP, "MakiHP");
        LoadInt(ref makiMaxHP, "MakiMaxHP");
        LoadInt(ref makiMP, "MakiMP");
        LoadInt(ref makiMaxMP, "MakiMaxMP");
        LoadFloat(ref yukariAttackRatio, "YukariAttackRatio");
        LoadFloat(ref makiAttackRatio, "MakiAttackRatio");

        skills = LoadDict<string, bool>("Skills");

        if (PlayerPrefs.HasKey("Scene"))
        {
            scene = PlayerPrefs.GetString("Scene");
        }
    }
    
    public static void LoadFloat(ref float value, string prefs)
    {
        if (PlayerPrefs.HasKey(prefs))
        {
            value = PlayerPrefs.GetFloat(prefs);
        }
    }
    public static void LoadInt(ref int value, string prefs)
    {
        if (PlayerPrefs.HasKey(prefs))
        {
            value = PlayerPrefs.GetInt(prefs);
        }
    }
    public static void LoadInt(ref int value, string prefs, int defaultValue)
    {
        if (PlayerPrefs.HasKey(prefs))
        {
            value = PlayerPrefs.GetInt(prefs);
        }
        else
        {
            value = defaultValue;
        }
    }

    public static Dictionary<Key, Value> LoadDict<Key, Value> (string key)
    {
        if (PlayerPrefs.HasKey(key)) {
            string str = PlayerPrefs.GetString(key, "");
            return Deserialize<Dictionary<Key, Value>> (str);
        }
        return new Dictionary<Key, Value> ();
    }

    public static void Save()
    {
        PlayerPrefs.SetFloat("Time", time);
        PlayerPrefs.SetInt("Score", score);
        PlayerPrefs.SetInt("CoinCount", coinCount);
        PlayerPrefs.SetInt("DeadCount", deadCount);

        PlayerPrefs.SetInt("SwitchState", (int)switchState);
        PlayerPrefs.SetInt("YukariHP", yukariHP);
        PlayerPrefs.SetInt("YukariMaxHP", yukariMaxHP);
        PlayerPrefs.SetInt("YukariMP", yukariMP);
        PlayerPrefs.SetInt("YukariMaxMP", yukariMaxMP);
        PlayerPrefs.SetInt("MakiHP", makiHP);
        PlayerPrefs.SetInt("MakiMaxHP", makiMaxHP);
        PlayerPrefs.SetInt("MakiMP", makiMP);
        PlayerPrefs.SetInt("MakiMaxMP", makiMaxMP);
        
        PlayerPrefs.SetFloat("YukariAttackRatio", yukariAttackRatio);
        PlayerPrefs.SetFloat("MakiAttackRatio", makiAttackRatio);

        string serizlizedSkills = Serialize<Dictionary<string, bool>> (skills);
        PlayerPrefs.SetString ("Skills", serizlizedSkills);
        
        scene = SceneManager.GetActiveScene().name;
        PlayerPrefs.SetString("Scene", scene);
    }

    
    public static void AddMP(bool isYukari, int point)
    { 
        if (isYukari)
        {
            yukariMP += point;
            yukariMP = Mathf.Min(yukariMaxMP, yukariMP);
            yukariMP = Mathf.Max(0, yukariMP);
        }
        else
        {
            makiMP += point;
            makiMP = Mathf.Min(makiMaxMP, makiMP);
            makiMP = Mathf.Max(0, makiMP);
        }
    }
    public static void AddHP(bool isYukari, int point)
    {
        if (isYukari)
        {
            yukariHP += point;
            yukariHP = Mathf.Min(yukariMaxHP, yukariHP);
            yukariHP = Mathf.Max(0, yukariHP);
        }
        else
        {
            makiHP += point;
            makiHP = Mathf.Min(makiMaxHP, makiHP);
            makiHP = Mathf.Max(0, makiHP);
        }
    }
    public static void ResetBossHP()
    {
        for (int i = 0; i < 3; i++)
        {
            bossHP[i] = 0;
            bossMaxHP[i] = 0;
        }
    }

    public static void AddSkill(string skillName, bool flag)
    {
        skills[skillName] = flag;
        UpdateMaxHPMP();
        UpdateAttackRatio();
    }
    public static bool GetSkill(string skillName)
    {
        return skills.ContainsKey(skillName) && skills[skillName];
    }
    static void UpdateMaxHPMP()
    {
        int beforeYukariMaxHP = yukariMaxHP;
        int beforeMakiMaxHP = makiMaxHP;
        int beforeYukariMaxMP = yukariMaxMP;
        int beforeMakiMaxMP = makiMaxMP;
        yukariMaxHP = 100;
        yukariMaxMP = 100;
        makiMaxHP = 100;
        makiMaxMP = 100;
        foreach (KeyValuePair<string, bool> kvp in skills)
        {
            string skillName = kvp.Key;
            switch (skillName)
            {
                case "y_heart_1":
                    yukariMaxHP += 25;
                    break;
                case "y_heart_2":
                    yukariMaxHP += 25;
                    break;
                case "y_heart_3":
                    yukariMaxHP += 25;
                    break;
                case "y_heart_4":
                    yukariMaxHP += 25;
                    break;
                case "m_heart_4":
                    makiMaxHP += 25;
                    break;
                case "m_heart_3":
                    makiMaxHP += 25;
                    break;
                case "m_heart_2":
                    makiMaxHP += 25;
                    break;
                case "m_heart_1":
                    makiMaxHP += 25;
                    break;
                case "y_mp_1":
                    yukariMaxMP += 25;
                    break;
                case "y_mp_2":
                    yukariMaxMP += 25;
                    break;
                case "y_mp_3":
                    yukariMaxMP += 25;
                    break;
                case "y_mp_4":
                    yukariMaxMP += 25;
                    break;
                case "m_mp_4":
                    makiMaxMP += 25;
                    break;
                case "m_mp_3":
                    makiMaxMP += 25;
                    break;
                case "m_mp_2":
                    makiMaxMP += 25;
                    break;
                case "m_mp_1":
                    makiMaxMP += 25;
                    break;
            }
        }
        
        // 最大HP/MPが増えたときに現在HP/MPも増やす
        if (yukariMaxHP > beforeYukariMaxHP)
        {
            yukariHP += (yukariMaxHP - beforeYukariMaxHP);
        }
        if (makiMaxHP > beforeMakiMaxHP)
        {
            makiHP += (makiMaxHP - beforeMakiMaxHP);
        }
        if (yukariMaxMP > beforeYukariMaxMP)
        {
            yukariMP += (yukariMaxMP - beforeYukariMaxMP);
        }
        if (makiMaxMP > beforeMakiMaxMP)
        {
            makiMP += (makiMaxMP - beforeMakiMaxMP);
        }
    }

    static void UpdateAttackRatio()
    {
        yukariAttackRatio = 1.0f;
        makiAttackRatio = 1.0f;
        foreach (KeyValuePair<string, bool> kvp in skills)
        {
            string skillName = kvp.Key;
            switch (skillName)
            {
                case "y_damage_1":
                    yukariAttackRatio += 0.25f;
                    break;
                case "y_damage_2":
                    yukariAttackRatio += 0.25f;
                    break;
                case "y_damage_3":
                    yukariAttackRatio += 0.25f;
                    break;
                case "y_damage_4":
                    yukariAttackRatio += 0.25f;
                    break;
                case "m_damage_4":
                    makiAttackRatio += 0.25f;
                    break;
                case "m_damage_3":
                    makiAttackRatio += 0.25f;
                    break;
                case "m_damage_2":
                    makiAttackRatio += 0.25f;
                    break;
                case "m_damage_1":
                    makiAttackRatio += 0.25f;
                    break;
            }
        }
    }

    private static string Serialize<T> (T obj){
        BinaryFormatter binaryFormatter = new BinaryFormatter ();
        MemoryStream    memoryStream    = new MemoryStream ();
        binaryFormatter.Serialize (memoryStream , obj);
        return Convert.ToBase64String (memoryStream   .GetBuffer ());
    }

    private static T Deserialize<T> (string str){
        BinaryFormatter binaryFormatter = new BinaryFormatter ();
        MemoryStream    memoryStream    = new MemoryStream (Convert.FromBase64String (str));
        return (T)binaryFormatter.Deserialize (memoryStream);
    }
}
