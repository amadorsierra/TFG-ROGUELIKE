using UnityEngine;

public class BoneController : WeaponController
{   
    protected override void Attack()
    {
        Vector2 shootDirection = playerMovement.fireDirection;
        
        // Instanciamos el hueso
        GameObject spawnedBone = Instantiate(weaponData.weaponPrefab, transform.position, Quaternion.identity);

        // Inyectamos todos los datos necesarios (Dirección y Velocidad)
        spawnedBone.GetComponent<ProjectileWeaponBehaviour>().Setup(
            shootDirection, 
            weaponData.projectileSpeed, 
            weaponData.range,
            weaponData.damage);
    }
}
