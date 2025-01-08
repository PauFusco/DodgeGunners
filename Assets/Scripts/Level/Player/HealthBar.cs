using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Image healthBar;
    
    private float _health;
    public float maxHealth;
    
    void Start()
    {
        ResetHealth();
    }

    public void TakeDamage()
    {
        if (_health > 0)
            _health--;

        healthBar.fillAmount = _health / maxHealth;
    }

    public void ResetHealth()
    {
        _health = maxHealth;
        healthBar.fillAmount = _health / maxHealth;
    }

    public float GetHealth()
    { return _health; }

    public void SetHealth(float health)
    {
        _health = health;
        healthBar.fillAmount = _health / maxHealth;
    }
}
