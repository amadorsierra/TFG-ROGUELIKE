using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponController : MonoBehaviour
{

    [Header("Weapon Stats")]
    public GameObject weaponPrefab;
    public float damage;
    public float fireRate = 0.5f;
    public float projectileSpeed;
    public float range = 5f;
    public int pierce;
    // Variable interna para controlar cuándo se puede volver a disparar
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
                nextFireTime = Time.time + fireRate;
                Attack();
            }
        }
    }

    protected virtual void Attack()
    {
    }
}
