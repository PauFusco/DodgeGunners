using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float m_speed = 1.0f;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.A))
            transform.position += Vector3.left * m_speed * Time.deltaTime;
        if (Input.GetKey(KeyCode.D))
            transform.position += Vector3.right * m_speed * Time.deltaTime;
        if (Input.GetKey(KeyCode.W))
            transform.position += Vector3.forward * m_speed * Time.deltaTime;
        if (Input.GetKey(KeyCode.S))
            transform.position += Vector3.back * m_speed * Time.deltaTime;

    }
}
