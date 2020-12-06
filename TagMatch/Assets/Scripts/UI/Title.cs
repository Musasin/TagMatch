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
                    titleCursor.transform.localPosition = new Vector2(titleCursorDefaultPos.x, titleCursorDefaultPos.y - (78 * (int)nowSelection));
                }
                AxisDownChecker.AxisDownUpdate();

                if (KeyConfig.GetJumpKeyDown())
                {
                    switch (nowSelection)
                    {
                        case TitleList.NEW_GAME:
                            SceneManager.LoadScene("Stage0-1");
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
                    optionCusrsorPosY -= (optionSelection >= OptionList.BGM ? 38 : 0);
                    optionCusrsorPosY -= (optionSelection >= OptionList.KEY_CONFIG ? 38 : 0);
                    optionCusrsorPosY -= (optionSelection >= OptionList.CLOSE ? 38 : 0);
                    optionCursor.transform.localPosition = new Vector2(optionCursorDefaultPos.x, optionCusrsorPosY);
                }
                AxisDownChecker.AxisDownUpdate();
                break;
            case TitleState.KEY_CONFIG:
                break;
        }
        
    }

    public void PlayGame()
    {
        SceneManager.LoadScene("Stage0-1");
    }
}
