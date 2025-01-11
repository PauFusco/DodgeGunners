using System;
using TMPro;
using UnityEngine;

public class PlayerBehaviour : MonoBehaviour
{
    private Rigidbody rb;
    public float speed = 2.0f;

    public float jumpForce = 5.0f;
    private int m_jumpCount = 0;
    public int maxJumps = 2;

    public bool isAlive = true;

    public HealthBar healthBarScript;

    public TextMeshProUGUI usernameText;
    public Score scoreScript;

    public bool canMove = true;

    [SerializeField]
    private int baseAmmo = 1;

    private int ammo;

    private void Start()
    {
        isAlive = true;
        if (!TryGetComponent<Rigidbody>(out rb))
            Debug.LogError("Rigidbody is missing on the player GameObject");

        ammo = baseAmmo;
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
        if (m_jumpCount > 0 && canMove)
            rb.AddForce(3 * jumpForce * Vector3.down, ForceMode.Impulse);
    }

    public void SetPosition(Vector3 newPos)
    { transform.position = newPos; }

    public void SetRotation(Quaternion newRot)
    { transform.rotation = newRot; }

    public Transform GetLocalTransform()
    { return transform; }

    public void ResetValues()
    {
        healthBarScript.SetHealth(3);
        ammo = baseAmmo;
    }

    public float GetHealth()
    { return healthBarScript.GetHealth(); }

    public int GetScore()
    { return scoreScript.GetScore(); }

    public int GetAmmo()
    { return ammo; }

    public void ReduceAmmo()
    { ammo--; }

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