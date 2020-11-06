using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AxisDownChecker : MonoBehaviour
{
    static float beforeAxisVertical, beforeAxisHorizontal;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public static void AxisDownUpdate()
    {
        beforeAxisHorizontal = Input.GetAxisRaw("Horizontal");
        beforeAxisVertical = Input.GetAxisRaw("Vertical");
    }
    
    public static bool GetAxisDownHorizontal()
    {
        return beforeAxisHorizontal == 0 && (Input.GetAxisRaw("Horizontal") > 0 || Input.GetAxisRaw("Horizontal") < 0);
    }
    public static bool GetAxisDownVertical()
    {
        return beforeAxisVertical == 0 && (Input.GetAxisRaw("Vertical") > 0 || Input.GetAxisRaw("Vertical") < 0);
    }
}
