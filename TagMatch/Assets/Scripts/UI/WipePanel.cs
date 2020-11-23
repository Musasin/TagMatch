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
                StaticValues.isPause = false;
                SceneManager.LoadScene(nextScene);
            }
        }
    }

    public void ChangeScene(string sceneName)
    {
        if (!isWipeOutPlaying)
        {
            StaticValues.isPause = true;
            isWipeOutPlaying = true;
            wipeTime = 0;
            nextScene = sceneName;
            anim.SetBool("WipeIn", false);
            anim.SetBool("WipeOut", true);
        }
    }
}
