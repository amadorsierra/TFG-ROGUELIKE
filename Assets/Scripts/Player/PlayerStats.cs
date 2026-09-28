using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [Header("Configuración Base")]
    [SerializeField]
    public PlayerDataSO baseStats;

    public Stat maxHealth = new Stat();
    public Stat moveSpeed = new Stat();
    public Stat attackDamage = new Stat();

    void Awake()
    {
        // 1. Inicializamos el estado leyendo de la "base de datos"
        maxHealth.Initialize(baseStats.maxHealth);
        moveSpeed.Initialize(baseStats.moveSpeed);
        attackDamage.Initialize(baseStats.attackDamage);
    }
}