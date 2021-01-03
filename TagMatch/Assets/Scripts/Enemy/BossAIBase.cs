using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BossAIBase : MonoBehaviour
{
    protected Animator anim;
    protected Boss bossScript;

    protected int stateIndex;
    protected bool isPlaying = false;
    protected bool isRight = false;
    protected bool isDead;

    protected Sequence sequence;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public virtual void Reset()
    {
        sequence.Kill();
        isPlaying = false;
        isRight = false;
        isDead = false;
    }
}
