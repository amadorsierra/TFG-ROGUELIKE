using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData_", menuName = "Stats/Enemy Data")]
public class EnemyDataSO : ScriptableObject
{
    [field: SerializeField]
    public GameObject enemyPrefab { get; private set; }
    [field: SerializeField]
    public string enemyName { get; private set; }
    [field: SerializeField]
    public float maxHealth { get; private set; }
    [field: SerializeField]
    public float attackDamage { get; private set; }
    [field: SerializeField]
    public float moveSpeed { get; private set; }
    [field: SerializeField]
    public float aggroRange { get; private set; }
    [field: SerializeField]
    public float attackRange { get; private set; }
    [field: SerializeField]
    public float attackCooldown { get; private set; }
}
