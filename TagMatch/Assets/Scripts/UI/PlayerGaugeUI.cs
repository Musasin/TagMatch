using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGaugeUI : MonoBehaviour
{
    Player player;
    StaticValues.SwitchState switchState;
    Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player").GetComponent<Player>();
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        anim.SetInteger("switchState", (int)StaticValues.switchState);
    }
}
