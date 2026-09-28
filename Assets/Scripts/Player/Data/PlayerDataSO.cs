using UnityEngine;

[CreateAssetMenu(fileName = "PlayerData_", menuName = "Stats/Player")]
public class PlayerDataSO : ScriptableObject
{
    [field: SerializeField]
    public float maxHealth { get; private set; }
    [field: SerializeField]
    public float moveSpeed { get; private set; }
    [field: SerializeField]
    public float attackDamage { get; private set; }
    [field: SerializeField]
    public float attackCooldown { get; private set; }

}
