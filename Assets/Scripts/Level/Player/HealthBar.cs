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
        health = maxHealth;
    }

    public void UpdateHealth(int damage)
    {
        if (health > 0)
            health -= damage;

        healthBar.fillAmount = health / maxHealth;
    }
}
