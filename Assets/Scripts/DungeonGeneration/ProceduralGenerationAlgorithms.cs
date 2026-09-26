using System.Collections.Generic;
using UnityEngine;

public static class ProceduralGenerationAlgorithms
{
    // Usamos HashSet para evitar duplicados, random walk puede visitar el mismo sitio varias veces
    public static HashSet<Vector2Int> SimpleRandomWalk(Vector2Int startPosition, int walkLength)
    {
        HashSet<Vector2Int> path = new HashSet<Vector2Int>();

        path.Add(startPosition);

        var previousPosition = startPosition;

        // Nos movemos en una dirección aleatoria y guardamos la posición
        for (int i = 0; i < walkLength; i++)
        {
            var newPosition = previousPosition + Direction2D.GetRandomCardinalDirection();
            path.Add(newPosition);
            previousPosition = newPosition;
        }

        return path;
    }

    public static List<Vector2Int> RandomWalkCorridor(Vector2Int startPosition, int corridorLength)
    {
        List<Vector2Int> corridor = new List<Vector2Int>();
        var direction = Direction2D.GetRandomCardinalDirection();
        var currentPosition = startPosition;
        corridor.Add(currentPosition);

        for (int i = 0; i < corridorLength; i++)
        {
            currentPosition += direction;
            corridor.Add(currentPosition);
        }

        return corridor;
    }


    // BoundsInt - nos da un area (box) determinada por un punto y el tamaño de la box
    public static List<BoundsInt> BinarySpacePartitioning(BoundsInt spaceToSplit, int minWidth, int minHeight)
    {
        Queue<BoundsInt> roomsQueue = new Queue<BoundsInt>();
        List<BoundsInt> roomsList = new List<BoundsInt>();
        roomsQueue.Enqueue(spaceToSplit);

        // Si tenemos rooms en la cola vamos a realizar el algoritmo BSP
        while (roomsQueue.Count > 0)
        {
            var room = roomsQueue.Dequeue();

            // Descartamos áreas demasiado pequeñas
            if(room.size.x < minWidth || room.size.y < minHeight) continue;

            // Comprobamos si el área es lo bastante grande para poder cortarse por la mitad
            bool canSplitH = room.size.y >= minHeight * 2;
            bool canSplitV = room.size.x >= minWidth * 2;

            // Si puede cortarse en ambas direcciones, elegimos una al azar
            if (canSplitH && canSplitV)
            {
                if (Random.value < 0.5f)
                {
                    SplitHorizontally(room, roomsQueue, minHeight);
                } 
                else { SplitVertically(room, roomsQueue, minWidth); }
            }
            
            // Solo se puede cortar en una dirección
            else if (canSplitH){
                SplitHorizontally(room, roomsQueue, minHeight);
            } 
            else if (canSplitV){
                SplitVertically(room, roomsQueue, minWidth);
            }  
            // No se puede dividir por lo que es una habitación final válida
            else {
                roomsList.Add(room);
            }
        }

        return roomsList;
    }


    private static void SplitVertically(BoundsInt room, Queue<BoundsInt> roomsQueue, int minWidth)
    {
        // Range es exlusivo del límite superior
        // La coordenada x de donde se va a realizar la división
        // Ambas mitades cumplen con el mínimo de anchura
        var xSplit = Random.Range(minWidth, room.size.x - minWidth);

        // room.min representa la esquina inferior izquierda del BoundsInt 
        BoundsInt room1 = new BoundsInt(room.min, new Vector3Int(xSplit, room.size.y, room.size.z));

        BoundsInt room2 = new BoundsInt(new Vector3Int(room.min.x + xSplit, room.min.y, room.min.z), 
                                        new Vector3Int(room.size.x - xSplit, room.size.y, room.size.z));

        roomsQueue.Enqueue(room1);
        roomsQueue.Enqueue(room2);
    }

    private static void SplitHorizontally(BoundsInt room, Queue<BoundsInt> roomsQueue, int minHeight)
    {
        // Range es exlusivo del límite superior
        // La coordenada x de donde se va a realizar la división
        // Ambas mitades cumplen con el mínimo de altura
        var ySplit = Random.Range(minHeight, room.size.y - minHeight);

        // room.min representa la esquina inferior izquierda del BoundsInt 
        BoundsInt room1 = new BoundsInt(room.min, new Vector3Int(room.size.x, ySplit, room.size.z));

        BoundsInt room2 = new BoundsInt(new Vector3Int(room.min.x, room.min.y + ySplit, room.min.z), 
                                        new Vector3Int(room.size.x, room.size.y - ySplit, room.size.z));

        roomsQueue.Enqueue(room1);
        roomsQueue.Enqueue(room2);
    }
}
