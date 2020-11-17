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
    }
    public static bool GetSkill(string skillName)
    {
        return skills.ContainsKey(skillName) && skills[skillName];
    }
}
