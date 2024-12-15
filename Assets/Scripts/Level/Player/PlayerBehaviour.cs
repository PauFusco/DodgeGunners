using System;
using System.Text;
using TMPro;
using UnityEngine;

public class PlayerBehaviour : MonoBehaviour
{
    public float m_speed = 1.0f;

    [SerializeField]
    private TextMeshProUGUI playerBillboard;

    public void SetPlayerTag(string username)
    { playerBillboard.text = username; }

    public void MoveLeft()
    { transform.position += m_speed * Time.deltaTime * Vector3.back; }

    public void MoveRight()
    { transform.position += m_speed * Time.deltaTime * Vector3.forward; }

    public void MoveUp()
    { transform.position += m_speed * Time.deltaTime * Vector3.up; }

    public void MoveDown()
    { transform.position += m_speed * Time.deltaTime * Vector3.down; }

    public void SetPosition(Vector3 newPos)
    { transform.position = newPos; }

    public void SetRotation(Quaternion newRot)
    { transform.rotation = newRot; }

    public Transform GetLocalTransform()
    { return transform; }
}