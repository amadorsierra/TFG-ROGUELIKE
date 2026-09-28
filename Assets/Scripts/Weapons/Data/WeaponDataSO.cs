using UnityEngine;

[CreateAssetMenu(fileName = "WeaponData_", menuName = "Stats/Weapon Data")]
public class WeaponDataSO : ScriptableObject
{
    [field: SerializeField]
    public GameObject weaponPrefab { get; private set; }
    public float damage { get; private set; }
    [field: SerializeField]
    public float attackCooldown { get; private set; }
    [field: SerializeField]
    public float projectileSpeed { get; private set; }
    [field: SerializeField]
    public float range { get; private set; }
    [field: SerializeField]
    public int pierce { get; private set; }
}
