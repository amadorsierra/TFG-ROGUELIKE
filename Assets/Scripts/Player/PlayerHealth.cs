using System;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{

    PlayerStats stats;
    private float currentHealth;

    // Patrón Observer
    // Avisa de que hemos cambiado la vida (currentHealth, maxHealth)
    public event Action<float, float> OnHealthChanged;
    // Avisa de que hemos muerto
    public event Action OnDeath;


    // Inicializamos la vida 
    void Start()
    {
        stats = GetComponent<PlayerStats>();
        currentHealth = stats.baseStats.maxHealth;

        // Invocamos el evento porque ha cambiado la vida
        OnHealthChanged?.Invoke(currentHealth, stats.baseStats.maxHealth);
    }

    public void TakeDamage(float damage)
    {
        if (currentHealth < 0 || damage <= 0) return;

        currentHealth -= damage;
        // Invocamos el evento porque ha cambiado la vida
        OnHealthChanged?.Invoke(currentHealth, stats.baseStats.maxHealth);

        if(currentHealth <= 0)
        {
            Die();
        }
    }

    public void Heal(float amount)
    {
        if (currentHealth <= 0 || amount <= 0) return;

        float currentMaxHealth = stats.maxHealth.GetValue();

        if(currentHealth + amount > currentMaxHealth) currentHealth = currentMaxHealth;
        else currentHealth += amount;
        
        // Invocamos el evento porque ha cambiado la vida
        OnHealthChanged?.Invoke(currentHealth, stats.baseStats.maxHealth);
    }


    private void Die()
    {
        // Disparamos el evento porque hemos muerto
        OnDeath?.Invoke();
        // Apagamos los componentes de movimiento y combate para que sea un cadáver inerte
        GetComponent<PlayerMovement>().enabled = false;
        
        PlayerCombat combatComponent = GetComponent<PlayerCombat>();
        if (combatComponent != null) combatComponent.enabled = false;
    }


}
