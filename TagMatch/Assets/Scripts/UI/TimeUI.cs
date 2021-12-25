using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeUI : MonoBehaviour
{
    Text t;

    // Start is called before the first frame update
    void Start()
    {
        t = GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!StaticValues.isPause && !StaticValues.isTalkPause)
        {
            StaticValues.time += Time.deltaTime;
        }
        int intTime = (int)StaticValues.time;
        int min = intTime / 60;
        int sec = intTime % 60;
        string strMin = (min < 10) ? "0" + min.ToString() : min.ToString();
        string strSec = (sec < 10) ? "0" + sec.ToString() : sec.ToString();
        t.text = strMin + ":" + strSec;
    }
}
