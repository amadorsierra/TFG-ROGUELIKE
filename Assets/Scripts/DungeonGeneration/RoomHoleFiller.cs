using System.Collections.Generic;
using UnityEngine;

public static class RoomHoleFiller
{
    public static HashSet<Vector2Int> FillAllHoles(HashSet<Vector2Int> floorPositions)
    {
        if (floorPositions == null || floorPositions.Count == 0) 
            return floorPositions;

        // 1. Calcular el rectángulo delimitador (Bounding Box) de toda la mazmorra
        int minX = int.MaxValue, maxX = int.MinValue;
        int minY = int.MaxValue, maxY = int.MinValue;

        foreach (var pos in floorPositions)
        {
            if (pos.x < minX) minX = pos.x;
            if (pos.x > maxX) maxX = pos.x;
            if (pos.y < minY) minY = pos.y;
            if (pos.y > maxY) maxY = pos.y;
        }

        // Añadimos 1 celda de margen para asegurar que el "agua" pueda circular por fuera
        minX -= 1; maxX += 1;
        minY -= 1; maxY += 1;

        // 2. Inundación (Flood Fill) desde el espacio exterior
        HashSet<Vector2Int> outsideWater = new HashSet<Vector2Int>();
        Queue<Vector2Int> queue = new Queue<Vector2Int>();
        
        // Empezamos a inundar desde la esquina inferior izquierda (que sabemos que está fuera)
        Vector2Int start = new Vector2Int(minX, minY);
        queue.Enqueue(start);
        outsideWater.Add(start);

        Vector2Int[] cardinalDirections = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };

        while (queue.Count > 0)
        {
            Vector2Int current = queue.Dequeue();

            foreach (var dir in cardinalDirections)
            {
                Vector2Int neighbor = current + dir;

                // Si nos salimos del Bounding Box, no seguimos por ahí
                if (neighbor.x < minX || neighbor.x > maxX || neighbor.y < minY || neighbor.y > maxY)
                    continue;

                // Si la celda vecina NO es suelo de la mazmorra y NO tiene agua todavía, el agua avanza
                if (!floorPositions.Contains(neighbor) && !outsideWater.Contains(neighbor))
                {
                    outsideWater.Add(neighbor);
                    queue.Enqueue(neighbor);
                }
            }
        }

        // 3. Rellenar huecos (Cualquier tamaño)
        HashSet<Vector2Int> filledFloor = new HashSet<Vector2Int>(floorPositions);

        // Recorremos todo el rectángulo delimitador
        for (int x = minX; x <= maxX; x++)
        {
            for (int y = minY; y <= maxY; y++)
            {
                Vector2Int pos = new Vector2Int(x, y);
                
                // Si esta posición NO es suelo original y NO fue alcanzada por el agua exterior...
                // Significa que es un hueco hermético. Lo convertimos en suelo.
                if (!floorPositions.Contains(pos) && !outsideWater.Contains(pos))
                {
                    filledFloor.Add(pos);
                }
            }
        }

        return filledFloor;
    }
}