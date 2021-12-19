using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Ending : MonoBehaviour
{
    TextMeshProUGUI timeText, coinText, deadText, calcText, scoreText, skipText;

    // Start is called before the first frame update
    void Start()
    {
        AudioManager.Instance.LoadACB("Ending", "Ending.acb", "Ending.awb");
        AudioManager.Instance.PlayBGM("ending");
        
        timeText = GameObject.Find("TimeText").GetComponent<TextMeshProUGUI>();
        coinText = GameObject.Find("CoinText").GetComponent<TextMeshProUGUI>();
        deadText = GameObject.Find("DeadText").GetComponent<TextMeshProUGUI>();
        calcText = GameObject.Find("CalcText").GetComponent<TextMeshProUGUI>();
        scoreText = GameObject.Find("ScoreText").GetComponent<TextMeshProUGUI>();
        skipText = GameObject.Find("SkipText").GetComponent<TextMeshProUGUI>();

        
        int intTime = (int)StaticValues.time;
        int min = intTime / 60;
        int sec = intTime % 60;
        string strMin = (min < 10) ? "0" + min.ToString() : min.ToString();
        string strSec = (sec < 10) ? "0" + sec.ToString() : sec.ToString();
        timeText.text = strMin + ":" + strSec;
        
        coinText.text = StaticValues.maxCoinCount.ToString() + "枚";
        deadText.text = StaticValues.deadCount.ToString() + "回";
        
        int timeScore = Mathf.Max(0, 7200 - intTime) * 100;
        int coinScore = StaticValues.maxCoinCount * 100;
        int deadScore = StaticValues.maxCoinCount * 2000;
        int totalScore = timeScore + coinScore - deadScore;
        
        calcText.text = "Time: +" + timeScore.ToString() + "\n";
        calcText.text += "Coin: +" + coinScore.ToString() + "\n";
        calcText.text += "Dead: -" + deadScore.ToString() + "\n";
        calcText.text += "\nTotal:";
        scoreText.text = totalScore.ToString();

        skipText.text = "[" + KeyConfig.GetTextFromKeyCode(KeyConfig.menuKey) + "]長押し: スキップ";
    }

    // Update is called once per frame
    void Update()
    {
        if (KeyConfig.GetMenuKey())
        {
            Time.timeScale = 5.0f;
        } else
        {
            Time.timeScale = 1.0f;
        }
        
    }
}
