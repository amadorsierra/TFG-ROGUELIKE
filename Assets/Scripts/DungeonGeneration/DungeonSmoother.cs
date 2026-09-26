using System.Collections.Generic;
using UnityEngine;

public static class DungeonSmoother
{
    // El parámetro 'iterations' define cuántas pasadas de suavizado se hacen. 
    // 1 o 2 suele ser perfecto. Más de 3 puede deformar demasiado la mazmorra.
    public static HashSet<Vector2Int> Smooth(HashSet<Vector2Int> floorPositions, int iterations = 2)
    {
        if (floorPositions == null || floorPositions.Count == 0) return floorPositions;

        HashSet<Vector2Int> smoothedFloor = new HashSet<Vector2Int>(floorPositions);
        Vector2Int[] allDirections = {
            Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right,
            new Vector2Int(1, 1), new Vector2Int(1, -1), new Vector2Int(-1, 1), new Vector2Int(-1, -1)
        };

        // 1. Obtenemos los límites (Bounding Box)
        int minX = int.MaxValue, maxX = int.MinValue;
        int minY = int.MaxValue, maxY = int.MinValue;
        foreach (var pos in floorPositions)
        {
            if (pos.x < minX) minX = pos.x; if (pos.x > maxX) maxX = pos.x;
            if (pos.y < minY) minY = pos.y; if (pos.y > maxY) maxY = pos.y;
        }

        // 2. Aplicamos el suavizado en bucle según las iteraciones
        for (int i = 0; i < iterations; i++)
        {
            HashSet<Vector2Int> nextFloor = new HashSet<Vector2Int>();

            for (int x = minX - 1; x <= maxX + 1; x++)
            {
                for (int y = minY - 1; y <= maxY + 1; y++)
                {
                    Vector2Int pos = new Vector2Int(x, y);
                    int floorNeighbors = 0;

                    // Contamos cuántos vecinos de las 8 direcciones son suelo
                    foreach (var dir in allDirections)
                    {
                        if (smoothedFloor.Contains(pos + dir)) floorNeighbors++;
                    }

                    bool isFloor = smoothedFloor.Contains(pos);

                    // REGLAS DEL AUTÓMATA CELULAR:
                    // Si es suelo y tiene 4 o más vecinos que también son suelo, se mantiene.
                    // Si es pared/hueco, pero está rodeado de 5 o más suelos, se convierte en suelo (tapa los huecos de las uniones).
                    if ((isFloor && floorNeighbors >= 4) || (!isFloor && floorNeighbors >= 5))
                    {
                        nextFloor.Add(pos);
                    }
                }
            }
            smoothedFloor = nextFloor;
        }

        return smoothedFloor;
    }
}