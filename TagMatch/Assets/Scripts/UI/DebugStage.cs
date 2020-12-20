using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DebugStage : MonoBehaviour
{
    Dictionary<string, bool> skills = new Dictionary<string, bool>();
    // Start is called before the first frame update
    void Start()
    {
        var skillNameText = GameObject.Find("SkillNameText").GetComponent<Text>();
        var skillDescriptionText = GameObject.Find("SkillDescriptionText").GetComponent<Text>();
        var costText = GameObject.Find("CostText").GetComponent<Text>();

        var csvFile = Resources.Load("MasterData/skill_tree") as TextAsset;
        StringReader reader = new StringReader(csvFile.text);
        skills = StaticValues.skills;
        int i = 0;
        while (reader.Peek() > -1)
        {
            i++;
            string line = reader.ReadLine();
            string[] datas = line.Split(',');
            if (i == 1) // 一行目はカラム名のためskip
            {
                continue;
            }
            StaticValues.AddSkill(datas[0], true);
        }
    }

    private void OnDestroy()
    {
        StaticValues.skills = skills;
    }

    // Update is called once per frame
    void Update()
    {
        StaticValues.AddMP(IsYukari(), 1);
    }

    public void PlayGame()
    {
        SceneManager.LoadScene("DebugStage");
    }

    private bool IsYukari()
    {
        return StaticValues.switchState == StaticValues.SwitchState.YUKARI || StaticValues.switchState == StaticValues.SwitchState.YUKARI_ONLY;
    }
    private bool IsMaki()
    {
        return StaticValues.switchState == StaticValues.SwitchState.MAKI || StaticValues.switchState == StaticValues.SwitchState.MAKI_ONLY;
    }
}
