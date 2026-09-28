using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponController : MonoBehaviour
{

    [Header("Datos del Arma")]
    public WeaponDataSO weaponData;
    protected float nextFireTime = 0f;
    protected PlayerMovement playerMovement;

    protected virtual void Start()
    {
        // Buscamos el script de movimiento al iniciar
        playerMovement = FindAnyObjectByType<PlayerMovement>();
    }

    protected virtual void Update()
    {
        if (playerMovement.attack.action.IsPressed())
        {
            if (Time.time >= nextFireTime)
            {
                nextFireTime = Time.time + weaponData.attackCooldown;
                Attack();
            }
        }
    }
    
    protected virtual void Attack() { }
}
