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
    { transform.position += m_speed * Time.deltaTime * Vector3.left; }

    public void MoveRight()
    { transform.position += m_speed * Time.deltaTime * Vector3.right; }

    public void MoveUp()
    { transform.position += m_speed * Time.deltaTime * Vector3.up; }

    public void MoveDown()
    { transform.position += m_speed * Time.deltaTime * Vector3.down; }

    public void SetPosition(Vector3 newPos)
    { transform.position = newPos; }
}