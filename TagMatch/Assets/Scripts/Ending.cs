using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class Ending : MonoBehaviour
{
    Animator anim;
    bool canChangeScene;
    TextMeshProUGUI modeText, timeText, coinText, maxCoinText, deadText, calcText, scoreText, skipText, playNextText;

    // Start is called before the first frame update
    void Start()
    {
        StaticValues.ClearedSave(); // エンディング開始時点でクリアフラグを保存しておく

        anim = GetComponent<Animator>();
        AudioManager.Instance.LoadACB("Ending", "Ending.acb", "Ending.awb");
        AudioManager.Instance.PlayBGM("ending");
        
        modeText = GameObject.Find("ModeText").GetComponent<TextMeshProUGUI>();
        timeText = GameObject.Find("TimeText").GetComponent<TextMeshProUGUI>();
        coinText = GameObject.Find("CoinText").GetComponent<TextMeshProUGUI>();
        maxCoinText = GameObject.Find("MaxCoinText").GetComponent<TextMeshProUGUI>();
        deadText = GameObject.Find("DeadText").GetComponent<TextMeshProUGUI>();
        calcText = GameObject.Find("CalcText").GetComponent<TextMeshProUGUI>();
        scoreText = GameObject.Find("ScoreText").GetComponent<TextMeshProUGUI>();
        skipText = GameObject.Find("SkipText").GetComponent<TextMeshProUGUI>();
        playNextText = GameObject.Find("PlayNextText").GetComponent<TextMeshProUGUI>();

        modeText.text = StaticValues.isHardMode ? "持たざる者モード\nx2" : (StaticValues.isExMode ? "全力バトルモード\nx1.5" : ""); 

        int intTime = (int)StaticValues.time;
        int min = intTime / 60;
        int sec = intTime % 60;
        string strMin = (min < 10) ? "0" + min.ToString() : min.ToString();
        string strSec = (sec < 10) ? "0" + sec.ToString() : sec.ToString();
        timeText.text = strMin + ":" + strSec;
        
        coinText.text = StaticValues.maxCoinCount.ToString() + "枚";
        maxCoinText.text = StaticValues.isExMode ? "/0" : "/3000";
        deadText.text = StaticValues.deadCount.ToString() + "回";
        
        int timeScore = Mathf.Max(0, 7200 - intTime) * 100;
        int coinScore = StaticValues.maxCoinCount * 100;
        int deadScore = StaticValues.deadCount * 5000;
        float totalScoreF = timeScore + coinScore - deadScore;
        totalScoreF *= StaticValues.isHardMode ? 2 : (StaticValues.isExMode ? 1.5f : 1);
        int totalScore = (int)Mathf.Floor(totalScoreF);
        
        calcText.text = "Time: +" + timeScore.ToString() + "\n";
        calcText.text += "Coin: +" + coinScore.ToString() + "\n";
        calcText.text += "Dead: -" + deadScore.ToString() + "\n";
        calcText.text += "\nTotal:";
        scoreText.text = totalScore.ToString();

        skipText.text = "[" + KeyConfig.GetTextFromKeyCode(KeyConfig.menuKey) + "]長押し: スキップ";

        playNextText.text = "[" + KeyConfig.GetTextFromKeyCode(KeyConfig.jumpKey) + "]: 進む";
    }

    // Update is called once per frame
    void Update()
    {
        if (KeyConfig.GetMenuKey() && !canChangeScene)
        {
            Time.timeScale = 5.0f;
        } else
        {
            Time.timeScale = 1.0f;
        }

        if (canChangeScene && KeyConfig.GetJumpKeyDown())
        {
            anim.SetTrigger("Press");
        }
    }

    public void SetCanChangeSceneFlag()
    {
        canChangeScene = true;
    }
    
    public void ChangeScene()
    {
        // エピローグではEndingのACBをそのまま使う
        StaticValues.isReloadACB = false;

        // EXモードは直接タイトルに戻す
        if (StaticValues.isExMode)
        {
            StaticValues.isReloadACB = true;
            SceneManager.LoadScene("Title");
        }
        // 6面で選択したキャラによってエピローグを出し分ける
        else if (StaticValues.stage6SelectChara == StaticValues.SwitchState.YUKARI)
        {
            SceneManager.LoadScene("Epilogue-Y");
        } else
        {
            SceneManager.LoadScene("Epilogue-M");
        }
    }
}
