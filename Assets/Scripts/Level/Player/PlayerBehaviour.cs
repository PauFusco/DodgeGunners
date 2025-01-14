using System;
using TMPro;
using UnityEngine;

public class PlayerBehaviour : MonoBehaviour
{
    [SerializeField]
    private int baseAmmo = 1;

    private Rigidbody rb;
    public Animator animator;

    public float speed = 2.0f;
    public float jumpForce = 5.0f;
    private int m_jumpCount = 0;
    public int maxJumps = 2;

    private int ammo;

    public TextMeshProUGUI usernameText;
    public HealthBar healthBarScript;
    public Score scoreScript;

    public bool isAlive = true;
    public bool canMove = true;
    private bool isGrounded = true;

    private void Start()
    {
        isAlive = true;
        if (!TryGetComponent<Rigidbody>(out rb))
            Debug.LogError("Rigidbody is missing on the player GameObject");

        ammo = baseAmmo;
        animator.Play("fall");
    }

    public void SetPlayerTag(string username)
    { usernameText.text = username; }

    public void MoveLeft()
    { 
        transform.position += speed * Time.deltaTime * Vector3.back;
        animator.Play("run");
    }

    public void MoveRight()
    { 
        transform.position += speed * Time.deltaTime * Vector3.forward;
        animator.Play("run");
    }

    public void MoveUp()
    {
        animator.Play("doubleJump");

        if (m_jumpCount < maxJumps)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            m_jumpCount++;
            animator.Play("jump");
        }
    }

    public void MoveDown()
    {
        if (m_jumpCount > 0 && canMove)
        {
            rb.AddForce(3 * jumpForce * Vector3.down, ForceMode.Impulse);
            animator.Play("fall");
        }

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
        animator.Play("fall");
    }

    public float GetHealth()
    { return healthBarScript.GetHealth(); }

    public int GetScore()
    { return scoreScript.GetScore(); }

    public int GetAmmo()
    { return ammo; }

    public void ReduceAmmo()
    { 
        ammo--;
        animator.Play("hit");
    }

    public void Die()
    { isAlive = false; }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            m_jumpCount = 0;
            animator.Play("idle");
        }
    }
}