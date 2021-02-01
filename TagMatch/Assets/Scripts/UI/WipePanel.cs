using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WipePanel : MonoBehaviour
{
    Animator anim;
    bool isWipeOutPlaying;
    float wipeTime;
    string nextScene;
    bool isStopBGM;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        anim.SetBool("WipeIn", true);
        anim.SetBool("WipeOut", false);
    }

    // Update is called once per frame
    void Update()
    {
        if(isWipeOutPlaying)
        {
            wipeTime += Time.deltaTime;
            if (wipeTime > 1)
            {
                if (isStopBGM)
                    AudioManager.Instance.StopBGM();
                StaticValues.isPause = false;
                SceneManager.LoadScene(nextScene);
            }
        }
    }

    public void ChangeScene(string sceneName, bool isStopBGM = false)
    {
        if (!isWipeOutPlaying)
        {
            StaticValues.isPause = true;
            isWipeOutPlaying = true;
            wipeTime = 0;
            nextScene = sceneName;
            this.isStopBGM = isStopBGM;
            anim.SetBool("WipeIn", false);
            anim.SetBool("WipeOut", true);
        }
    }

    public void WipeOut()
    {
        anim.SetBool("WipeIn", false);
        anim.SetBool("WipeOut", true);
    }
    public void WipeIn()
    {
        anim.SetBool("WipeIn", true);
        anim.SetBool("WipeOut", false);
    }
}
