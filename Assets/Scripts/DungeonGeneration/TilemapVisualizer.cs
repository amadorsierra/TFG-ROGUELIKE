using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapVisualizer : MonoBehaviour
{
    // Se usa SerializeField para que se vea en el inspector de Unity
    [SerializeField]
    private Tilemap floorTilemap, wallTilemap;
    [SerializeField]
    // Se puede hacer un array y elegir una tile del floor random para que haya más diseños
    private TileBase floorTile, wallTop;

    // IEnumerable - Interfaz base en C# que permite recorrer colecciones de elementos mediante un bucle foreach
    public void PaintFloorTiles(IEnumerable<Vector2Int> floorPositions)
    {
        // 1. Pintamos el suelo NORMAL en la capa de SUELO
        PaintTiles(floorPositions, floorTilemap, floorTile);
    }

    private void PaintTiles(IEnumerable<Vector2Int> positions, Tilemap tilemap, TileBase tile)
    {
        foreach (var position in positions)
        {
            PaintSingleTile(tilemap, tile, position);
        }
    }

    private void PaintSingleTile(Tilemap tilemap, TileBase tile, Vector2Int position)
    {
        var tilePosition = tilemap.WorldToCell((Vector3Int) position);
        tilemap.SetTile(tilePosition, tile);
        //tilemap.SetTile((Vector3Int)position, tile);
    }

    internal void PaintSingleBasicWall(Vector2Int position)
    {
        //PaintSingleTile(wallTilemap, wallTop, position);

        // Pintamos tambien un suelo para que no haya hueco entre pared y suelo
        //PaintSingleTile(floorTilemap, wallTop, position);

        // 1. Pintamos el SUELO en la capa de suelo (para tapar el hueco blanco del fondo)
        PaintSingleTile(floorTilemap, floorTile, position);

        // 2. Pintamos la PARED en la capa de pared (para que el autotile se conecte perfectamente)
        PaintSingleTile(wallTilemap, wallTop, position);
    }

    public void Clear()
    {
        floorTilemap.ClearAllTiles();
        wallTilemap.ClearAllTiles();
    }

}
