using UnityEngine;

public class EnemyStats : MonoBehaviour
{
    [Header("Configuración Base")]
    [SerializeField]
    EnemyDataSO stats;

    public Stat maxHealth = new Stat();
    public Stat attackDamage = new Stat();
    public Stat moveSpeed = new Stat();

    void Awake()
    {
        maxHealth.Initialize(stats.maxHealth);
        attackDamage.Initialize(stats.attackDamage);
        moveSpeed.Initialize(stats.moveSpeed);
    }
}
