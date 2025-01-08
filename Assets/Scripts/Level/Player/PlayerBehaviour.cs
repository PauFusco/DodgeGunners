using System;
using System.Text;
using TMPro;
using UnityEngine;

public class PlayerBehaviour : MonoBehaviour
{
    private Rigidbody rb;
    public float speed = 2.0f;

    public float jumpForce = 5.0f;
    private int m_jumpCount = 0;
    public int maxJumps = 2;

    private readonly UInt16 _score;
    public bool isAlive = true;

    public HealthBar healthBar;

    public TextMeshProUGUI usernameText;
    public Score score;

    private void Start()
    {
        isAlive = true;
        rb = GetComponent<Rigidbody>();
        if (rb == null)
            Debug.LogError("Rigidbody is missing on the player GameObject");
    }

    public void SetPlayerTag(string username)
    { usernameText.text = username; }

    public void MoveLeft()
    { transform.position += speed * Time.deltaTime * Vector3.back; }

    public void MoveRight()
    { transform.position += speed * Time.deltaTime * Vector3.forward; }

    public void MoveUp()
    {
        if (m_jumpCount < maxJumps)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            m_jumpCount++;
        }
    }

    public void MoveDown()
    {
        if (m_jumpCount > 0)
            rb.AddForce(Vector3.down * jumpForce * 3, ForceMode.Impulse);
    }

    public void SetPosition(Vector3 newPos)
    { transform.position = newPos; }

    public void SetRotation(Quaternion newRot)
    { transform.rotation = newRot; }

    public Transform GetLocalTransform()
    { return transform; }

    public float GetHealth()
    { return healthBar.GetHealth(); }

    public int GetScore()
    { return score.GetScore(); }

    public void Die()
    { isAlive = false; }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            m_jumpCount = 0;
        }
    }
}