using UnityEngine;

[RequireComponent(typeof(Animator))]
public class EnemyAnimationController : MonoBehaviour
{
    private Animator am;

    void Awake()
    {
        am = GetComponent<Animator>();
    }

    // El cerebro llamará a este método para dictar exactamente qué debe hacer el cuerpo
    public void UpdateAnimations(float speed, Vector2 lookDirection)
    {
        am.SetFloat("Speed", speed);

        // Si hay una dirección válida, actualizamos hacia dónde mira, sin importar la velocidad
        if (lookDirection != Vector2.zero)
        {
            am.SetFloat("Horizontal", lookDirection.x);
            am.SetFloat("Vertical", lookDirection.y);
        }
    }

    public void TriggerAttack()
    {
        am.SetTrigger("Attack");
    }
}