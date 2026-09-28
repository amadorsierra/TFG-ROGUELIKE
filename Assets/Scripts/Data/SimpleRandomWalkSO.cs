using UnityEngine;

[CreateAssetMenu(fileName = "SimpleRandomWalkParameters_", menuName = "PCG/SimpleRandomWalkData")]
public class SimpleRandomWalkSO : ScriptableObject
{
    [field: SerializeField]
    public int iterations { get; private set; } = 10;
    [field: SerializeField] 
    public int walkLength { get; private set; } = 10;
    [field: SerializeField]
    public bool startRandomlyEachIteration { get; private set; } = true;

}
