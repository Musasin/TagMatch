using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ending : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        AudioManager.Instance.LoadACB("Ending", "Ending.acb", "Ending.awb");
        AudioManager.Instance.PlayBGM("ending");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
