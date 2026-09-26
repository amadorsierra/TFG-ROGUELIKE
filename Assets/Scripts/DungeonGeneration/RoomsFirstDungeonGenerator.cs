using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class RoomsFirstDungeonGenerator : SimpleDungeonWalkDungeonGenerator
{
    [SerializeField]
    private int minRoomWidth = 4, minRoomHeight = 4;
    [SerializeField]
    private int dungeonWidth = 20, dungeonHeight = 20;

    // Al dejar un offset de 1, dejamos una baldosa libre entre rooms.
    // Así no fusionamos el suelo de dos rooms que han sido cortados.
    [SerializeField]
    [Range(0, 10)]
    private int offset = 1;

    [SerializeField]
    private int minRoomNumber = 8;

    [SerializeField]
    private int corridorWidth = 3;

    [SerializeField]
    [Range(0.1f, 1f)]
    private float roomDensity = 0.6f;

    // Para que los rooms sean de forma aleatoria o cuadrados
    [SerializeField]
    private bool useRandomWalkRooms = false;

    [SerializeField]
    private DungeonObjectSpawner objectSpawner;

    // PCG DATA
    // Tiene que como key la posición donde se crea el room y como
    // valor las posiciones (tiles) que abarca el room.
    private Dictionary<Vector2Int, HashSet<Vector2Int>> roomsDictionary
            = new Dictionary<Vector2Int, HashSet<Vector2Int>>();

    protected override void RunProceduralGeneratation()
    {
        roomsDictionary.Clear(); // Limpiamos datos de generaciones anteriores
        CreateRooms(); 
    }

    private void CreateRooms()
    {
        var allRooms = ProceduralGenerationAlgorithms.BinarySpacePartitioning
                                                    (new BoundsInt((Vector3Int) startPosition, 
                                                    new Vector3Int(dungeonWidth, dungeonHeight, 0))
                                                    , minRoomWidth, minRoomHeight);

        // Filtrado de densidad, dejamos la mazmorra con menos salas
        List<BoundsInt> activeRooms = new List<BoundsInt>();
        foreach (var room in allRooms)
        {
            if (Random.value <= roomDensity)
            {
                activeRooms.Add(room);
            }
        }
        
        // Mientras las salas activas sean menos que el mínimo de salas que queremos
        // Y queden salas disponibles en la lista original para rescatar
        while(activeRooms.Count < minRoomNumber && activeRooms.Count < allRooms.Count)
        {
            BoundsInt randomRoom = allRooms[Random.Range(0, allRooms.Count)];

            if(!activeRooms.Contains(randomRoom))
            {
                activeRooms.Add(randomRoom);
            }
        }

        HashSet<Vector2Int> floor = new HashSet<Vector2Int>();

        if(useRandomWalkRooms)
        {
            floor = CreateRandomWalkRooms(activeRooms);
        }
        else
        {
            floor = CreateSimpleRooms(activeRooms);
        }

        // // Para unir los rooms generados con BSP vamos uniendo los centros de los rooms más cercanos.
        List<Vector2Int> roomCenters = new List<Vector2Int>();
        
        foreach (var room in activeRooms)
        {
            roomCenters.Add((Vector2Int) Vector3Int.RoundToInt(room.center));
        }

        HashSet<Vector2Int> corridors = ConnectRooms(roomCenters);
        floor.UnionWith(corridors);


        // floor = DungeonSmoother.Smooth(floor, 2);
    
        tilemapVisualizer.PaintFloorTiles(floor);
        WallGenerator.CreateWalls(floor, tilemapVisualizer);
        

        // Una vez generado el mapa geométrico, delegamos toda la lógica de juego al Spawner
        if (objectSpawner != null)
        {
            objectSpawner.SetupDungeonContent(roomsDictionary);
        }
        
    }


    // Solo rellena huecos 1x1, el RandomWalk puede haber generado 
    // huecos 2x2 que este algoritmo no rellena.
    // Hacer más adelante otro algoritmo capaz de rellenar todo.
    private HashSet<Vector2Int> FillInternalHoles(HashSet<Vector2Int> floor)
    {
        HashSet<Vector2Int> cleanFloor = new HashSet<Vector2Int>(floor);
        HashSet<Vector2Int> potentialHoles = new HashSet<Vector2Int>();
        foreach (var position in floor)
        {
            foreach (var direction in Direction2D.cardinalDirectionsList)
            {
                Vector2Int neighbour = position + direction;
                // Si no hay un tile de suelo en ese punto (neighbour) 
                // se convierte en un potential hole
                if (!floor.Contains(neighbour))
                {
                    potentialHoles.Add(neighbour);
                }
            }
        }

        foreach (var hole in potentialHoles)
        {
            // Si el hueco está rodeado en todas las direcciones por suelo
            // quiere decir que es un hueco en el suelo y no una pared
            if (floor.Contains(hole + Vector2Int.up) &&
                floor.Contains(hole + Vector2Int.down) &&
                floor.Contains(hole + Vector2Int.left) &&
                floor.Contains(hole + Vector2Int.right))
            {
                cleanFloor.Add(hole); // Tapamos el agujero añadiéndolo al suelo
            }
        }

        return cleanFloor;
    }

    private HashSet<Vector2Int> CreateRandomWalkRooms(List<BoundsInt> activeRooms)
    {
        HashSet<Vector2Int> allFloors = new HashSet<Vector2Int>();
        foreach (var room in activeRooms)
        {
            var roomCenter = (Vector2Int) Vector3Int.RoundToInt(room.center);
            // Generamos el suelo del room
            var roomFloor = RunRandomWalk(randomWalkParameters, roomCenter);
            roomFloor = RoomHoleFiller.FillAllHoles(roomFloor);

            HashSet<Vector2Int> filledRoomFloor = new HashSet<Vector2Int>();

            // Si una baldosa generada por el caminante se sale de estos 
            // límites, choca contra el muro invisible y no se dibuja.
            foreach (var position in roomFloor)
            {
                if (position.x >= (room.xMin + offset) && 
                    position.x <= (room.xMax - offset) && 
                    position.y >= (room.yMin + offset) && 
                    position.y <= (room.yMax - offset))
                {
                    filledRoomFloor.Add(position);
                }
            }

            SaveRoomData(roomCenter, filledRoomFloor); // Guardamos solo esta sala
            allFloors.UnionWith(filledRoomFloor);
        }

        return allFloors;
    }


    // Para el spawn de enemigos e items
    private void SaveRoomData(Vector2Int roomPosition, HashSet<Vector2Int> roomFloor)
    {
        roomsDictionary[roomPosition] = roomFloor;
    }

    // Elegimos aleatoriamente un room center y encontramos su room center más cercano, eliminamos
    // estos dos puntos de la lista de centers y los unimos con un pasillo. Iniciamos de vuelta
    // la busqueda del punto más cercano tenieno ahora como origen el anterior punto más cercano.
    private HashSet<Vector2Int> ConnectRooms(List<Vector2Int> roomCenters)
    {
        HashSet<Vector2Int> corridors = new HashSet<Vector2Int>();
        // Elegimos un room center aleatorio 
        var currentRoomCenter = roomCenters[Random.Range(0, roomCenters.Count)];
        roomCenters.Remove(currentRoomCenter);

        while(roomCenters.Count > 0)
        {
            Vector2Int closestRoomCenter = FindClosestPointTo(currentRoomCenter, roomCenters);
            roomCenters.Remove(closestRoomCenter);
            
            HashSet<Vector2Int> newCorridor = CreateCorridor(currentRoomCenter, closestRoomCenter);
            corridors.UnionWith(newCorridor);
            currentRoomCenter = closestRoomCenter;
        }

        return corridors;
    }

    void AddWidePosition(Vector2Int centerPos, HashSet<Vector2Int> corridor)
    {
        // 1. Calculamos la esquina inferior izquierda de nuestro "pincel"
        // Si el ancho es 3, el pincel empieza 1 baldosa a la izquierda y 1 hacia abajo del centro.
        int startX = centerPos.x - (corridorWidth / 2);
        int startY = centerPos.y - (corridorWidth / 2);

        // 2. Bucles clásicos de 0 a 'corridorWidth', súper intuitivos
        for (int x = 0; x < corridorWidth; x++)
        {
            for (int y = 0; y < corridorWidth; y++)
            {
                // Vamos sumando x e y a la posición inicial para pintar todo el cuadrado
                corridor.Add(new Vector2Int(startX + x, startY + y));
            }
        }
    }

    private HashSet<Vector2Int> CreateCorridor(Vector2Int currentRoomCenter, Vector2Int closestRoomCenter)
    {
        HashSet<Vector2Int> corridor = new HashSet<Vector2Int>();
        var position = currentRoomCenter;
        corridor.Add(position);

        AddWidePosition(position, corridor);
        // Queremos unir los dos room center con un pasillo
        // Vamos subiendo o bajando la y para quedarnos con la misma coordenada y los dos room center
        while(position.y != closestRoomCenter.y)
        {
            // Tenemos que ir hacia arriba
            if(closestRoomCenter.y > position.y)
            {
                position += Vector2Int.up;
            } else if (closestRoomCenter.y < position.y)
            {
                position += Vector2Int.down;
            }
            AddWidePosition(position, corridor);
        }

        // Vamos subiendo o bajando la x para quedarnos con la misma coordenada x los dos room center
        while(position.x != closestRoomCenter.x)
        {
            // Tenemos que ir hacia arriba
            if(closestRoomCenter.x > position.x)
            {
                position += Vector2Int.right;
            } else if (closestRoomCenter.x < position.x)
            {
                position += Vector2Int.left;
            }
            AddWidePosition(position, corridor);
        }

        return corridor;
    }

    private Vector2Int FindClosestPointTo(Vector2Int currentRoomCenter, List<Vector2Int> roomCenters)
    {
        Vector2Int closestPoint = Vector2Int.zero;
        float distanceBetween = float.MaxValue;

        foreach (var currentPoint in roomCenters)
        {
            float currentDistance = Vector2Int.Distance(currentPoint, currentRoomCenter);
            if (currentDistance < distanceBetween)
            {
                distanceBetween = currentDistance;
                closestPoint = currentPoint;
            }
        } 

        return closestPoint;
    }

    private HashSet<Vector2Int> CreateSimpleRooms(List<BoundsInt> roomsList)
    {
        HashSet<Vector2Int> allFloors = new HashSet<Vector2Int>();

        foreach (var room in roomsList)
        {
            HashSet<Vector2Int> roomFloor = new HashSet<Vector2Int>();
            Vector2Int roomCenter = (Vector2Int) Vector3Int.RoundToInt(room.center);

            // Dibujamos un recuadro perfecto, empezando a pintar después del offset 
            // y parando antes de llegar al tamaño total menos el offset.
            for (int col = offset; col < room.size.x - offset; col++)
            {
                for (int row = offset; row < room.size.y - offset; row++)
                {
                    Vector2Int position = (Vector2Int) room.min + new Vector2Int(col, row);
                    roomFloor.Add(position);
                }
            }

            SaveRoomData(roomCenter, roomFloor); // Guardamos solo esta sala
            allFloors.UnionWith(roomFloor);
        }
        return allFloors;
    }
}
