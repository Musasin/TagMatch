using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using DG.Tweening;

public class SkillSelect : MonoBehaviour
{
    public GameObject skillOpenEffect;
    private char lf = (char)10;
    TextAsset csvFile;
    string nowKey;
    Text skillNameText, skillDescriptionText, costText;
    bool isEnabled = true;
    float intervalTime;

    private struct SkillTreeData
    {
        public string unique_key;
        public string name;
        public string description;
        public int level;
        public int cost;
        public string release_condition;
        public int pos_x;
        public int pos_y;
        public string r_key;
        public string l_key;
        public string u_key;
        public string d_key;
        public GameObject gameObject;

        public SkillTreeData(string unique_key, string name, string description, string level, string cost, string release_condition, string pos_x, string pos_y, string r_key, string l_key, string u_key, string d_key)
        {
            this.unique_key = unique_key;
            this.name = name;
            this.description = description;
            this.level = int.Parse(level);
            this.cost = int.Parse(cost);
            this.release_condition = release_condition;
            this.pos_x = int.Parse(pos_x);
            this.pos_y = int.Parse(pos_y);
            this.r_key = r_key;
            this.l_key = l_key;
            this.u_key = u_key;
            this.d_key = d_key;
            gameObject = GameObject.Find(unique_key);
            if (this.release_condition == "")
            {
                gameObject.transform.Find("RockPanel").gameObject.SetActive(false);
            }
            Transform levelIcon1 = gameObject.transform.Find("LevelIcon1");
            if (levelIcon1 != null && this.level != 1) levelIcon1.gameObject.SetActive(false);
            Transform levelIcon2 = gameObject.transform.Find("LevelIcon2");
            if (levelIcon2 != null && this.level != 2) levelIcon2.gameObject.SetActive(false);
            Transform levelIcon3 = gameObject.transform.Find("LevelIcon3");
            if (levelIcon3 != null && this.level != 3) levelIcon3.gameObject.SetActive(false);
        }
    }
    private Dictionary<string, SkillTreeData> skillTrees = new Dictionary<string, SkillTreeData>();

    // Start is called before the first frame update
    void Start()
    {
        skillNameText = GameObject.Find("SkillNameText").GetComponent<Text>();
        skillDescriptionText = GameObject.Find("SkillDescriptionText").GetComponent<Text>();
        costText = GameObject.Find("CostText").GetComponent<Text>();

        csvFile = Resources.Load("MasterData/skill_tree") as TextAsset;
        StringReader reader = new StringReader(csvFile.text);

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
            if (i == 2) // 初期化用に一番初めのキーを入れとく
            {
                nowKey = datas[0];
            }
            var skillData = new SkillTreeData(datas[0],datas[1],datas[2],datas[3],datas[4],datas[5],datas[6],datas[7],datas[8],datas[9],datas[10],datas[11]);
            skillTrees.Add(datas[0], skillData);
        }
        transform.localPosition = new Vector2(skillTrees[nowKey].pos_x, skillTrees[nowKey].pos_y);
        
        // ポーズ中でも動かせるようにタイムスケールを無視する
        GetComponent<Animator>().updateMode = AnimatorUpdateMode.UnscaledTime;
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!isEnabled) 
            return;

        if (AxisDownChecker.GetAxisDownHorizontal())
        {
            if (Input.GetAxisRaw("Horizontal") < 0)
                nowKey = skillTrees[nowKey].l_key;
            else if (Input.GetAxisRaw("Horizontal") > 0)
                nowKey = skillTrees[nowKey].r_key;

            transform.DOLocalMove(new Vector2(skillTrees[nowKey].pos_x, skillTrees[nowKey].pos_y), 0.1f).SetUpdate(true);
        }

        if (AxisDownChecker.GetAxisDownVertical())
        {
            if (Input.GetAxisRaw("Vertical") < 0)
                nowKey = skillTrees[nowKey].d_key;
            else if (Input.GetAxisRaw("Vertical") > 0)
                nowKey = skillTrees[nowKey].u_key;
            transform.DOLocalMove(new Vector2(skillTrees[nowKey].pos_x, skillTrees[nowKey].pos_y), 0.1f).SetUpdate(true);
        }
        
        if (KeyConfig.GetJumpKeyDown())
        {
            if (!StaticValues.GetSkill(skillTrees[nowKey].unique_key) && 
                StaticValues.coinCount >= skillTrees[nowKey].cost &&
                (StaticValues.GetSkill(skillTrees[nowKey].release_condition) || skillTrees[nowKey].release_condition == ""))
            {
                StaticValues.coinCount -= skillTrees[nowKey].cost;
                GameObject effect = Instantiate(skillOpenEffect, transform);
                effect.GetComponent<Animator>().updateMode = AnimatorUpdateMode.UnscaledTime;
                StaticValues.AddSkill(skillTrees[nowKey].unique_key, true);
                UpdateSkillIcons();
            }
        }
        AxisDownChecker.AxisDownUpdate();
        
        skillNameText.text = skillTrees[nowKey].name;
        skillDescriptionText.text = skillTrees[nowKey].description.Replace("\\n", lf.ToString());
        costText.text = "-" + skillTrees[nowKey].cost.ToString();
    }

    void UpdateSkillIcons()
    {
        foreach (KeyValuePair<string, SkillTreeData> kvp in skillTrees)
        {
            SkillTreeData skill = kvp.Value;

            // スキル解放条件を満たした際に鍵マークを消す
            if (StaticValues.GetSkill(skill.release_condition))
            {
                GameObject rockPanel = skill.gameObject.transform.Find("RockPanel").gameObject;
                if (rockPanel != null)
                    rockPanel.transform.DOScale(new Vector2(0, 0), 0.5f).SetUpdate(true);
            }
            
            // スキル習得済み
            if (StaticValues.GetSkill(skill.unique_key))
            {
                GameObject darkPanel = skill.gameObject.transform.Find("DarkPanel").gameObject;
                if (darkPanel != null)
                    darkPanel.GetComponent<Image>().DOColor(new Color(1, 1, 1, 0), 0.5f).SetUpdate(true);
            }
            
            // スキル習得条件を満たしている & スキル未習得
            if ((skill.release_condition == "" || StaticValues.GetSkill(skill.release_condition)) && 
                !StaticValues.GetSkill(skill.unique_key))
            {
                GameObject darkPanel = skill.gameObject.transform.Find("DarkPanel").gameObject;
                if (darkPanel != null)
                {
                    if (skill.cost <= StaticValues.coinCount)
                        darkPanel.GetComponent<Image>().DOColor(new Color(1, 1, 1, 0.4f), 0.5f).SetUpdate(true);
                    else 
                        darkPanel.GetComponent<Image>().DOColor(new Color(1, 1, 1, 0.8f), 0.5f).SetUpdate(true);
                }
            }
        }
    }

    public void SetEnabled(bool flag)
    {
        isEnabled = flag;
    }
}
