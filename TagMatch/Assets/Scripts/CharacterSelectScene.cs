﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

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
                    AudioManager.Instance.PlaySE("accept");
                    time = 0;
                    state = CharacterSelectState.FADE_OUT;
                }
                break;
            case CharacterSelectState.FADE_OUT:
                fadePanel.color = new Color(0, 0, 0, 0.0f + time);
                if (time > 1.0f)
                {
                    // 選択した方だけを使える状態にしてシーン遷移
                }
                break;
        }
    }
}
