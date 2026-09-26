using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public float moveSpeed = 3f;
    private Transform playerTransform;
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        
        // Buscamos al jugador automáticamente en la escena por su etiqueta o componente
        PlayerMovement player = FindAnyObjectByType<PlayerMovement>();
        if (player != null)
        {
            playerTransform = player.transform;
        }
    }

    void FixedUpdate()
    {
        if (playerTransform != null)
        {
            // Calculamos la dirección hacia el jugador
            Vector2 direction = (playerTransform.position - transform.position).normalized;
            
            // Movemos al enemigo usando físicas
            rb.linearVelocity = direction * moveSpeed;
        }
    }
}