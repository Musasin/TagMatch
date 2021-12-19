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
    public GameObject yukariPrefab, rightYukariPrefab, makiPrefab, rightMakiPrefab, kiritanPrefab, akanePrefab, leftAkanePrefab, aoiPrefab, leftAoiPrefab, itakoPrefab, zunkoPrefab, frimomenPrefab, mob1Prefab, mob2Prefab, mob3Prefab;
    public GameObject leftWindow, rightWindow, angerLeftWindow, angerRightWindow, centerWindow;
    public GameObject leftNamePlate, rightNamePlate;
    public GameObject talkFlash, whiteOutPanel;

    WipePanel wipePanel;
    GameObject nowWindow, beforeWindow1, beforeWindow2;
    GameObject namePlate;
    Dictionary<string, GameObject> charaObject = new Dictionary<string, GameObject>();
    Dictionary<string, CharaPicture> charaPicture = new Dictionary<string, CharaPicture>();
    Dictionary<string, Animator> leftCharaAnimator = new Dictionary<string, Animator>();
    Dictionary<string, Animator> rightCharaAnimator = new Dictionary<string, Animator>();
    bool isCameraMoving, isClosing;
    bool beforeIsPlaying;
    bool isPlaying, isWiping, isWhiteOuting;
    bool isBossBattleWaiting, isSpecialBossBattleWaiting, isLastBossBattleWaiting;
    string stackScenarioFileName, stackSceneName;
    float closeTime;
    int nowKey;
    float time, wipeTime, waitTime, whiteOutTime;

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

    void AddChraObject(string key, GameObject prefab)
    {
        if(prefab == null) { return; }
        var obj = Instantiate(prefab, transform);
        if(obj == null) { return; }

        charaObject.Add(key, obj);
    }

    // Start is called before the first frame update
    void Start()
    {
        wipePanel = GameObject.Find("WipePanel").GetComponent<WipePanel>();
        
        string sceneName = SceneManager.GetActiveScene().name;
        string acbName = sceneName.Split('-')[0];
        LoadACB(acbName, acbName + ".acb", acbName + ".awb");


        AddChraObject("yukari", yukariPrefab);
        AddChraObject("right_yukari", rightYukariPrefab);
        AddChraObject("maki", makiPrefab);
        AddChraObject("right_maki", rightMakiPrefab);
        AddChraObject("kiritan", kiritanPrefab);
        AddChraObject("akane", akanePrefab);
        AddChraObject("left_akane", leftAkanePrefab);
        AddChraObject("aoi", aoiPrefab);
        AddChraObject("left_aoi", leftAoiPrefab);
        AddChraObject("itako", itakoPrefab);
        AddChraObject("zunko", zunkoPrefab);
        AddChraObject("frimomen", frimomenPrefab);
        AddChraObject("mob1", mob1Prefab);
        AddChraObject("mob2", mob2Prefab);
        AddChraObject("mob3", mob3Prefab);

        if (sceneName == "Stage6-F2")
            AudioManager.Instance.PlayBGM("frimomen2nd_talk");
        else if (AudioManager.Instance.lastPlayedBGM != "stage")
            AudioManager.Instance.PlayBGM("stage");

        StaticValues.Save();
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

        if (isSpecialBossBattleWaiting && StaticValues.bossDamage >= 200)
        {
            SetScenario(stackScenarioFileName, false);
        }

        // ラスボスのHPを削りきったら、スローモーションにしつつ画面をホワイトアウトさせる
        if (isLastBossBattleWaiting && StaticValues.bossHP.Sum() <= 0)
        {
            StaticValues.isTalkPause = true;
            isLastBossBattleWaiting = false;
            isWhiteOuting = true;
            whiteOutTime = 1;
            Instantiate(whiteOutPanel, transform);
            Time.timeScale = 0.25f;
        }
        
        // ホワイトアウト中はボタン押下を受け付けず、ホワイトアウトが終わったらその場でシーン切り替え
        if (isWhiteOuting)
        {
            whiteOutTime -= Time.deltaTime;
            if (whiteOutTime <= 0)
            {
                Time.timeScale = 1.0f;
                isWhiteOuting = false;
                StaticValues.isReloadACB = true;
                wipePanel.ChangeScene(stackSceneName, true);
            } else
            {
                return;
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

        // ボス撃破後に連打でページ送りしてしまうのを防止するため少しの間入力を受け付けない
        if (time <= 0.7f)
        {
            return;
        }

        // ワイプ中はボタン押下を受け付けず、ワイプが終わったらその場で再生
        if (isWiping)
        {
            wipeTime -= Time.deltaTime;
            if (wipeTime <= 0)
            {
                isWiping = false;
                UpdateStep();
            } else
            {
                return;
            }
        }

        if (waitTime > 0)
        {
            waitTime -= Time.deltaTime;
            if (waitTime <= 0)
            {
                UpdateStep();
            }
            else
            {
                return;
            }
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

        Debug.Log(scenario[nowKey.ToString()].type);
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

                AudioManager.Instance.StopExVoice();

                // 話者を先頭に持っていく
                if (chara != "")
                {
                    charaObject[chara].transform.SetAsLastSibling();
                    if (namePlate != null) namePlate.transform.SetAsLastSibling();
                }

                if (position == "left" || position == "anger_left")
                {
                    GameObject window = leftWindow;
                    if (position == "anger_left")
                    {
                        window = angerLeftWindow;
                    }
                    AddTalk(window, scenario[nowKey.ToString()].text.Replace("\\n", lf.ToString()));
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
                if (position == "right" || position == "anger_right")
                {
                    GameObject window = rightWindow;
                    if (position == "anger_right")
                    {
                        window = angerRightWindow;
                    }
                    AddTalk(window, scenario[nowKey.ToString()].text.Replace("\\n", lf.ToString()));
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
                if (position == "center")
                {
                    AddTalk(centerWindow, scenario[nowKey.ToString()].text.Replace("\\n", lf.ToString()));
                }

                // 表情差分
                if (charaPicture.ContainsKey(chara))
                {
                    charaPicture[chara].SetSprite(
                        scenario[nowKey.ToString()].main,
                        scenario[nowKey.ToString()].eye_brows, scenario[nowKey.ToString()].eye, scenario[nowKey.ToString()].mouse, 
                        scenario[nowKey.ToString()].option1, scenario[nowKey.ToString()].option2, scenario[nowKey.ToString()].option3);
                }

                if (scenario[nowKey.ToString()].voice != "")
                {
                    AudioManager.Instance.PlayExVoice(scenario[nowKey.ToString()].voice);
                }

                break;
                
            case "name_plate":
                if (namePlate != null) Destroy(namePlate);
                namePlate = Instantiate(position == "left" ? leftNamePlate : rightNamePlate, transform);
                namePlate.transform.Find("NameText").GetComponent<TextMeshProUGUI>().text = GetCharaName(scenario[nowKey.ToString()].chara);
                namePlate.transform.Find("DetailText").GetComponent<TextMeshProUGUI>().text = scenario[nowKey.ToString()].text;
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
            case "stop_bgm":
                AudioManager.Instance.StopBGM();
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
                CloseWindowForEnd();
                scenario.Clear();
                return;
            case "start_boss_battle_special": // フリモメン第二形態でのみ使用 回復分を加味せず、一定量のダメージを与えたら発火
                stackScenarioFileName = scenario[nowKey.ToString()].text;
                isSpecialBossBattleWaiting = true;
                CloseWindowForEnd();
                scenario.Clear();
                return;
            case "start_last_boss_battle": // フリモメン第二形態でのみ使用 撃破後にスローモーション+ホワイトアウトでシーンを切り替える
                stackSceneName = scenario[nowKey.ToString()].text;
                isLastBossBattleWaiting = true;
                CloseWindowForEnd();
                scenario.Clear();
                return;
            case "end":
                CloseWindowForEnd();
                scenario.Clear();
                return;
            case "scene_change":
                StaticValues.isReloadACB = scenario[nowKey.ToString()].main == "unreload" ? false : true;
                wipePanel.ChangeScene(scenario[nowKey.ToString()].text, true);
                CloseWindowForEnd();
                scenario.Clear();
                StaticValues.ResetBossHP();
                return;
            case "wipe_in":
                isWiping = true;
                wipeTime = 0.5f; // インのときは少し早く停止状態を解く
                wipePanel.WipeIn();
                break;
            case "wipe_out":
                isWiping = true;
                wipeTime = 1.0f;
                wipePanel.WipeOut();
                break;
            case "flash":
                Instantiate(talkFlash, transform);
                break;
            case "wait":
                waitTime = float.Parse(scenario[nowKey.ToString()].text);
                break;
            case "support_on": // フリモメン第二形態専用
                GameObject.Find("SupportBullet").GetComponent<SupportBulletCreater>().SetEnabled(true);
                break;
            case "close_window":
                CloseWindow();
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

    void CloseWindowForEnd()
    {
        CloseWindow();
        isClosing = true;
        closeTime = 0.1f;
    }

    void CloseWindow()
    {
        if (beforeWindow2 != null)
            beforeWindow2.GetComponent<Transform>().DOMove(new Vector2(beforeWindow2.GetComponent<Transform>().position.x, 800), 0.3f);
        if (beforeWindow1 != null)
            beforeWindow1.GetComponent<Transform>().DOMove(new Vector2(beforeWindow1.GetComponent<Transform>().position.x, 800), 0.3f);
        if (nowWindow != null)
            nowWindow.GetComponent<Transform>().DOMove(new Vector2(nowWindow.GetComponent<Transform>().position.x, 800), 0.3f);
        if (namePlate != null) 
            Destroy(namePlate);
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
            beforeWindow1.GetComponentInChildren<Animator>().SetInteger("state", 1);
        }
        if (beforeWindow2 != null)
        {
            Transform tr = beforeWindow2.GetComponent<Transform>();
            tr.DOMove(new Vector2(tr.position.x, tr.position.y + 200), 0.3f);
            beforeWindow2.GetComponentInChildren<Animator>().SetInteger("state", 2);
        }
    }

    public void SetScenario(string scenarioFileName, bool isMoveCamera)
    {
        Debug.Log("SetScenario: " + scenarioFileName);
        time = 0;
        nowKey = 0;
        StaticValues.isTalkPause = true;
        isBossBattleWaiting = false;
        isSpecialBossBattleWaiting = false;

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

    private string GetCharaName(string chara)
    {
        switch (chara)
        {
            case "frimomen":
                return "フリモメン";
            case "yukari":
                return "結月ゆかり";
            case "yukari_right":
                return "結月ゆかり";
            case "maki":
                return "弦巻マキ";
            case "maki_right":
                return "弦巻マキ";
            case "akane":
                return "琴葉茜";
            case "akane_left":
                return "琴葉茜";
            case "aoi":
                return "琴葉葵";
            case "aoi_left":
                return "琴葉葵";
            case "kiritan":
                return "東北きりたん";
            case "zunko":
                return "東北ずん子";
            case "itako":
                return "東北イタコ";
            default:
                return "";
        }
    }
        
    private void LoadACB(string cueSheetName, string acbName, string awbName = "")
    {
        if (StaticValues.isReloadACB == false) { return; }
        AudioManager.Instance.LoadACB(cueSheetName, acbName, awbName);
        StaticValues.isReloadACB = false;
    }
}
