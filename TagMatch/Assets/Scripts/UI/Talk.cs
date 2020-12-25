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
    public GameObject yukariPrefab, rightYukariPrefab, makiPrefab, rightMakiPrefab, kiritanPrefab;
    public GameObject leftWindow, rightWindow;

    GameObject yukari, maki, kiritan;

    GameObject nowWindow, beforeWindow1, beforeWindow2;
    Dictionary<string, CharaPicture> charaPicture = new Dictionary<string, CharaPicture>();
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
        public string eye_brows;
        public string eye;
        public string mouse;
        public string option1;
        public string option2;
        public string option3;
        public bool play_next;
        
        public ScenarioData(string id, string type, string text, string eye_brows, string eye, string mouse, string option1, string option2, string option3, string play_next)
        {
            this.id = int.Parse(id);
            this.type = type;
            this.text = text;
            this.eye_brows = eye_brows;
            this.eye = eye;
            this.mouse = mouse;
            this.option1 = option1;
            this.option2 = option2;
            this.option3 = option3;
            this.play_next = (play_next == "1" ? true : false);
        }
    }
    private Dictionary<string, ScenarioData> scenario = new Dictionary<string, ScenarioData>();

    // Start is called before the first frame update
    void Start()
    {
        // 仮。
        AudioManager.Instance.ChangeBGMVolume(0.4f);
        AudioManager.Instance.ChangeSEVolume(0.4f);
        AudioManager.Instance.ChangeVoiceVolume(0.4f);
        //AudioManager.Instance.ChangeBGMVolume(0);
        //AudioManager.Instance.ChangeSEVolume(0);
        //AudioManager.Instance.ChangeVoiceVolume(0);

        isPlaying = false;
        string sceneName = SceneManager.GetActiveScene().name;
        string acbName = sceneName.Split('-')[0];
        LoadACB(acbName, acbName + ".acb");

        if (AudioManager.Instance.lastPlayedBGM != "stage")
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
                    case "right_yukari":
                        if (yukari == null)
                        { 
                            if (scenario[nowKey.ToString()].text == "right_yukari")
                                yukari = Instantiate(rightYukariPrefab, transform);
                            else
                                yukari = Instantiate(yukariPrefab, transform);
                            charaPicture.Add(scenario[nowKey.ToString()].text, new CharaPicture(yukari));
                        }
                        yukari.GetComponent<Animator>().SetBool("isOut", false);
                        break;
                    case "maki":
                    case "right_maki":
                        if (maki == null)
                        {
                            if (scenario[nowKey.ToString()].text == "right_maki")
                                maki = Instantiate(rightMakiPrefab, transform);
                            else
                                maki = Instantiate(makiPrefab, transform);
                            charaPicture.Add(scenario[nowKey.ToString()].text, new CharaPicture(maki));
                        }
                        maki.GetComponent<Animator>().SetBool("isOut", false);
                        break;
                    case "kiritan":
                        if (kiritan == null)
                        {
                            kiritan = Instantiate(kiritanPrefab, transform);
                            charaPicture.Add("kiritan", new CharaPicture(kiritan));
                        }
                        kiritan.GetComponent<Animator>().SetBool("isOut", false);
                        break;
                }
                break;
            case "switch":
                bool isMainYukari = scenario[nowKey.ToString()].text == "yukari";
                SwitchYukaMaki(isMainYukari);
                break;
            case "face":
                if (charaPicture[scenario[nowKey.ToString()].text] != null)
                {
                    charaPicture[scenario[nowKey.ToString()].text].SetSprite(
                        scenario[nowKey.ToString()].eye_brows, scenario[nowKey.ToString()].eye, scenario[nowKey.ToString()].mouse, 
                        scenario[nowKey.ToString()].option1, scenario[nowKey.ToString()].option2, scenario[nowKey.ToString()].option3);
                }
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
                    case "maki":
                        maki.GetComponent<Animator>().SetBool("isOut", true);
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

    void SwitchYukaMaki(bool isMainYukari)
    {
        Animator makiAnim = maki.GetComponent<Animator>();
        makiAnim.SetBool("isUp", isMainYukari);
        Animator yukariAnim = yukari.GetComponent<Animator>();
        yukariAnim.SetBool("isUp", !isMainYukari);
    }

    void AddTalk(GameObject talkWindow, string text)
    {
        AudioManager.Instance.PlaySE("talk_step");
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
            var scenarioData = new ScenarioData(datas[0],datas[1],datas[2],datas[3],datas[4],datas[5],datas[6],datas[7],datas[8],datas[9]);
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
