using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "New Extruded Tile", menuName = "2D/Tiles/Extruded Tile")]
public class ExtrudedTile : Tile
{
    public bool isExtruded = true;
    public int extrusionDepth = 5;

    public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
    {
        base.GetTileData(position, tilemap, ref tileData);

        if (sprite == null)
        {
            Debug.LogWarning("Sprite is missing on ExtrudedTile!");
        }
    }
}