using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[System.Serializable]
public class SpawnableEntity
{
    public GameObject prefab;
    [Range(1, 5)] public int minQuantity = 1;
    [Range(1, 10)] public int maxQuantity = 3;
}

public class RoomContent : MonoBehaviour
{
    public enum RoomType { Spawn, Combat, Treasure, Boss }
    [Header("Configuración de la Sala")]
    public RoomType roomType;

    [Header("Spawns específicos para esta sala")]
    public List<SpawnableEntity> possibleEnemies;
    public GameObject chestPrefab;
    public GameObject bossKeyPrefab;

    // Método que llamará nuestro spawner externo para poblar la sala aleatoriamente
    public void InitializeRoom(Vector2Int roomCenter, HashSet<Vector2Int> roomTiles)
    {
        switch (roomType)
        {
            case RoomType.Spawn:
                // Las salas de spawn no tienen enemigos ni recompensas
                break;

            case RoomType.Treasure:
                SpawnAtCenterTile(chestPrefab, roomCenter, roomTiles);
                break;

            case RoomType.Boss:
                SpawnAtCenterTile(bossKeyPrefab, roomCenter, roomTiles);
                // Aquí podrías instanciar al jefe también
                break;

            case RoomType.Combat:
                SpawnEnemies(roomTiles);
                break;
        }
    }

    private void SpawnEnemies(HashSet<Vector2Int> roomTiles)
{
    if (possibleEnemies == null || possibleEnemies.Count == 0) return;

    // Filtramos las baldosas para quedarnos solo con las que tienen espacio alrededor (evita muros)
    List<Vector2Int> safeTiles = new List<Vector2Int>();
    foreach (var tile in roomTiles)
    {
        // Comprobamos que las 4 direcciones cardinales sean suelo (es decir, que no estén pegados a una pared)
        if (roomTiles.Contains(tile + Vector2Int.up) &&
            roomTiles.Contains(tile + Vector2Int.down) &&
            roomTiles.Contains(tile + Vector2Int.left) &&
            roomTiles.Contains(tile + Vector2Int.right))
        {
            safeTiles.Add(tile);
        }
    }

    // Si por lo general no hay suficientes baldosas seguras, recurrimos a las normales para no bloquear el spawn
    if (safeTiles.Count == 0) safeTiles = new List<Vector2Int>(roomTiles);

    foreach (var enemyData in possibleEnemies)
    {
        int amount = Random.Range(enemyData.minQuantity, enemyData.maxQuantity + 1);

        for (int i = 0; i < amount && safeTiles.Count > 0; i++)
        {
            int randomIndex = Random.Range(0, safeTiles.Count);
            Vector2Int spawnTile = safeTiles[randomIndex];

            Instantiate(enemyData.prefab, new Vector3(spawnTile.x + 0.5f, spawnTile.y + 0.5f, 0), Quaternion.identity, transform);
            
            safeTiles.RemoveAt(randomIndex); 
        }
    }
}

    private void SpawnAtRandomTile(GameObject prefabToSpawn, HashSet<Vector2Int> roomTiles)
    {
        if (prefabToSpawn == null || roomTiles.Count == 0) return;

        var tileList = new List<Vector2Int>(roomTiles);
        Vector2Int randomTile = tileList[UnityEngine.Random.Range(0, tileList.Count)];

        // AÑADIDO 'transform' AL FINAL también para cofres y llaves
        Instantiate(prefabToSpawn, new Vector3(randomTile.x, randomTile.y, 0), Quaternion.identity, transform);
    }

    private void SpawnAtCenterTile(GameObject prefabToSpawn, Vector2Int roomCenter, HashSet<Vector2Int> roomTiles)
{
    if (prefabToSpawn == null) return;

    // Directamente usamos la Key del diccionario
    Instantiate(prefabToSpawn, new Vector3(roomCenter.x + 0.5f, roomCenter.y + 0.5f, 0), Quaternion.identity, transform);
}

    // private void SpawnAtCenterTile(GameObject prefabToSpawn, Vector2Int roomCenter, HashSet<Vector2Int> roomTiles)
    // {
    //     if (prefabToSpawn == null) return;

    //     // Usamos la Key del diccionario (roomCenter) como objetivo principal.
    //     // Si justo ese centro no forma parte de los tiles de la sala, buscamos el más cercano de la lista.
    //     Vector2Int spawnTarget = roomCenter;

    //     if (!roomTiles.Contains(roomCenter))
    //     {
    //         float closestDist = float.MaxValue;
    //         foreach (var tile in roomTiles)
    //         {
    //             float dist = Vector2Int.Distance(tile, roomCenter);
    //             if (dist < closestDist)
    //             {
    //                 closestDist = dist;
    //                 spawnTarget = tile;
    //             }
    //         }
    //     }

    //     // Instanciamos centrado en la baldosa y emparentado a la sala
    //     Instantiate(prefabToSpawn, new Vector3(spawnTarget.x + 0.5f, spawnTarget.y + 0.5f, 0), Quaternion.identity, transform);
    // }
}