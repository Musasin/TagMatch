using UnityEngine;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    [SerializeField]
    Text yukariStatusText, makiStatusText, yukariSkillText, makiSkillText;

    Animator anim;
    enum MenuState
    {
        CLOSED, MENU, STATUS, SKILL, VOLUME, TITLE_CHECK, CLOSING, TITLE_CLOSING,
    };
    MenuState menuState;
    enum MenuList
    {
        STATUS, SKILL, VOLUME, TITLE, CLOSE,
    }
    MenuList nowSelection;
    enum VolumeList
    {
        BGM, SE, VOICE, CLOSE,
    }
    VolumeList volumeNowSelection;
    bool returnTitleNowSelection = false;
    
    GameObject menuCursor, volumeCursor, returnTitleCursor, skillSelectCursor;
    Vector2 cursorDefaultPos, volumeCursorDefaultPos, returnTitleCursorDefaultPos;
    Text bgmVolumeText, seVolumeText, voiceVolumeText;
    float closeTime;
    private int volumeWaitFlame;
    private int beforeInputHorSign;

    // Start is called before the first frame update
    void Start()
    {
        volumeWaitFlame = 0;
        beforeInputHorSign = 0;
        StaticValues.LoadVolume();
        anim = GetComponent<Animator>();
        menuState = MenuState.CLOSED;
        nowSelection = MenuList.STATUS;
        volumeNowSelection = VolumeList.BGM;
        
        menuCursor = GameObject.Find("MenuCursor");
        cursorDefaultPos = menuCursor.transform.localPosition;
        
        volumeCursor = GameObject.Find("VolumeMenuCursor");
        volumeCursorDefaultPos = volumeCursor.transform.localPosition;

        returnTitleCursor = GameObject.Find("ReturnTitleCursor");
        returnTitleCursorDefaultPos = returnTitleCursor.transform.localPosition;
        
        skillSelectCursor = GameObject.Find("SkillSelectCursor");
        skillSelectCursor.GetComponent<SkillSelect>().SetEnabled(false);
        
        bgmVolumeText = GameObject.Find("BGMVolume").GetComponent<Text>();
        seVolumeText = GameObject.Find("SEVolume").GetComponent<Text>();
        voiceVolumeText = GameObject.Find("VoiceVolume").GetComponent<Text>();
        
        // ポーズ中でも動かせるようにタイムスケールを無視する
        anim.updateMode = AnimatorUpdateMode.UnscaledTime;
    }

    // Update is called once per frame
    void Update()
    {
        switch (menuState)
        {
            case MenuState.CLOSED:
                if (KeyConfig.GetMenuKeyDown() && !StaticValues.isTalkPause)
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
                    menuCursor.transform.localPosition = new Vector2(cursorDefaultPos.x, cursorDefaultPos.y - (50 * (int)nowSelection) - (nowSelection == MenuList.CLOSE ? 50 : 0));
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
                        case MenuList.VOLUME:
                            menuState = MenuState.VOLUME;
                            break;
                        case MenuList.TITLE:
                            menuState = MenuState.TITLE_CHECK;
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

            case MenuState.VOLUME:
                if (KeyConfig.GetJumpKeyDown() || KeyConfig.GetShotKeyDown() || KeyConfig.GetMenuKeyDown())
                {
                    AudioManager.Instance.PlaySE("cancel");
                    menuState = MenuState.MENU;
                    anim.SetInteger("menuState", (int)menuState);
                }

                if (AxisDownChecker.GetAxisDownVertical())
                {
                    AudioManager.Instance.PlaySE("select");
                    if (Input.GetAxisRaw("Vertical") < 0) volumeNowSelection++;
                    else if (Input.GetAxisRaw("Vertical") > 0) volumeNowSelection--;
                    if (volumeNowSelection > VolumeList.CLOSE) volumeNowSelection = VolumeList.BGM;
                    if (volumeNowSelection < VolumeList.BGM) volumeNowSelection = VolumeList.CLOSE;
                    volumeCursor.transform.localPosition = new Vector2(volumeCursorDefaultPos.x, volumeCursorDefaultPos.y - (50 * (int)volumeNowSelection) - (volumeNowSelection == VolumeList.CLOSE ? 50 : 0));
                }

                var inputHor = Input.GetAxisRaw("Horizontal");
                var inputSign = System.Math.Sign((int)inputHor);
                if ((inputHor < 0 || 0 < inputHor) && beforeInputHorSign != inputSign)
                {
                    var delta = 0.1f * (float)inputSign;
                    switch (volumeNowSelection)
                    {
                        case VolumeList.BGM:
                            StaticValues.bgmVolume += delta;
                            StaticValues.bgmVolume = Mathf.Clamp(StaticValues.bgmVolume, 0, 1.0f);
                            StaticValues.SaveVolume();
                            volumeWaitFlame = 0;
                            break;
                        case VolumeList.SE:
                            StaticValues.seVolume += delta;
                            StaticValues.seVolume = Mathf.Clamp(StaticValues.seVolume, 0, 1.0f);
                            StaticValues.SaveVolume();
                            break;
                        case VolumeList.VOICE:
                            StaticValues.voiceVolume += delta;
                            StaticValues.voiceVolume = Mathf.Clamp(StaticValues.voiceVolume, 0, 1.0f);
                            StaticValues.SaveVolume();
                            break;
                        case VolumeList.CLOSE:
                            break;
                    }
                    beforeInputHorSign = inputSign;
                }
                if(inputHor == 0){
                    beforeInputHorSign = 0;
                }

                if (volumeNowSelection == VolumeList.SE)
                {
                    if (volumeWaitFlame++ >= 200)
                    {
                        AudioManager.Instance.PlaySE("coin");
                        volumeWaitFlame = 0;
                    }
                }
                else if (volumeNowSelection == VolumeList.VOICE)
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
                break;

            case MenuState.TITLE_CHECK:
                if (AxisDownChecker.GetAxisDownHorizontal())
                {
                    AudioManager.Instance.PlaySE("select");
                    returnTitleNowSelection = !returnTitleNowSelection;
                    returnTitleCursor.transform.localPosition = new Vector2(returnTitleCursorDefaultPos.x + (returnTitleNowSelection ? -135 : 0), returnTitleCursorDefaultPos.y);
                }
                AxisDownChecker.AxisDownUpdate();

                if (KeyConfig.GetShotKeyDown() || KeyConfig.GetMenuKeyDown() || KeyConfig.GetJumpKeyDown() && !returnTitleNowSelection)
                {
                    AudioManager.Instance.PlaySE("cancel");
                    skillSelectCursor.GetComponent<SkillSelect>().SetEnabled(false);
                    menuState = MenuState.MENU;
                    anim.SetInteger("menuState", (int)menuState);
                }

                if (KeyConfig.GetJumpKeyDown() && returnTitleNowSelection)
                {
                    AudioManager.Instance.PlaySE("accept");
                    Time.timeScale = 1.0f;
                    StaticValues.isReloadACB = true;
                    GameObject.Find("WipePanel").GetComponent<WipePanel>().ChangeScene("Title");
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
                    menuCursor.transform.localPosition = new Vector2(cursorDefaultPos.x, cursorDefaultPos.y);
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
