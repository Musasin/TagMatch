using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateWithDirection : MonoBehaviour
{
    Rigidbody2D rb;
    public float correctionDeg = 0; /* バレットのデフォルト向きの補正値 0で右向き */
    bool isActive = false;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isActive)
        {
            Vector2 v = rb.velocity;
            float atan = Mathf.Atan2(v.y, v.x);
            transform.rotation = Quaternion.Euler(0, 0, atan * Mathf.Rad2Deg + correctionDeg);
        }
    }

    public void SetIsActive(bool flag)
    {
        isActive = flag;
    }
}