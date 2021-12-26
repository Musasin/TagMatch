using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class Title : MonoBehaviour
{
    float time;
    Animator anim;
    Image fadePanel;
    GameObject titleCursor;
    GameObject optionCursor, optionLeftCursor, optionRightCursor, levelSelectCursor;
    Text bgmVolumeText, seVolumeText, voiceVolumeText, levelDescriptionText;
    Vector2 titleCursorDefaultPos;
    Vector2 optionCursorDefaultPos;
    Vector2 levelSelecrCursorDefaultPos;
    KeyConfigUI keyConfigUIScript;
    string loadSceneName;
    private int volumeWaitFlame;
    private int beforeInputHorSign;

    int nowSelectionWindowSize;

    enum TitleState
    {
        FADE_IN, TITLE, NEW_GAME_SELECT, EX_MODE_SELECT, OPTION, KEY_CONFIG, FADE_OUT, EXIT
    }
    TitleState titleState;

    enum TitleList
    {
        NEW_GAME, CONTINUE, EX_MODE, OPTION, EXIT,
    }
    TitleList nowSelection;

    enum OptionList
    {
        WINDOW_SIZE, BGM, SE, VOICE, KEY_CONFIG, CLOSE,
    }
    OptionList optionSelection;

    bool isSelectHardMode = false;

    // Start is called before the first frame update
    void Start()
    {
        if (PlayerPrefs.HasKey("WindowSize"))
            nowSelectionWindowSize = PlayerPrefs.GetInt("WindowSize");
        else
            nowSelectionWindowSize = 1;


        StaticValues.LoadIsCleared();
        StaticValues.LoadVolume();
        time = 0;
        anim = GetComponent<Animator>();
        fadePanel = GameObject.Find("FadePanel").GetComponent<Image>();
        titleCursor = GameObject.Find("TitleCursor");
        optionCursor = GameObject.Find("OptionCursor");
        optionLeftCursor = GameObject.Find("OptionLeftCursor");
        optionRightCursor = GameObject.Find("OptionRightCursor");
        levelSelectCursor = GameObject.Find("LevelSelectCursor");
        titleCursorDefaultPos = titleCursor.transform.localPosition;
        optionCursorDefaultPos = optionCursor.transform.localPosition;
        levelSelecrCursorDefaultPos = levelSelectCursor.transform.localPosition;
        keyConfigUIScript = GameObject.Find("KeyConfig").GetComponent<KeyConfigUI>();

        bgmVolumeText = GameObject.Find("BGMVolume").GetComponent<Text>();
        seVolumeText = GameObject.Find("SEVolume").GetComponent<Text>();
        voiceVolumeText = GameObject.Find("VoiceVolume").GetComponent<Text>();
        levelDescriptionText = GameObject.Find("LevelDescriptionText").GetComponent<Text>();
        
        if (StaticValues.isReloadACB == false) { return; }
        AudioManager.Instance.LoadACB("Title", "Title.acb", "Title.awb");
        StaticValues.isReloadACB = false;

        StaticValues.isReloadACB = false;

        if (PlayerPrefs.HasKey("Scene"))
        {
            nowSelection = TitleList.CONTINUE;
            titleCursor.transform.localPosition = new Vector2(titleCursorDefaultPos.x, titleCursorDefaultPos.y - (67 * (int)nowSelection));
            GameObject.Find("ContinueText").GetComponent<TextMeshProUGUI>().color = new Vector4(0, 0, 0, 1);
        }
        if (StaticValues.isCleared)
        {
            GameObject.Find("EXModeText").GetComponent<TextMeshProUGUI>().color = new Vector4(0, 0, 0, 1);
        }
    }
    
    void ResetWindowSize()
    {
        switch (nowSelectionWindowSize)
        {
            case 0:
                Screen.SetResolution(512, 384, false, 60);
                break;
            case 1:
                Screen.SetResolution(1024, 768, false, 60);
                break;
            case 2:
                Screen.SetResolution(2048, 1536, false, 60);
                break;
            case 3:
                Screen.SetResolution(1024, 768, true, 60);
                break;
            default:
                Screen.SetResolution(1024, 768, false, 60);
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;
        switch (titleState)
        {
            case TitleState.FADE_IN:
                fadePanel.color = new Color(0, 0, 0, 1.0f - time);
                if (time > 1.0f)
                {
                    if (AudioManager.Instance.lastPlayedBGM != "title")
                    {
                        AudioManager.Instance.PlayBGM("title");
                    }
                    titleState = TitleState.TITLE;
                }
                break;
                
            case TitleState.FADE_OUT:
                fadePanel.color = new Color(0, 0, 0, 0.0f + time);
                if (time > 1.0f)
                {
                    SceneManager.LoadScene(loadSceneName);
                    titleState = TitleState.EXIT;
                }
                break;
            case TitleState.TITLE:
                if (AxisDownChecker.GetAxisDownVertical())
                {
                    AudioManager.Instance.PlaySE("select");
                    if (Input.GetAxisRaw("Vertical") < 0) nowSelection++;
                    else if (Input.GetAxisRaw("Vertical") > 0) nowSelection--;
                    if (nowSelection > TitleList.EXIT) nowSelection = TitleList.NEW_GAME;
                    if (nowSelection < TitleList.NEW_GAME) nowSelection = TitleList.EXIT;
                    titleCursor.transform.localPosition = new Vector2(titleCursorDefaultPos.x, titleCursorDefaultPos.y - (67 * (int)nowSelection));
                }
                AxisDownChecker.AxisDownUpdate();

                if (KeyConfig.GetJumpKeyDown())
                {
                    switch (nowSelection)
                    {
                        case TitleList.NEW_GAME:
                            AudioManager.Instance.PlaySE("accept");
                            StaticValues.Reset();
                            StaticValues.isReloadACB = true;
                            loadSceneName = "Opening";
                            titleState = TitleState.FADE_OUT;
                            time = 0;
                            break;
                        case TitleList.CONTINUE:
                            if (!PlayerPrefs.HasKey("Scene"))
                            {
                                AudioManager.Instance.PlaySE("cancel");
                                return;
                            }
                            AudioManager.Instance.PlaySE("accept");
                            StaticValues.Reset();
                            StaticValues.isReloadACB = true;
                            if (PlayerPrefs.HasKey("Scene"))
                            {
                                StaticValues.Load();
                                loadSceneName = StaticValues.scene;
                                titleState = TitleState.FADE_OUT;
                                time = 0;
                            }
                            break;
                        case TitleList.EX_MODE:
                            if (!StaticValues.isCleared)
                            {
                                AudioManager.Instance.PlaySE("cancel");
                                return;
                            }
                            AudioManager.Instance.PlaySE("accept");
                            titleState = TitleState.EX_MODE_SELECT;
                            break;
                        case TitleList.OPTION:
                            AudioManager.Instance.PlaySE("accept");
                            titleState = TitleState.OPTION;
                            break;
                        case TitleList.EXIT:       
                            AudioManager.Instance.PlaySE("accept");                     
#if UNITY_EDITOR
                                UnityEditor.EditorApplication.isPlaying = false;
#elif UNITY_STANDALONE
                                UnityEngine.Application.Quit();
#endif
                            break;
                    }
                }
                break;
                
            case TitleState.NEW_GAME_SELECT: // 未使用
                break;
            case TitleState.EX_MODE_SELECT:
                if (AxisDownChecker.GetAxisDownVertical())
                {
                    AudioManager.Instance.PlaySE("select");
                    isSelectHardMode = !isSelectHardMode;

                    if (isSelectHardMode)
                    {
                        levelSelectCursor.transform.localPosition = new Vector2(levelSelecrCursorDefaultPos.x, levelSelecrCursorDefaultPos.y - 76);
                        levelDescriptionText.text = "空中ダッシュと二段ジャンプのみが開放された状態で\nすべてのボスと連戦する高難易度モードです";
                    } else
                    {
                        levelSelectCursor.transform.localPosition = new Vector2(levelSelecrCursorDefaultPos.x, levelSelecrCursorDefaultPos.y);
                        levelDescriptionText.text = "スキルツリーが全て開放された状態で\nすべてのボスと連戦するモードです";
                    }
                }
                AxisDownChecker.AxisDownUpdate();

                if (KeyConfig.GetJumpKeyDown())
                {
                    StaticValues.Reset();
                    AudioManager.Instance.PlaySE("accept");
                    StaticValues.isReloadACB = true;
                    loadSceneName = "Stage1-3";
                    titleState = TitleState.FADE_OUT;
                    time = 0;
                    StaticValues.isExMode = true;
                    StaticValues.isHardMode = isSelectHardMode;
                    if (!isSelectHardMode)
                    {
                        // EXモードかつハードモードでない時のパッシブスキルを予め適用させておく
                        StaticValues.yukariMaxHP = 200;
                        StaticValues.yukariHP = 200;
                        StaticValues.yukariMaxMP = 200;
                        StaticValues.yukariMP = 200;
                        StaticValues.yukariAttackRatio = 2.0f;
                        StaticValues.makiMaxHP = 200;
                        StaticValues.makiHP = 200;
                        StaticValues.makiMaxMP = 200;
                        StaticValues.makiMP = 200;
                        StaticValues.makiAttackRatio = 2.0f;
                    }
                }

                if (KeyConfig.GetShotKeyDown())
                {
                    AudioManager.Instance.PlaySE("cancel");
                    titleState = TitleState.TITLE;
                }
                break;
                
            case TitleState.OPTION:
                if (AxisDownChecker.GetAxisDownVertical())
                {
                    AudioManager.Instance.PlaySE("select");
                    if (Input.GetAxisRaw("Vertical") < 0) optionSelection++;
                    else if (Input.GetAxisRaw("Vertical") > 0) optionSelection--;
                    if (optionSelection > OptionList.CLOSE) optionSelection = OptionList.WINDOW_SIZE;
                    if (optionSelection < OptionList.WINDOW_SIZE) optionSelection = OptionList.CLOSE;
                    float optionCusrsorPosY = optionCursorDefaultPos.y - (38 * (int)optionSelection);
                    optionCusrsorPosY -= (optionSelection >= OptionList.BGM ? 76 : 0);
                    optionCusrsorPosY -= (optionSelection >= OptionList.KEY_CONFIG ? 38 : 0);
                    optionCusrsorPosY -= (optionSelection >= OptionList.CLOSE ? 38 : 0);
                    optionCursor.transform.localPosition = new Vector2(optionCursorDefaultPos.x, optionCusrsorPosY);

                    bool isLeftRightCursorActive = (optionSelection < OptionList.KEY_CONFIG);
                    optionLeftCursor.SetActive(isLeftRightCursorActive);
                    optionRightCursor.SetActive(isLeftRightCursorActive);
                }

                var inputHor = Input.GetAxisRaw("Horizontal");
                var inputSign = System.Math.Sign((int)inputHor);
                if ((inputHor < 0 || 0 < inputHor) && beforeInputHorSign != inputSign)
                {
                    var delta = 0.1f * (float)inputSign;
                    switch (optionSelection)
                    {
                        case OptionList.BGM:
                            StaticValues.bgmVolume += delta;
                            StaticValues.bgmVolume = Mathf.Clamp(StaticValues.bgmVolume, 0, 1.0f);
                            StaticValues.SaveVolume();
                            volumeWaitFlame = 0;
                            break;
                        case OptionList.SE:
                            StaticValues.seVolume += delta;
                            StaticValues.seVolume = Mathf.Clamp(StaticValues.seVolume, 0, 1.0f);
                            StaticValues.SaveVolume();
                            break;
                        case OptionList.VOICE:
                            StaticValues.voiceVolume += delta;
                            StaticValues.voiceVolume = Mathf.Clamp(StaticValues.voiceVolume, 0, 1.0f);
                            StaticValues.SaveVolume();
                            break;
                    }
                    beforeInputHorSign = inputSign;
                }
                if (inputHor == 0)
                {
                    beforeInputHorSign = 0;
                }

                if (optionSelection == OptionList.SE)
                {
                    if (volumeWaitFlame++ >= 200)
                    {
                        AudioManager.Instance.PlaySE("coin");
                        volumeWaitFlame = 0;
                    }
                }
                else if (optionSelection == OptionList.VOICE)
                {
                    if (volumeWaitFlame++ >= 200)
                    {
                        AudioManager.Instance.PlaySE("yukari_dash");
                        volumeWaitFlame = 0;
                    }
                }
                bgmVolumeText.text = (Mathf.Round(StaticValues.bgmVolume * 10) * 10).ToString() + "％";
                seVolumeText.text = (Mathf.Round(StaticValues.seVolume * 10 ) * 10).ToString() + "％";
                voiceVolumeText.text = (Mathf.Round(StaticValues.voiceVolume * 10) * 10).ToString() + "％";


                AxisDownChecker.AxisDownUpdate();


                if (KeyConfig.GetJumpKeyDown())
                {
                    AudioManager.Instance.PlaySE("accept");
                    switch (optionSelection)
                    {
                        case OptionList.WINDOW_SIZE:
                            break;
                        case OptionList.BGM:
                        case OptionList.SE:
                        case OptionList.VOICE:
                            break;
                        case OptionList.KEY_CONFIG:
                            titleState = TitleState.KEY_CONFIG;
                            keyConfigUIScript.StartConfig();
                            break;
                        case OptionList.CLOSE:
                            titleState = TitleState.TITLE;
                            break;
                    }
                }
                break;

            case TitleState.KEY_CONFIG:
                if (keyConfigUIScript.GetState() == KeyConfigUI.KeyConfigState.IDLE)
                {
                    titleState = TitleState.OPTION;
                }
                break;
        }
        anim.SetInteger("TitleState", (int)titleState);
        
    }

    public void PlayGame()
    {
        SceneManager.LoadScene("Stage1-0");
    }

}
