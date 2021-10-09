using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Menu : MonoBehaviour
{
    Animator anim;
    enum MenuState
    {
        CLOSED, MENU, STATUS, SKILL, CLOSING
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
    float closeTime;

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
        switch (menuState)
        {
            case MenuState.CLOSED:
                if (KeyConfig.GetMenuKeyDown())
                {
                    AudioManager.Instance.PlaySE("open_menu");
                    StaticValues.isPause = true;
                    Time.timeScale = 0;
                    menuState = MenuState.MENU;
                    anim.SetInteger("menuState", (int)menuState);
                }
                break;

            case MenuState.MENU:
                if (AxisDownChecker.GetAxisDownVertical())
                {
                    AudioManager.Instance.PlaySE("select");
                    if (Input.GetAxisRaw("Vertical") < 0) nowSelection++;
                    else if (Input.GetAxisRaw("Vertical") > 0) nowSelection--;
                    if (nowSelection > MenuList.CLOSE) nowSelection = MenuList.STATUS;
                    if (nowSelection < MenuList.STATUS) nowSelection = MenuList.CLOSE;
                    menuCursor.transform.localPosition = new Vector2(cussorDefaultPos.x, cussorDefaultPos.y - (50 * (int)nowSelection) - (nowSelection == MenuList.CLOSE ? 50 :0));
                }
                AxisDownChecker.AxisDownUpdate();

                if (KeyConfig.GetJumpKeyDown())
                {
                    AudioManager.Instance.PlaySE("accept");
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
                            CloseMenu();
                            break;
                        case MenuList.CLOSE:
                            CloseMenu();
                            break;
                    }
                    anim.SetInteger("menuState", (int)menuState);
                }
                if (KeyConfig.GetShotKeyDown() || KeyConfig.GetMenuKeyDown())
                {
                    CloseMenu();
                    anim.SetInteger("menuState", (int)menuState);
                }
                break;
                
            case MenuState.STATUS:
                if (KeyConfig.GetJumpKeyDown() || KeyConfig.GetShotKeyDown() || KeyConfig.GetMenuKeyDown())
                {
                    AudioManager.Instance.PlaySE("cancel");
                    menuState = MenuState.MENU;
                    anim.SetInteger("menuState", (int)menuState);
                }
                break;
                
            case MenuState.SKILL:
                if (KeyConfig.GetShotKeyDown() || KeyConfig.GetMenuKeyDown())
                {
                    AudioManager.Instance.PlaySE("cancel");
                    skillSelectCursor.GetComponent<SkillSelect>().SetEnabled(false);
                    menuState = MenuState.MENU;
                    anim.SetInteger("menuState", (int)menuState);
                }
                break;

            case MenuState.CLOSING:
                closeTime -= Time.deltaTime;
                anim.SetInteger("menuState", (int)MenuState.CLOSED);
                if (closeTime <= 0)
                {
                    menuState = MenuState.CLOSED;
                    StaticValues.isPause = false;
                    Time.timeScale = 1.0f;
                    nowSelection = 0;
                    menuCursor.transform.localPosition = new Vector2(cussorDefaultPos.x, cussorDefaultPos.y);
                }
                break;
        }
    }

    private void CloseMenu()
    {
        AudioManager.Instance.PlaySE("close_menu");
        menuState = MenuState.CLOSING;
        Time.timeScale = 1.0f;
        closeTime = 0.1f;
    }

    public void SetSkillPanelEnabled()
    {
        skillSelectCursor.GetComponent<SkillSelect>().SetEnabled(true);
    }
}
