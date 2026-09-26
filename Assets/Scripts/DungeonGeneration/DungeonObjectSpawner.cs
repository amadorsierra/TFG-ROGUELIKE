using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DungeonObjectSpawner : MonoBehaviour
{
    [Header("Prefabs de Salas Especiales")]
    public GameObject spawnRoomPrefab;
    public GameObject treasureRoomPrefab;
    public GameObject bossRoomPrefab;

    [Header("Variantes de Salas de Combate")]
    public GameObject[] combatRoomPrefabs;

    public GameObject player;

    // Lista para llevar el control de todo lo que spawneamos y poder borrarlo luego
    private List<GameObject> spawnedContentList = new List<GameObject>();

    public void SetupDungeonContent(Dictionary<Vector2Int, HashSet<Vector2Int>> roomsDictionary)
    {
        // 1. LIMPIEZA: Borramos todos los enemigos, cofres y salas anteriores
        ClearPreviousDungeon();

        List<Vector2Int> roomCenters = roomsDictionary.Keys.ToList();
        if (roomCenters.Count == 0) return;

        // 2. Elegir la sala de Spawn
        Vector2Int spawnRoomCenter = roomCenters[Random.Range(0, roomCenters.Count)];

        // 3. Ordenar por distancia para Boss y Treasure
        var sortedRooms = roomCenters
            .OrderByDescending(center => Vector2Int.Distance(spawnRoomCenter, center))
            .ToList();

        Vector2Int bossRoomCenter = sortedRooms.First();
        Vector2Int treasureRoomCenter = sortedRooms.Count > 1 ? sortedRooms[1] : bossRoomCenter;

        // 4. Posicionar al jugador
        if (player != null)
        {
            player.transform.position = new Vector3(spawnRoomCenter.x, spawnRoomCenter.y, 0);
        }

        // 5. Instanciar el contenido nuevo
        foreach (var roomCenter in sortedRooms)
        {
            GameObject prefabToInstantiate = null;
            RoomContent.RoomType currentType;

            if (roomCenter == spawnRoomCenter)
            {
                prefabToInstantiate = spawnRoomPrefab;
                currentType = RoomContent.RoomType.Spawn;
            }
            else if (roomCenter == bossRoomCenter)
            {
                prefabToInstantiate = bossRoomPrefab;
                currentType = RoomContent.RoomType.Boss;
            }
            else if (roomCenter == treasureRoomCenter)
            {
                prefabToInstantiate = treasureRoomPrefab;
                currentType = RoomContent.RoomType.Treasure;
            }
            else
            {
                if (combatRoomPrefabs != null && combatRoomPrefabs.Length > 0)
                {
                    prefabToInstantiate = combatRoomPrefabs[Random.Range(0, combatRoomPrefabs.Length)];
                }
                currentType = RoomContent.RoomType.Combat;
            }

            if (prefabToInstantiate != null && roomsDictionary.TryGetValue(roomCenter, out HashSet<Vector2Int> roomTiles))
            {
                // Instanciamos el contenedor de la sala y lo guardamos en la lista para poder borrarlo después
                GameObject contentInstance = Instantiate(prefabToInstantiate, new Vector3(roomCenter.x, roomCenter.y, 0), Quaternion.identity, transform);
                spawnedContentList.Add(contentInstance);

                RoomContent roomContent = contentInstance.GetComponent<RoomContent>();
                if (roomContent != null)
                {
                    roomContent.roomType = currentType;
                    
                    // Si el RoomContent spawnea enemigos, asegurate de que tambien los rastree o 
                    // simplemente se destruyen al destruir el contenedor de la sala.
                    roomContent.InitializeRoom(roomCenter, roomTiles);
                }
            }
        }
    }

    private void ClearPreviousDungeon()
    {
        // Destruimos cada objeto que se creó en la generación anterior
        foreach (var obj in spawnedContentList)
        {
            // Si estamos en Play Mode usamos Destroy, si estamos editando en el editor usamos DestroyImmediate
            if (Application.isPlaying)
            {
                Destroy(obj);
            }
            else
            {
                DestroyImmediate(obj);
            }
        }
        // Vaciamos la lista
        spawnedContentList.Clear();
    }
}