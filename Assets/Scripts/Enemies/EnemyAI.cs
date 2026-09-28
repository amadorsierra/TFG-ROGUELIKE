using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [Header("Estadísticas base")]
    public EnemyDataSO stats;
    
    private Transform playerTransform;
    private Rigidbody2D rb;
    private EnemyAnimationController animController; // Referencia a nuestra marioneta
    
    private float nextAttackTime = 0f;

    void Start()
    {
        if (stats == null) return;

        rb = GetComponent<Rigidbody2D>();
        animController = GetComponent<EnemyAnimationController>(); 
        
        PlayerMovement player = FindAnyObjectByType<PlayerMovement>();
        if (player != null) playerTransform = player.transform;
    }

    void FixedUpdate()
    {
        if (playerTransform != null)
        {
            float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);
            
            // 1. ESTADO DE ATAQUE
            if (distanceToPlayer <= stats.attackRange)
            {
                rb.linearVelocity = Vector2.zero; // Detenemos el cuerpo físico
                
                // ZONA MUERTA: Solo recalculamos la mirada si no estás literalmente cruzando su centro
                if (distanceToPlayer > 0.15f) 
                {
                    Vector2 lookDirection = (playerTransform.position - transform.position).normalized;
                    animController.UpdateAnimations(0f, lookDirection); 
                }
                else 
                {
                    // Si estás demasiado cerca (superpuesto), pasamos Vector2.zero. 
                    // Nuestra marioneta ignorará esto y mantendrá la última mirada correcta.
                    animController.UpdateAnimations(0f, Vector2.zero);
                }

                if (Time.time >= nextAttackTime)
                {
                    animController.TriggerAttack();
                    nextAttackTime = Time.time + stats.attackCooldown;
                }
            }
            // 2. ESTADO DE PERSECUCIÓN
            else if (distanceToPlayer <= stats.aggroRange)
            {
                Vector2 moveDirection = (playerTransform.position - transform.position).normalized;
                rb.linearVelocity = moveDirection * stats.moveSpeed;
                
                // Le pasamos la velocidad real y la dirección de movimiento
                animController.UpdateAnimations(rb.linearVelocity.magnitude, moveDirection);
            }
            // 3. ESTADO DE REPOSO
            else
            {
                rb.linearVelocity = Vector2.zero;
                
                // Le decimos que la velocidad es 0, y pasamos Vector2.zero para que no cambie la última mirada
                animController.UpdateAnimations(0f, Vector2.zero);
            }
        }
    }
}