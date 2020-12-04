using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticValues : MonoBehaviour
{
    public static int score;
    public static int coinCount = 1000;
    public static float time;
    public static bool isPause;
    
    public static int yukariHP = 100;
    public static int yukariMaxHP = 100;
    public static int yukariMP = 100;
    public static int yukariMaxMP = 100;
    public static int makiHP = 100;
    public static int makiMaxHP = 100;
    public static int makiMP = 100;
    public static int makiMaxMP = 100;

    public static int bossHP;
    public static int bossMaxHP;

    public static bool isFixedCamera;

    public static Dictionary<string, bool> skills = new Dictionary<string, bool>();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
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

    public static void AddSkill(string skillName, bool flag)
    {
        skills[skillName] = flag;
        UpdateMaxHPMP();
    }
    public static bool GetSkill(string skillName)
    {
        return skills.ContainsKey(skillName) && skills[skillName];
    }
    static void UpdateMaxHPMP()
    {
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
    }
}
