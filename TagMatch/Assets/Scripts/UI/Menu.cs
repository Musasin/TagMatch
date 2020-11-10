using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Menu : MonoBehaviour
{
    Animator anim;
    enum MenuState
    {
        CLOSED, MENU, STATUS, SKILL
    };
    MenuState menuState;
    enum MenuList
    {
        STATUS, SKILL, TITLE, CLOSE
    }
    MenuList nowSelection;

    GameObject menuCursor;
    GameObject skillSelectCursor;
    Vector2 cussorDefaultPos;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        menuState = MenuState.CLOSED;
        nowSelection = MenuList.STATUS;
        menuCursor = GameObject.Find("MenuCursor");
        cussorDefaultPos = menuCursor.transform.localPosition;
        skillSelectCursor = GameObject.Find("SkillSelectCursor");
        skillSelectCursor.GetComponent<SkillSelect>().SetEnabled(false);
        
        // ポーズ中でも動かせるようにタイムスケールを無視する
        anim.updateMode = AnimatorUpdateMode.UnscaledTime;
    }

    // Update is called once per frame
    void Update()
    {
        if (menuState == MenuState.CLOSED)
        {
            StaticValues.isPause = false;
            Time.timeScale = 1.0f;
        } else
        {
            StaticValues.isPause = true;
            Time.timeScale = 0;
        }

        switch (menuState)
        {
            case MenuState.CLOSED:
                if (Input.GetButtonDown("Menu"))
                {
                    menuState = MenuState.MENU;
                    anim.SetInteger("menuState", (int)menuState);
                }
                break;

            case MenuState.MENU:
                if (AxisDownChecker.GetAxisDownVertical())
                {
                    if (Input.GetAxisRaw("Vertical") < 0) nowSelection++;
                    else if (Input.GetAxisRaw("Vertical") > 0) nowSelection--;
                    if (nowSelection > MenuList.CLOSE) nowSelection = MenuList.STATUS;
                    if (nowSelection < MenuList.STATUS) nowSelection = MenuList.CLOSE;
                    menuCursor.transform.localPosition = new Vector2(cussorDefaultPos.x, cussorDefaultPos.y - (50 * (int)nowSelection) - (nowSelection == MenuList.CLOSE ? 50 :0));
                }
                AxisDownChecker.AxisDownUpdate();

                if (Input.GetButtonDown("Jump"))
                {
                    switch (nowSelection)
                    {
                        case MenuList.STATUS:
                            menuState = MenuState.STATUS;
                            break;
                        case MenuList.SKILL:
                            menuState = MenuState.SKILL;
                            break;
                        case MenuList.TITLE:
                            // TODO 後で作る
                            menuState = MenuState.CLOSED;
                            break;
                        case MenuList.CLOSE:
                            menuState = MenuState.CLOSED;
                            break;
                    }
                    anim.SetInteger("menuState", (int)menuState);
                }
                if (Input.GetButtonDown("Shot") || Input.GetButtonDown("Menu"))
                {
                    menuState = MenuState.CLOSED;
                    anim.SetInteger("menuState", (int)menuState);
                }
                break;
                
            case MenuState.STATUS:
                if (Input.GetButtonDown("Jump") || Input.GetButtonDown("Shot") || Input.GetButtonDown("Menu"))
                {
                    menuState = MenuState.MENU;
                    anim.SetInteger("menuState", (int)menuState);
                }
                break;
                
            case MenuState.SKILL:
                if (Input.GetButtonDown("Shot") || Input.GetButtonDown("Menu"))
                {
                    skillSelectCursor.GetComponent<SkillSelect>().SetEnabled(false);
                    menuState = MenuState.MENU;
                    anim.SetInteger("menuState", (int)menuState);
                }
                break;
        }
    }

    public void SetSkillPanelEnabled()
    {
        skillSelectCursor.GetComponent<SkillSelect>().SetEnabled(true);
    }
}
