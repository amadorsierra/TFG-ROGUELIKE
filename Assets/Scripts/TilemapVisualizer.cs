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
        PaintTiles(floorPositions, floorTilemap, floorTile);
    }

    private void PaintTiles(IEnumerable<Vector2Int> positions, Tilemap tilemap, TileBase tile)
    {
        foreach (var position in positions)
        {
            PaintSingleTile(tilemap, tile, position);
        }
    }

    private void PaintSingleTile(Tilemap tilemap, TileBase tile, Vector2Int positon)
    {
        var tilePositon = tilemap.WorldToCell((Vector3Int) positon);
        tilemap.SetTile(tilePositon, tile);
    }

    internal void PaintSingleBasicWall(Vector2Int position)
    {
        PaintSingleTile(wallTilemap, wallTop, position);
    }

    public void Clear()
    {
        floorTilemap.ClearAllTiles();
        wallTilemap.ClearAllTiles();
    }
}
