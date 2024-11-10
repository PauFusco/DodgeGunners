using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameManager;
//using GameManager;

public class PlayerManager : MonoBehaviour
{
    public float m_speed = 1.0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Remote player update pos
        UpdateRemote(/*which user is?*/);

        // Update player
        // Depending on key, call function
    }

    void UpdateRemote() 
    {
        // Recieve information
        // Call functions
    }

    void MoveLeft(GameObject player)
    {
        player.transform.position += Vector3.left * m_speed * Time.deltaTime;
    }

    void MoveRight(GameObject player)
    {
        player.transform.position += Vector3.left * m_speed * Time.deltaTime;
    }

    void MoveForward(GameObject player)
    {
        player.transform.position += Vector3.left * m_speed * Time.deltaTime;
    }

    void MoveBack(GameObject player)
    {
        player.transform.position += Vector3.left * m_speed * Time.deltaTime;
    }
}
