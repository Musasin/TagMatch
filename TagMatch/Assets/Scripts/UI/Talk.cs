using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using DG.Tweening;
using TMPro;
using UnityEngine.SceneManagement;

public class Talk: MonoBehaviour
{
    public GameObject yukari, rightYukari, maki, rightMaki, kiritan;
    public GameObject leftWindow, rightWindow;

    GameObject nowWindow, beforeWindow1, beforeWindow2;
    bool isCameraMoving, isClosing;
    bool beforeIsPlaying;
    bool isPlaying;
    float closeTime;
    int nowKey;

    private char lf = (char)10;

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
        string sceneName = SceneManager.GetActiveScene().name;
        string acbName = sceneName.Split('-')[0];
        LoadACB(acbName, acbName + ".acb");
        AudioManager.Instance.PlayBGM("stage");
    }

    // Update is called once per frame
    void Update()
    {
        if (isCameraMoving)
        {
            if (!StaticValues.isFixedCamera)
            {
                isCameraMoving = false;
                beforeIsPlaying = false;
                isPlaying = true;
            }
        }
        if (isClosing)
        {
            closeTime -= Time.deltaTime;
            if (closeTime <= 0)
            {
                isClosing = false;
                isPlaying = false;
                StaticValues.isTalkPause = false;
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
        
        if (KeyConfig.GetShotKeyDown() || KeyConfig.GetJumpKeyDown())
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
                        yukari.GetComponent<Animator>().SetBool("isOut", false);
                        break;
                    case "right_yukari":
                        rightYukari = Instantiate(rightYukari, transform);
                        rightYukari.GetComponent<Animator>().SetBool("isOut", false);
                        break;
                    case "maki":
                        maki = Instantiate(maki, transform);
                        maki.GetComponent<Animator>().SetBool("isOut", false);
                        break;
                    case "right_maki":
                        rightMaki = Instantiate(rightMaki, transform);
                        rightMaki.GetComponent<Animator>().SetBool("isOut", false);
                        break;
                    case "kiritan":
                        kiritan = Instantiate(kiritan, transform);
                        kiritan.GetComponent<Animator>().SetBool("isOut", false);
                        break;
                }
                break;
            case "switch":
                SwitchYukaMaki();
                break;
            case "left":
                AddTalk(leftWindow, scenario[nowKey.ToString()].text.Replace("\\n", lf.ToString()));
                break;
            case "right":
                AddTalk(rightWindow, scenario[nowKey.ToString()].text.Replace("\\n", lf.ToString()));
                break;
            case "out":
                switch (scenario[nowKey.ToString()].text)
                {
                    case "yukari":
                        yukari.GetComponent<Animator>().SetBool("isOut", true);
                        break;
                    case "right_yukari":
                        rightYukari.GetComponent<Animator>().SetBool("isOut", true);
                        break;
                    case "maki":
                        maki.GetComponent<Animator>().SetBool("isOut", true);
                        break;
                    case "right_maki":
                        rightMaki.GetComponent<Animator>().SetBool("isOut", true);
                        break;
                    case "kiritan":
                        kiritan.GetComponent<Animator>().SetBool("isOut", true);
                        break;
                }
                break;
            case "play_bgm":
                AudioManager.Instance.StopBGM();
                AudioManager.Instance.PlayBGM(scenario[nowKey.ToString()].text);
                break;
            case "end":
                if (beforeWindow2 != null)
                    beforeWindow2.GetComponent<Transform>().DOMove(new Vector2(beforeWindow2.GetComponent<Transform>().position.x, 800), 0.3f);
                if (beforeWindow1 != null)
                    beforeWindow1.GetComponent<Transform>().DOMove(new Vector2(beforeWindow1.GetComponent<Transform>().position.x, 800), 0.3f);
                if (nowWindow != null)
                    nowWindow.GetComponent<Transform>().DOMove(new Vector2(nowWindow.GetComponent<Transform>().position.x, 800), 0.3f);
                isClosing = true;
                closeTime = 0.1f;
                scenario.Clear();
                return;
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
        obj.GetComponentInChildren<TextMeshProUGUI>().text = text;

        Destroy(beforeWindow2);
        beforeWindow2 = beforeWindow1;
        beforeWindow1 = nowWindow;
        nowWindow = obj;

        if (beforeWindow1 != null)
        {
            Transform tr = beforeWindow1.GetComponent<Transform>();
            tr.DOMove(new Vector2(tr.position.x, tr.position.y + 200), 0.3f);
            beforeWindow1.GetComponent<Animator>().SetInteger("state", 1);
        }
        if (beforeWindow2 != null)
        {
            Transform tr = beforeWindow2.GetComponent<Transform>();
            tr.DOMove(new Vector2(tr.position.x, tr.position.y + 200), 0.3f);
            beforeWindow2.GetComponent<Animator>().SetInteger("state", 2);
        }
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
            beforeIsPlaying = false;
            isPlaying = true;
        }
    }
        
    private void LoadACB(string cueSheetName, string acbName, string awbName = "")
    {
        if (StaticValues.isReloadACB == false) { return; }
        AudioManager.Instance.LoadACB(cueSheetName, acbName, awbName);
        StaticValues.isReloadACB = false;
    }
}
