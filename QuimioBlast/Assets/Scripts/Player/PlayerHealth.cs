using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Header("Vida")]
    public float maxHealth = 100f;
    private float currentHealth;

    public float CurrentHealth => currentHealth;

    private void Start()
    {
        currentHealth = maxHealth;
    }

    /// <summary>
    /// Aplica cura ao jogador. Não ultrapassa o máximo de vida.
    /// </summary>
    public void Heal(float amount)
    {
        if (IsFullHealth())
        {
            Debug.Log("Vida já está cheia! Item não foi consumido.");
            return;
        }

        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
        Debug.Log($"[PlayerHealth] Curado em {amount}. Vida atual: {currentHealth}/{maxHealth}");
    }

    /// <summary>
    /// Aplica dano ao jogador. Se chegar a 0, chama Die().
    /// </summary>
    public void TakeDamage(float amount)
    {
        if (amount <= 0f) return;

        currentHealth -= amount;
        Debug.Log($"[PlayerHealth] Tomou {amount} de dano. Vida atual: {currentHealth}/{maxHealth}");

        if (currentHealth <= 0f)
        {
            currentHealth = 0f;
            Die();
        }
    }

    /// <summary>
    /// Retorna true se o jogador já está com vida no máximo.
    /// </summary>
    public bool IsFullHealth()
    {
        return currentHealth >= maxHealth;
    }

    /// <summary>
    /// Lógica de morte do jogador.
    /// TODO: substituir pelo carregamento da tela de morte quando ela for implementada.
    /// </summary>
    private void Die()
    {
        Debug.Log("[PlayerHealth] Jogador morreu!");
        // TODO: carregar tela de morte
        // SceneManager.LoadScene("GameOver");
    }
}
