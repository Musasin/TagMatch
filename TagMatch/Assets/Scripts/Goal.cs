using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Goal : MonoBehaviour
{
    public string nextScene;
    WipePanel wipePanel;

    // Start is called before the first frame update
    void Start()
    {
        wipePanel = GameObject.Find("WipePanel").GetComponent<WipePanel>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            wipePanel.ChangeScene(nextScene);
        }
    }
}
