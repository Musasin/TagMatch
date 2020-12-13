using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Title : MonoBehaviour
{
    Animator anim;
    GameObject titleCursor;
    GameObject optionCursor;
    Vector2 titleCursorDefaultPos;
    Vector2 optionCursorDefaultPos;
    KeyConfigUI keyConfigUIScript;

    enum TitleState
    {
        TITLE, NEW_GAME_SELECT, EX_MODE_SELECT, OPTION, KEY_CONFIG
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
        anim = GetComponent<Animator>();
        titleCursor = GameObject.Find("TitleCursor");
        optionCursor = GameObject.Find("OptionCursor");
        titleCursorDefaultPos = titleCursor.transform.localPosition;
        optionCursorDefaultPos = optionCursor.transform.localPosition;
        keyConfigUIScript = GameObject.Find("KeyConfig").GetComponent<KeyConfigUI>();
    }

    // Update is called once per frame
    void Update()
    {
        switch (titleState)
        {
            case TitleState.TITLE:
                if (AxisDownChecker.GetAxisDownVertical())
                {
                    if (Input.GetAxisRaw("Vertical") < 0) nowSelection++;
                    else if (Input.GetAxisRaw("Vertical") > 0) nowSelection--;
                    if (nowSelection > TitleList.EXIT) nowSelection = TitleList.NEW_GAME;
                    if (nowSelection < TitleList.NEW_GAME) nowSelection = TitleList.EXIT;
                    titleCursor.transform.localPosition = new Vector2(titleCursorDefaultPos.x, titleCursorDefaultPos.y - (76 * (int)nowSelection));
                }
                AxisDownChecker.AxisDownUpdate();

                if (KeyConfig.GetJumpKeyDown())
                {
                    switch (nowSelection)
                    {
                        case TitleList.NEW_GAME:
                            SceneManager.LoadScene("Stage1-0");
                            break;
                        case TitleList.CONTINUE:
                            // TODO
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
                    if (Input.GetAxisRaw("Vertical") < 0) optionSelection++;
                    else if (Input.GetAxisRaw("Vertical") > 0) optionSelection--;
                    if (optionSelection > OptionList.CLOSE) optionSelection = OptionList.WINDOW_SIZE;
                    if (optionSelection < OptionList.WINDOW_SIZE) optionSelection = OptionList.CLOSE;
                    float optionCusrsorPosY = optionCursorDefaultPos.y - (38 * (int)optionSelection);
                    optionCusrsorPosY -= (optionSelection >= OptionList.BGM ? 76 : 0);
                    optionCusrsorPosY -= (optionSelection >= OptionList.KEY_CONFIG ? 38 : 0);
                    optionCusrsorPosY -= (optionSelection >= OptionList.CLOSE ? 38 : 0);
                    optionCursor.transform.localPosition = new Vector2(optionCursorDefaultPos.x, optionCusrsorPosY);
                }
                AxisDownChecker.AxisDownUpdate();


                if (KeyConfig.GetJumpKeyDown())
                {
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
