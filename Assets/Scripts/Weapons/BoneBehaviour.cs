using UnityEngine;

public class BoneBehaviour : ProjectileWeaponBehaviour
{
    void Update()
    {
        transform.position += speed * Time.deltaTime * direction;

        if (Vector3.Distance(startPosition, transform.position) >= range)
        {
            Destroy(gameObject);
        }
    }
}
