using UnityEngine;

public class ProjectileWeaponBehaviour : MonoBehaviour
{

    protected Vector3 direction;
    protected float speed;
    protected float range;
    protected Vector3 startPosition; // Para saber de donde se lanzó el proyectil

    // Configura el arma
    public virtual void Setup(Vector3 newDirection, float newSpeed, float newRange)
    {
        direction = newDirection;
        speed = newSpeed;
        range = newRange;
        startPosition = transform.position;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    // Este método de Unity se ejecuta automáticamente cuando choca con otro Trigger
    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        // Comprobamos si el objeto con el que chocó tiene la etiqueta "Wall"
        if (collision.CompareTag("Wall"))
        {
            Destroy(gameObject);
        }

        if (collision.CompareTag("Enemy"))
        {
            // Opcional: Aquí más adelante le restaremos vida al enemigo
            
            // Destruimos el hueso al impactar
            Destroy(gameObject);
            
            // Destruimos al enemigo temporalmente para probar
            Destroy(collision.gameObject);
        }
    }

}
