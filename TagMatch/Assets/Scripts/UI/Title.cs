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
        // 仮。 Talkにもあるので終わったら統合する
        AudioManager.Instance.ChangeBGMVolume(0.4f);
        AudioManager.Instance.ChangeSEVolume(0.4f);
        AudioManager.Instance.ChangeVoiceVolume(0.4f);

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
        
        if (StaticValues.isReloadACB == false) { return; }
        AudioManager.Instance.LoadACB("Title", "Title.acb");
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
