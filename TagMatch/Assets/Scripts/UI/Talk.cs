using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class Talk: MonoBehaviour
{
    public GameObject yukari, maki, kiritan;
    public GameObject leftWindow, rightWindow;

    bool isCameraMoving;
    bool beforeIsPlaying;
    bool isPlaying;
    int nowKey;

    private struct ScenarioData
    {
        public int id;
        public string type;
        public string text;
        public bool play_next;
        
        public ScenarioData(string id, string type, string text, string play_next)
        {
            this.id = int.Parse(id);
            this.type = type;
            this.text = text;
            this.play_next = (play_next == "1" ? true : false);
        }
    }
    private Dictionary<string, ScenarioData> scenario = new Dictionary<string, ScenarioData>();

    // Start is called before the first frame update
    void Start()
    {
        isPlaying = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (isCameraMoving)
        {
            if (!StaticValues.isFixedCamera)
            {
                isCameraMoving = false;
                isPlaying = true;
            }
        }

        if (!isPlaying)
        {
            return;
        }

        if (beforeIsPlaying != isPlaying)
        {
            beforeIsPlaying = isPlaying;
            UpdateStep(); // 最初一回自動再生
            return;
        }
        
        if (Input.GetButtonDown("Shot") || Input.GetButtonDown("Jump"))
        {
            UpdateStep();
        }
    }

    void UpdateStep()
    {
        switch (scenario[nowKey.ToString()].type)
        {
            case "instantiate":
                switch (scenario[nowKey.ToString()].text)
                {
                    case "yukari":
                        yukari = Instantiate(yukari, transform);
                        break;
                    case "maki":
                        maki = Instantiate(maki, transform);
                        break;
                    case "kiritan":
                        kiritan = Instantiate(kiritan, transform);
                        break;
                }
                break;
            case "switch":
                SwitchYukaMaki();
                break;
            case "left":
                AddTalk(leftWindow, scenario[nowKey.ToString()].text);
                break;
            case "right":
                AddTalk(rightWindow, scenario[nowKey.ToString()].text);
                break;
        }
        if (scenario[nowKey.ToString()].play_next)
        {
            nowKey++;
            UpdateStep();
        } else
        {
            nowKey++;
        }
    }

    void SwitchYukaMaki()
    {
        Animator makiAnim = maki.GetComponent<Animator>();
        makiAnim.SetBool("isUp", !makiAnim.GetBool("isUp"));
        Animator yukariAnim = yukari.GetComponent<Animator>();
        yukariAnim.SetBool("isUp", !yukariAnim.GetBool("isUp"));
    }

    void AddTalk(GameObject talkWindow, string text)
    {
        GameObject obj = Instantiate(talkWindow, transform);
        obj.GetComponentInChildren<Text>().text = text;
    }

    public void SetScenario(string scenarioFileName, bool isMoveCamera)
    {
        StaticValues.isTalkPause = true;

        TextAsset csvFile = Resources.Load("Scenario/" + scenarioFileName) as TextAsset;
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
                nowKey = int.Parse(datas[0]);
            }
            var scenarioData = new ScenarioData(datas[0],datas[1],datas[2],datas[3]);
            scenario.Add(datas[0], scenarioData);
        }
        if (isMoveCamera)
        {
            isCameraMoving = true;
        } else
        {
            isPlaying = true;
        }
    }
}
