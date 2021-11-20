using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Linq;

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

    public virtual bool IsLifeHalf()
    {
        return (StaticValues.bossHP.Sum() <= (StaticValues.bossMaxHP.Sum() / 2));
    }
    public virtual bool IsLifeTwoThirds()
    {
        return (StaticValues.bossHP.Sum() <= (StaticValues.bossMaxHP.Sum() * 2 / 3));
    }
    public virtual bool IsLifeOneThirds()
    {
        return (StaticValues.bossHP.Sum() <= (StaticValues.bossMaxHP.Sum() / 3));
    }
}
