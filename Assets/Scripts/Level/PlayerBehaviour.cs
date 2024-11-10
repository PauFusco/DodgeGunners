using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehaviour : MonoBehaviour
{
    [SerializeField]
    private float m_speed = 1.0f;

    public void MoveLeft()
    {
        transform.position += m_speed * Time.deltaTime * Vector3.left;
    }

    public void MoveRight()
    {
        transform.position += m_speed * Time.deltaTime * Vector3.right;
    }

    public void MoveForward()
    {
        transform.position += m_speed * Time.deltaTime * Vector3.up;
    }

    public void MoveBack()
    {
        transform.position += m_speed * Time.deltaTime * Vector3.down;
    }

    public void SetPosition(Vector3 newPos)
    {
        transform.position = newPos;
    }
}