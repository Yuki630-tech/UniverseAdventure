using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateObj : MonoBehaviour
{
    [Tooltip("‰ñ“]‘¬“x"), SerializeField] Vector3 rotateSpeed;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(rotateSpeed.x * Time.deltaTime, rotateSpeed.y * Time.deltaTime, rotateSpeed.z * Time.deltaTime);
    }
}
