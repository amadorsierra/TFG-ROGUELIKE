using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerStats))]
[RequireComponent(typeof(Animator))]
public class PlayerCombat : MonoBehaviour
{
    [Header("Input System de ataque")]
    public InputActionReference attack;

    [Header("Equipamiento")]
    public WeaponDataSO currentWeapon;

    private PlayerStats stats;
    private Animator animator;
    private float nextAttackTime = 0f;

    void OnEnable()
    {
        if (attack != null && attack.action != null)
        {
            attack.action.started += OnAttackPressed;
            attack.action.Enable();
        }
    }

    void OnDisable()
    {
        if (attack != null && attack.action != null)
        {
            attack.action.started -= OnAttackPressed;
            attack.action.Disable();
        }
    }

    void Start()
    {
        stats = GetComponent<PlayerStats>();
        animator = GetComponent<Animator>();

        if (currentWeapon != null)
        {
            // Si tenemos equipada un arma el daño de esta se suma a nuestro daño base
            stats.attackDamage.AddModifier(currentWeapon.damage);
        }
    }

    // Este método se ejecuta automáticamente cuando el Input System detecta el "press" de ataque
    private void OnAttackPressed(InputAction.CallbackContext context)
    {
        float cooldown = currentWeapon != null ? currentWeapon.attackCooldown : stats.baseStats.attackCooldown;

        // Comprobamos el cooldown antes de permitir la acción
        if (Time.time >= nextAttackTime)
        {
            //ExecuteAttack();
            nextAttackTime = Time.time + cooldown;
        }
    }

    // private void ExecuteAttack()
    // {
    //     // Disparamos la animación de ataque en tu marioneta
    //     animator.SetTrigger("Attack");
    //     Debug.Log("¡Ataque ejecutado vía Input System!");
    // }

    // Mantenemos tu sistema de Trigger físico para la espada (o la lógica que uses)
    private void OnTriggerEnter2D(Collider2D collision)
    {
        EnemyHealth enemy = collision.GetComponent<EnemyHealth>();

        if (enemy != null && currentWeapon != null)
        {
            float totalDamage = stats.attackDamage.GetValue();
            enemy.TakeDamage(totalDamage);
            
            Debug.Log($"Impacto con {currentWeapon.name}. Daño infligido: {totalDamage}");
        }
    }
}