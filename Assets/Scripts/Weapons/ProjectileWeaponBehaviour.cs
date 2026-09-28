using UnityEngine;

public class ProjectileWeaponBehaviour : MonoBehaviour
{

    protected Vector3 direction;
    protected float speed;
    protected float range;
    protected float damage;
    protected Vector3 startPosition; // Para saber de donde se lanzó el proyectil

    // Configura el arma
    public virtual void Setup(Vector3 direction, float speed, float range, float damage)
    {
        this.direction = direction;
        this.speed = speed;
        this.range = range;
        this.damage = damage;
        startPosition = transform.position;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    // Limitamos el rango de proyectiles
    protected virtual void Update()
    {
        transform.position += speed * Time.deltaTime * direction;

        if (Vector3.Distance(startPosition, transform.position) >= range)
        {
            Destroy(gameObject);
        }
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
            EnemyHealth enemy = collision.GetComponent<EnemyHealth>();
            if (enemy != null)
            {
                enemy.TakeDamage(Mathf.RoundToInt(damage));
            }

            // Destruimos el proyectil al impactar
            Destroy(gameObject);
        }
    }

}
