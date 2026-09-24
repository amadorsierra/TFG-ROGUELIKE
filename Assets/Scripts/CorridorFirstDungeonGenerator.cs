using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CorridorFirstDungeonGenerator : SimpleDungeonWalkDungeonGenerator
{
    [SerializeField]
    private int corridorLength = 14;
    [SerializeField]
    private int corridorCount = 5;
    [SerializeField]
    [Range(0.1f, 1)]
    private float roomPercent = 0.8f;

    protected override void RunProceduralGeneratation()
    {
        CorridorFirstGeneration();
    }

    private void CorridorFirstGeneration()
    {
        HashSet<Vector2Int> floorPositions = new HashSet<Vector2Int>();
        HashSet<Vector2Int> potentialRoomPositions = new HashSet<Vector2Int>();

        // 1. Generar la red de pasillos
        CreateCorridors(floorPositions, potentialRoomPositions);

        // 2. Identificar los callejones sin salida antes de crear nada
        List<Vector2Int> deadEnds = FindAllDeadEnds(floorPositions);

        // 3. Crear habitaciones unificando la lógica y respetando el porcentaje
        HashSet<Vector2Int> roomPositions = CreateRooms(potentialRoomPositions, deadEnds);

        // 4. Fusionar datos y renderizar
        floorPositions.UnionWith(roomPositions);
        tilemapVisualizer.PaintFloorTiles(floorPositions);
        WallGenerator.CreateWalls(floorPositions, tilemapVisualizer);
    }

    private void CreateRoomsAtDeadEnds(List<Vector2Int> deadEnds, HashSet<Vector2Int> roomFloors)
    {
        foreach (var position in deadEnds)
        {
            if(roomFloors.Contains(position) == false)
            {
                var room = RunRandomWalk(randomWalkParameters, position);
                roomFloors.UnionWith(room);
            }
        }
    }

    private List<Vector2Int> FindAllDeadEnds(HashSet<Vector2Int> floorPositions)
    {
        List<Vector2Int> deadEnds = new List<Vector2Int>();

        foreach (var position in floorPositions)
        {
            int neighbourCount = 0;
            foreach (var direction in Direction2D.cardinalDirectionsList)
            {
                // Checkeamos si contiene el neighbour
                if (floorPositions.Contains(position + direction))
                {
                    neighbourCount++;
                }
            }
            if(neighbourCount == 1)
            {
                deadEnds.Add(position);
            }
        }

        return deadEnds;
    }

    // private List<Vector2Int> FindAllDeadEnds(HashSet<Vector2Int> floorPositions)
    // {
    //     return floorPositions.Where(pos => Direction2D.cardinalDirectionsList.Count(dir => floorPositions.Contains(pos + dir)) == 1).ToList();
    // }

    private HashSet<Vector2Int> CreateRooms(HashSet<Vector2Int> potentialRoomPositions, List<Vector2Int> deadEnds)
    {
        HashSet<Vector2Int> roomPositions = new HashSet<Vector2Int>();

        // Paso A: Generar siempre una habitación en cada callejón sin salida
        foreach (var position in deadEnds)
        {
            var roomFloor = RunRandomWalk(randomWalkParameters, position);
            roomPositions.UnionWith(roomFloor);
            potentialRoomPositions.Remove(position); // Evitamos posibles duplicados ?
        }

        // Paso B: Calcular cuántas habitaciones nos faltan para cumplir el porcentaje real
        int targetRoomCount = Mathf.RoundToInt(potentialRoomPositions.Count * roomPercent);
        int roomsRemaining = targetRoomCount - deadEnds.Count;

        if (roomsRemaining > 0)
        {
            var roomsToCreate = potentialRoomPositions.OrderBy(x => Guid.NewGuid()).Take(roomsRemaining);

            foreach (var roomPosition in roomsToCreate)
            {
                var roomFloor = RunRandomWalk(randomWalkParameters, roomPosition);
                roomPositions.UnionWith(roomFloor);
            }
        }

        return roomPositions;
    }

    private void CreateCorridors(HashSet<Vector2Int> floorPositions, HashSet<Vector2Int> potentialRoomPositions)
    {
        var currentPosition = startPosition;
        potentialRoomPositions.Add(currentPosition);

        for (int i = 0; i < corridorCount; i++)
        {
            var corridor = ProceduralGenerationAlgorithms.RandomWalkCorridor(currentPosition, corridorLength);
            currentPosition = corridor[corridor.Count - 1];
            // Al final de cada pasillo es un posible punto para generar una sala
            potentialRoomPositions.Add(currentPosition);
            floorPositions.UnionWith(corridor);
        }
    }
}
