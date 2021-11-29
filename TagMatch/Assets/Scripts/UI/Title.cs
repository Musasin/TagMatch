using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Title : MonoBehaviour
{
    float time;
    Animator anim;
    Image fadePanel;
    GameObject titleCursor;
    GameObject optionCursor, optionLeftCursor, optionRightCursor;
    Text bgmVolumeText, seVolumeText, voiceVolumeText;
    Vector2 titleCursorDefaultPos;
    Vector2 optionCursorDefaultPos;
    KeyConfigUI keyConfigUIScript;
    string loadSceneName;

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

    // Start is called before the first frame update
    void Start()
    {
        StaticValues.LoadVolume();
        time = 0;
        anim = GetComponent<Animator>();
        fadePanel = GameObject.Find("FadePanel").GetComponent<Image>();
        titleCursor = GameObject.Find("TitleCursor");
        optionCursor = GameObject.Find("OptionCursor");
        optionLeftCursor = GameObject.Find("OptionLeftCursor");
        optionRightCursor = GameObject.Find("OptionRightCursor");
        titleCursorDefaultPos = titleCursor.transform.localPosition;
        optionCursorDefaultPos = optionCursor.transform.localPosition;
        keyConfigUIScript = GameObject.Find("KeyConfig").GetComponent<KeyConfigUI>();

        bgmVolumeText = GameObject.Find("BGMVolume").GetComponent<Text>();
        seVolumeText = GameObject.Find("SEVolume").GetComponent<Text>();
        voiceVolumeText = GameObject.Find("VoiceVolume").GetComponent<Text>();
        
        if (StaticValues.isReloadACB == false) { return; }
        AudioManager.Instance.LoadACB("Title", "Title.acb", "Title.awb");
        StaticValues.isReloadACB = false;

        StaticValues.isReloadACB = false;

        if (PlayerPrefs.HasKey("Scene"))
        {
            nowSelection = TitleList.CONTINUE;
            titleCursor.transform.localPosition = new Vector2(titleCursorDefaultPos.x, titleCursorDefaultPos.y - (67 * (int)nowSelection));
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
                    AudioManager.Instance.PlaySE("accept");
                    switch (nowSelection)
                    {
                        case TitleList.NEW_GAME:
                            StaticValues.isReloadACB = true;
                            loadSceneName = "Opening";
                            titleState = TitleState.FADE_OUT;
                            time = 0;
                            break;
                        case TitleList.CONTINUE:
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
                            // TODO
                            break;
                        case TitleList.OPTION:
                            titleState = TitleState.OPTION;
                            // TODO
                            break;
                        case TitleList.EXIT:                            
#if UNITY_EDITOR
                                UnityEditor.EditorApplication.isPlaying = false;
#elif UNITY_STANDALONE
                                UnityEngine.Application.Quit();
#endif
                            break;
                    }
                }
                break;
                
            case TitleState.NEW_GAME_SELECT:
            case TitleState.EX_MODE_SELECT:
                // そのうち難易度選択画面が必要になる
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

                if (Input.GetAxisRaw("Horizontal") < 0 || Input.GetAxisRaw("Horizontal") > 0)
                {
                    switch (optionSelection)
                    {
                        case OptionList.BGM:
                            if (Input.GetAxisRaw("Horizontal") < 0) StaticValues.bgmVolume -= 0.01f;
                            else if (Input.GetAxisRaw("Horizontal") > 0) StaticValues.bgmVolume += 0.01f;
                            StaticValues.bgmVolume = Mathf.Clamp(StaticValues.bgmVolume, 0, 1.0f);
                            StaticValues.SaveVolume();
                            break;
                        case OptionList.SE:
                            if (Input.GetAxisRaw("Horizontal") < 0) StaticValues.seVolume -= 0.01f;
                            else if (Input.GetAxisRaw("Horizontal") > 0) StaticValues.seVolume += 0.01f;
                            StaticValues.seVolume = Mathf.Clamp(StaticValues.seVolume, 0, 1.0f);
                            StaticValues.SaveVolume();
                            if (Mathf.Floor(StaticValues.seVolume * 100) % 3 == 0) AudioManager.Instance.PlaySE("select");
                            break;
                        case OptionList.VOICE:
                            if (Input.GetAxisRaw("Horizontal") < 0) StaticValues.voiceVolume -= 0.01f;
                            else if (Input.GetAxisRaw("Horizontal") > 0) StaticValues.voiceVolume += 0.01f;
                            StaticValues.voiceVolume = Mathf.Clamp(StaticValues.voiceVolume, 0, 1.0f);
                            StaticValues.SaveVolume();
                            if (Mathf.Floor(StaticValues.voiceVolume * 100) % 3 == 0) AudioManager.Instance.PlaySE("yukari_dash");
                            break;
                    }
                }
                bgmVolumeText.text = Mathf.Floor(StaticValues.bgmVolume * 100).ToString() + "％";
                seVolumeText.text = Mathf.Floor(StaticValues.seVolume * 100).ToString() + "％";
                voiceVolumeText.text = Mathf.Floor(StaticValues.voiceVolume * 100).ToString() + "％";


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
