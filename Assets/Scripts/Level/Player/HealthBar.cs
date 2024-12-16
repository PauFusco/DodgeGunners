using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Image healthBar;
    
    public float health;
    public float maxHealth;
    
    void Start()
    {
        ResetHealth();
    }

    public void TakeDamage()
    {
        if (health > 0)
            health --;

        healthBar.fillAmount = health / maxHealth;
    }

    public void ResetHealth()
    {
        health = maxHealth;
        healthBar.fillAmount = health / maxHealth;
    }
}
