using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using DG.Tweening;
using TMPro;
using UnityEngine.SceneManagement;
using System.Linq;

public class Talk: MonoBehaviour
{
    public GameObject yukariPrefab, rightYukariPrefab, makiPrefab, rightMakiPrefab, kiritanPrefab, akanePrefab, aoiPrefab;
    public GameObject leftWindow, rightWindow;

    WipePanel wipePanel;
    GameObject nowWindow, beforeWindow1, beforeWindow2;
    Dictionary<string, GameObject> charaObject = new Dictionary<string, GameObject>();
    Dictionary<string, CharaPicture> charaPicture = new Dictionary<string, CharaPicture>();
    Dictionary<string, Animator> leftCharaAnimator = new Dictionary<string, Animator>();
    Dictionary<string, Animator> rightCharaAnimator = new Dictionary<string, Animator>();
    bool isCameraMoving, isClosing;
    bool beforeIsPlaying;
    bool isPlaying;
    bool isBossBattleWaiting;
    string stackScenarioFileName;
    float closeTime;
    int nowKey;
    float time;

    private char lf = (char)10;

    private struct ScenarioData
    {
        public int id;
        public string type;
        public string chara;
        public string position;
        public string text;
        public string main;
        public string eye_brows;
        public string eye;
        public string mouse;
        public string option1;
        public string option2;
        public string option3;
        public bool play_next;
        public string voice;
        
        public ScenarioData(string id, string type, string chara, string position, string text, string main, string eye_brows, string eye, string mouse, string option1, string option2, string option3, string play_next, string voice)
        {
            this.id = int.Parse(id);
            this.type = type;
            this.chara = chara;
            this.position = position;
            this.text = text;
            this.main= main;
            this.eye_brows = eye_brows;
            this.eye = eye;
            this.mouse = mouse;
            this.option1 = option1;
            this.option2 = option2;
            this.option3 = option3;
            this.play_next = (play_next == "1" ? true : false);
            this.voice = voice;
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
        
        wipePanel = GameObject.Find("WipePanel").GetComponent<WipePanel>();

        isPlaying = false;
        string sceneName = SceneManager.GetActiveScene().name;
        string acbName = sceneName.Split('-')[0];
        LoadACB(acbName, acbName + ".acb");
        
        charaObject.Add("yukari", Instantiate(yukariPrefab, transform));
        charaObject.Add("maki", Instantiate(makiPrefab, transform));
        charaObject.Add("kiritan", Instantiate(kiritanPrefab, transform));
        charaObject.Add("akane", Instantiate(akanePrefab, transform));
        charaObject.Add("aoi", Instantiate(aoiPrefab, transform));

        if (AudioManager.Instance.lastPlayedBGM != "stage")
            AudioManager.Instance.PlayBGM("stage");
    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;

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

        if (isBossBattleWaiting && StaticValues.bossHP.Sum() <= 0)
        {
            SetScenario(stackScenarioFileName, false);
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

        // ボス撃破後に連打でページ送りしてしまうのを防止するため少しの間入力を受け付けない
        if (time <= 0.7f)
        {
            return;
        }
        
        if (KeyConfig.GetShotKeyDown() || KeyConfig.GetJumpKeyDown())
        {
            UpdateStep();
        }
    }

    void UpdateStep()
    {
        if (!scenario.ContainsKey(nowKey.ToString()))
        {
            return;
        }

        string chara = scenario[nowKey.ToString()].chara;
        string position = scenario[nowKey.ToString()].position;

        switch (scenario[nowKey.ToString()].type)
        {
            case "instantiate":
                if (position == "right")
                {
                    if (!rightCharaAnimator.ContainsKey(chara))
                    {
                        Animator anim = charaObject[chara].GetComponent<Animator>();
                        rightCharaAnimator.Add(chara, anim);
                    }
                    rightCharaAnimator[chara].SetBool("isOut", false);
                }
                else if (position == "left")
                {
                    if (!leftCharaAnimator.ContainsKey(chara))
                    {
                        Animator anim = charaObject[chara].GetComponent<Animator>();
                        leftCharaAnimator.Add(chara, anim);
                    }
                    leftCharaAnimator[chara].SetBool("isOut", false);
                }

                if (!charaPicture.ContainsKey(chara))
                {
                    charaPicture.Add(chara, new CharaPicture(charaObject[chara]));
                }

                // 表情差分
                if (charaPicture.ContainsKey(chara))
                {
                    charaPicture[chara].SetSprite(
                        scenario[nowKey.ToString()].main,
                        scenario[nowKey.ToString()].eye_brows, scenario[nowKey.ToString()].eye, scenario[nowKey.ToString()].mouse, 
                        scenario[nowKey.ToString()].option1, scenario[nowKey.ToString()].option2, scenario[nowKey.ToString()].option3);
                }
                break;

            case "talk":
                if (position == "left")
                {
                    AddTalk(leftWindow, scenario[nowKey.ToString()].text.Replace("\\n", lf.ToString()));
                    foreach (KeyValuePair<string, Animator> kvp in leftCharaAnimator)
                    {
                        if (kvp.Key == chara)
                        {
                            kvp.Value.SetBool("isSub", false);
                        } else
                        {
                            kvp.Value.SetBool("isSub", true);
                        }
                    }
                } 
                if (position == "right")
                {
                    AddTalk(rightWindow, scenario[nowKey.ToString()].text.Replace("\\n", lf.ToString()));
                    foreach (KeyValuePair<string, Animator> kvp in rightCharaAnimator)
                    {
                        if (kvp.Key == chara)
                        {
                            kvp.Value.SetBool("isSub", false);
                        } else
                        {
                            kvp.Value.SetBool("isSub", true);
                        }
                    }
                }

                charaObject[chara].transform.SetAsLastSibling();

                // 表情差分
                if (charaPicture.ContainsKey(chara))
                {
                    charaPicture[chara].SetSprite(
                        scenario[nowKey.ToString()].main,
                        scenario[nowKey.ToString()].eye_brows, scenario[nowKey.ToString()].eye, scenario[nowKey.ToString()].mouse, 
                        scenario[nowKey.ToString()].option1, scenario[nowKey.ToString()].option2, scenario[nowKey.ToString()].option3);
                }
                break;

            case "out":
                if (position == "right" && rightCharaAnimator.ContainsKey(chara))
                    rightCharaAnimator[chara].SetBool("isOut", true);
                if (position == "left" && leftCharaAnimator.ContainsKey(chara))
                    leftCharaAnimator[chara].SetBool("isOut", true);
                break;
            case "play_bgm":
                AudioManager.Instance.StopBGM();
                AudioManager.Instance.PlayBGM(scenario[nowKey.ToString()].text);
                break;
            case "animation_flag":
                GameObject obj = GameObject.Find(scenario[nowKey.ToString()].chara);
                if (obj != null)
                {
                    Animator anim = obj.GetComponent<Animator>();
                    if (anim != null)
                    {
                        bool beforeFlag = anim.GetBool(scenario[nowKey.ToString()].text);
                        anim.SetBool(scenario[nowKey.ToString()].text, !beforeFlag);
                    }
                }
                break;
            case "start_boss_battle":
                stackScenarioFileName = scenario[nowKey.ToString()].text;
                isBossBattleWaiting = true;
                CloseWindow();
                scenario.Clear();
                return;
            case "end":
                CloseWindow();
                scenario.Clear();
                return;
            case "scene_change":
                StaticValues.isReloadACB = true;
                wipePanel.ChangeScene(scenario[nowKey.ToString()].text, true);
                CloseWindow();
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

    void CloseWindow()
    {
        if (beforeWindow2 != null)
            beforeWindow2.GetComponent<Transform>().DOMove(new Vector2(beforeWindow2.GetComponent<Transform>().position.x, 800), 0.3f);
        if (beforeWindow1 != null)
            beforeWindow1.GetComponent<Transform>().DOMove(new Vector2(beforeWindow1.GetComponent<Transform>().position.x, 800), 0.3f);
        if (nowWindow != null)
            nowWindow.GetComponent<Transform>().DOMove(new Vector2(nowWindow.GetComponent<Transform>().position.x, 800), 0.3f);
        isClosing = true;
        closeTime = 0.1f;
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
        time = 0;
        nowKey = 0;
        StaticValues.isTalkPause = true;
        isBossBattleWaiting = false;

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
            var scenarioData = new ScenarioData(datas[0],datas[1],datas[2],datas[3],datas[4],datas[5],datas[6],datas[7],datas[8],datas[9],datas[10],datas[11],datas[12],datas[13]);
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
