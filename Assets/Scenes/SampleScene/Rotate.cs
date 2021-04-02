using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour
{
    public float speed = 10;
    // Update is called once per frame
    
    void FixedUpdate()
    {
        Vector3 rotate = transform.eulerAngles;
        transform.rotation = Quaternion.Euler(rotate.x,rotate.y+speed,rotate.z);
    }
}
