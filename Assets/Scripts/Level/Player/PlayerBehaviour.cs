using System;
using TMPro;
using UnityEngine;

public class PlayerBehaviour : MonoBehaviour
{
    public enum AnimationState
    {
        IDLE,
        RUN,
        DOUBLEJUMP,
        JUMP,
        FALL,
        HIT,
        DEFAULT
    }

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

    private AnimationState currentAnimation = AnimationState.IDLE;

    private void Start()
    {
        isAlive = true;
        if (!TryGetComponent<Rigidbody>(out rb))
            Debug.LogError("Rigidbody is missing on the player GameObject");

        ammo = baseAmmo;
        PlayAnimation(currentAnimation);
    }

    public void SetPlayerTag(string username)
    { usernameText.text = username; }

    public void MoveLeft()
    { 
        transform.position += speed * Time.deltaTime * Vector3.back;
        PlayAnimation(AnimationState.RUN);
    }

    public void MoveRight()
    { 
        transform.position += speed * Time.deltaTime * Vector3.forward;
        PlayAnimation(AnimationState.RUN);
    }

    public void MoveUp()
    {
        if (m_jumpCount < maxJumps)
        {
            m_jumpCount++;
            
            if (m_jumpCount == 1) PlayAnimation(AnimationState.JUMP);
            else if (m_jumpCount == 2) PlayAnimation(AnimationState.DOUBLEJUMP);

            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    public void MoveDown()
    {
        if (m_jumpCount > 0 && canMove)
        {
            rb.AddForce(3 * jumpForce * Vector3.down, ForceMode.Impulse);
            PlayAnimation(AnimationState.FALL);
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
        PlayAnimation(AnimationState.IDLE);
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
    }

    public bool isGrounded() 
    { return m_jumpCount == 0; }

    public void SetIdle() 
    { PlayAnimation(AnimationState.IDLE); }

    public void Die()
    { 
        isAlive = false;
        PlayAnimation(AnimationState.IDLE);
    }
    public void PlayAnimation(AnimationState state)
    {
        if (currentAnimation == state) return;

        currentAnimation = state;
        animator.Play(state.ToString().ToLower());
    }

    public int GetCurrentAnimation()
    {
        return (int)currentAnimation;
    }

    public void SetAnimation(int state)
    {
        PlayAnimation((AnimationState)state);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            m_jumpCount = 0;
            SetIdle();
        }
    }
}