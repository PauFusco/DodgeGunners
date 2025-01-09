using UnityEngine;

public class ProjectileBehaviour : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            var player = other.GetComponent<PlayerBehaviour>();
            if (player != null)
            {
                player.TakeDamage();
                if (player.healthBar.GetHealth() == 0)
                {
                    player.Die();
                }
            }

            Destroy(gameObject);
        }
    }
}