using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public float maxHealth = 100f;
    private float currentHealth;

    [Header("Feedback de dano na tela")]
    public ScreenDamageEffect screenDamage;

    private void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;

        Debug.Log("Player tomou dano! Vida: " + currentHealth);

        // 🔴 efeito visual na tela
        if (screenDamage != null)
        {
            screenDamage.ShowDamage();
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("Player morreu 💀");
        gameObject.SetActive(false);
    }
}