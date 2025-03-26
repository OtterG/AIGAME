using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateCube : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // 每一帧让物体围绕Y轴旋转
        transform.Rotate(0, 30 * Time.deltaTime, 0);
    }
}
