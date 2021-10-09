using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    [SerializeField]
    Text yukariStatusText, makiStatusText, yukariSkillText, makiSkillText;

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
                UpdateStatusText();
                if (AxisDownChecker.GetAxisDownVertical())
                {
                    AudioManager.Instance.PlaySE("select");
                    if (Input.GetAxisRaw("Vertical") < 0) nowSelection++;
                    else if (Input.GetAxisRaw("Vertical") > 0) nowSelection--;
                    if (nowSelection > MenuList.CLOSE) nowSelection = MenuList.STATUS;
                    if (nowSelection < MenuList.STATUS) nowSelection = MenuList.CLOSE;
                    menuCursor.transform.localPosition = new Vector2(cussorDefaultPos.x, cussorDefaultPos.y - (50 * (int)nowSelection) - (nowSelection == MenuList.CLOSE ? 50 : 0));
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

    private void UpdateStatusText()
    {
        yukariStatusText.text = "　　　【STATUS】\n";
        yukariStatusText.text += "HP: " + StaticValues.yukariHP + "/" + StaticValues.yukariMaxHP + "\n";
        yukariStatusText.text += "MP: " + StaticValues.yukariMP + "/" + StaticValues.yukariMaxMP + "\n";
        yukariStatusText.text += "武器: スターシューター\n";
        yukariStatusText.text += "威力: " + (int)(10 * StaticValues.yukariAttackRatio) + "\n";
        yukariStatusText.text += "連射数: 5\n";
        
        makiStatusText.text = "　　　【STATUS】\n";
        makiStatusText.text += "HP: " + StaticValues.makiHP + "/" + StaticValues.makiMaxHP + "\n";
        makiStatusText.text += "MP: " + StaticValues.makiMP + "/" + StaticValues.makiMaxMP + "\n";
        makiStatusText.text += "武器: むすタン\n";
        makiStatusText.text += "威力: " + (int)(15 * StaticValues.makiAttackRatio) + "\n";
        makiStatusText.text += "連射数: 2\n";

        yukariSkillText.text = "　　　【SKILL】\n";
        if (StaticValues.GetSkill("y_dash_1")) yukariSkillText.text += "・空中ダッシュ\n";
        if (StaticValues.GetSkill("y_dash_2")) yukariSkillText.text += "・ダッシュ中無敵\n";
        if (StaticValues.GetSkill("y_shot_1")) yukariSkillText.text += "・対空射撃\n";
        if (StaticValues.GetSkill("y_shot_2")) yukariSkillText.text += "・対地射撃\n";
        if (StaticValues.GetSkill("y_down_1")) yukariSkillText.text += "・低姿勢射撃\n";
        if (StaticValues.GetSkill("y_down_2")) yukariSkillText.text += "・精密射撃\n";
        
        makiSkillText.text = "　　　【SKILL】\n";
        if (StaticValues.GetSkill("m_jump_1")) makiSkillText.text += "・二段ジャンプ\n";
        if (StaticValues.GetSkill("m_jump_2")) makiSkillText.text += "・くるりんジャンプ\n";
        if (StaticValues.GetSkill("m_shot_1")) makiSkillText.text += "・エレキバリヤー\n";
        if (StaticValues.GetSkill("m_shot_2")) makiSkillText.text += "・グレート\n　エレキファイヤー\n";
        if (StaticValues.GetSkill("m_down_1")) makiSkillText.text += "・ヒール\n";
        if (StaticValues.GetSkill("m_down_2")) makiSkillText.text += "・かくれる\n";
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
