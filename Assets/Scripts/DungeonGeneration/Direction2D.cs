using System.Collections.Generic;
using UnityEngine;

public static class Direction2D
{
    // Lista estática con las 4 direcciones cardinales (Arriba, Derecha, Abajo, Izquierda)
    public static List<Vector2Int> cardinalDirectionsList = new List<Vector2Int>
    {
        new Vector2Int(0, 1),  // UP
        new Vector2Int(1, 0),  // RIGHT
        new Vector2Int(0, -1), // DOWN
        new Vector2Int(-1, 0)  // LEFT
    };

    public static List<Vector2Int> eightDirectionsList = new List<Vector2Int>
    {
        new Vector2Int(0, 1),   // UP
        new Vector2Int(1, 1),   // UP-RIGHT
        new Vector2Int(1, 0),   // RIGHT
        new Vector2Int(1, -1),  // DOWN-RIGHT
        new Vector2Int(0, -1),  // DOWN
        new Vector2Int(-1, -1), // DOWN-LEFT
        new Vector2Int(-1, 0),  // LEFT
        new Vector2Int(-1, 1)   // UP-LEFT
    };
    
    // Retorna una dirección aleatoria
    public static Vector2Int GetRandomCardinalDirection()
    {
        return cardinalDirectionsList[Random.Range(0, cardinalDirectionsList.Count)];
    }
}
