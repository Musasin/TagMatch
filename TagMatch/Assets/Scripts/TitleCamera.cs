using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleCamera : MonoBehaviour
{
    const float CAMERA_MOVE_SPEED = 10.0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, new Vector2(transform.position.x + 100, transform.position.y), CAMERA_MOVE_SPEED * Time.deltaTime);
    }
}
