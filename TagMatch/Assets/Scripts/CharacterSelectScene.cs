using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CharacterSelectScene : MonoBehaviour
{
    [SerializeField]
    Animator yukariAnimator, makiAnimator;

    [SerializeField]
    GameObject grayOutObject, startTextObject;

    [SerializeField]
    Image fadePanel;
    
    enum CharacterSelectState
    {
        FADE_IN, SELECT_CHARACTER, FADE_OUT, EXIT
    }
    CharacterSelectState state = CharacterSelectState.FADE_IN;

    float time;
    bool isYukari;
    bool isMaki;
    TextMeshProUGUI startText;

    // Start is called before the first frame update
    void Start()
    {
        startText = startTextObject.GetComponent<TextMeshProUGUI>();
        grayOutObject.SetActive(false);
        startTextObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;
        switch (state)
        {
            case CharacterSelectState.FADE_IN:
                fadePanel.color = new Color(0, 0, 0, 1.0f - time);
                if (time > 1.0f)
                {
                    state = CharacterSelectState.SELECT_CHARACTER;
                }
                break;
            case CharacterSelectState.SELECT_CHARACTER:
                if (AxisDownChecker.GetAxisDownHorizontal())
                {
                    AudioManager.Instance.PlaySE("select");
                    if (Input.GetAxisRaw("Horizontal") < 0)
                    {
                        isYukari = true;
                        isMaki = false;
                    }
                    else if (Input.GetAxisRaw("Horizontal") > 0)
                    {
                        isYukari = false;
                        isMaki = true;
                    }

                    if (isYukari || isMaki)
                    {
                        grayOutObject.SetActive(true);
                        startTextObject.SetActive(true);
                        startText.text = "PRESS [" + KeyConfig.GetTextFromKeyCode(KeyConfig.jumpKey) + "] START";
                        grayOutObject.transform.localPosition = new Vector2(isYukari ? 200 : -200, -30);
                        if (isYukari)
                        {
                            yukariAnimator.SetBool("isRun", true);
                            makiAnimator.SetBool("isRun", false);
                        }
                        else
                        {
                            yukariAnimator.SetBool("isRun", false);
                            makiAnimator.SetBool("isRun", true);
                        }
                    }
                }
                AxisDownChecker.AxisDownUpdate();
                if (KeyConfig.GetJumpKeyDown())
                {
                    if (isYukari || isMaki)
                    {
                        AudioManager.Instance.PlaySE("accept");
                        time = 0;
                        state = CharacterSelectState.FADE_OUT;
                    } else
                    {
                        AudioManager.Instance.PlaySE("cancel");
                    }
                }
                break;
            case CharacterSelectState.FADE_OUT:
                fadePanel.color = new Color(0, 0, 0, 0.0f + time);
                if (time > 1.0f)
                {
                    // 死んでると困るので全回復させちゃう
                    StaticValues.yukariHP = StaticValues.yukariMaxHP;
                    StaticValues.yukariMP = StaticValues.yukariMaxMP;
                    StaticValues.makiHP = StaticValues.makiMaxHP;
                    StaticValues.makiMP = StaticValues.makiMaxMP;

                    // 選択した方だけを使える状態にしてシーン遷移
                    if (isYukari)
                    {
                        StaticValues.stage6SelectChara = StaticValues.SwitchState.YUKARI;
                        SceneManager.LoadScene("Stage6-Y");
                    }
                    else
                    {
                        StaticValues.stage6SelectChara = StaticValues.SwitchState.MAKI;
                        SceneManager.LoadScene("Stage6-M");
                    }
                    state = CharacterSelectState.EXIT;
                }
                break;
        }
    }
}
