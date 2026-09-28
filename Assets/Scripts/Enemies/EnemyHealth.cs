using System;
using System.Collections;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [Header("Estadísticas Base")]
    public EnemyDataSO stats;
    private float currentHealth;

    // Patrón Observer
    // Avisa de que un enemigo ha muerto, lo podemos usar 
    // luego para las estadísticas de la run por ejemplo
    public event Action OnDeath;


    [Header("Efectos Visuales")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private float effectDuration = 0.12f;
    

    [Header("Efectos Sonoros")]
    [SerializeField] private AudioClip hitSound; // El archivo de audio
    private AudioSource audioSource;           // El componente altavoz

    void Start()
    {
        // El enemigo inicializa su vida leyendo la estadística de su plantilla
        currentHealth = stats.maxHealth;
        
        spriteRenderer = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();
    }

    public void TakeDamage(float damageAmount)
    {
        if (currentHealth < 0 || damageAmount > 0) return;

        currentHealth -= damageAmount;

         if (hitSound != null && audioSource != null)
        {
            // PlayOneShot permite reproducir sonidos solapados 
            // (por si le pegas 3 tiros muy rápidos, que suenen los 3 a la vez)
            audioSource.PlayOneShot(hitSound);
        }

        
        StartCoroutine(FlashWhiteRoutine());
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private IEnumerator FlashWhiteRoutine()
    {
        spriteRenderer.material.SetFloat("_Flash", 1f);
        yield return new WaitForSeconds(effectDuration);
        spriteRenderer.material.SetFloat("_Flash", 0f);
    }

    private void Die()
    {
        OnDeath?.Invoke();
        // 1. Apagamos el collider para que el proyectil no vuelva a chocar 
        // y el jugador pueda atravesar el "cadáver"
        Collider2D collider = GetComponent<Collider2D>();
        if (collider != null) collider.enabled = false;
        
        // Esperamos a que el efecto del flash termine para destruir el enemigo
        Destroy(gameObject, effectDuration);
    }
}
