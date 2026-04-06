using UnityEngine;
using UnityEngine.Tilemaps;

public class TileExtruder : MonoBehaviour
{
    public Tilemap designTilemap;   // The layer you paint on
    public float zOffset = 0.1f;    // Distance between 3D layers

    void Start()
    {
        GenerateDepth();
    }

    [ContextMenu("Generate Depth")] // Allows you to run this in the Editor
    public void GenerateDepth()
    {
        BoundsInt bounds = designTilemap.cellBounds;

        foreach (var pos in bounds.allPositionsWithin)
        {
            TileBase tile = designTilemap.GetTile(pos);

            // Check if this is our custom tile type and if it wants to be extruded
            if (tile is ExtrudedTile extrudedTile && extrudedTile.isExtruded)
            {
                CreateStack(pos, extrudedTile);
            }
        }
    }

    void CreateStack(Vector3Int position, ExtrudedTile tile)
    {
        for (int i = 1; i <= tile.extrusionDepth; i++)
        {
            // Calculate the new Z position for the "fake" 3D layer
            Vector3 spawnPos = designTilemap.CellToWorld(position);
            spawnPos.z += i * zOffset;

            // Logic: You can spawn a GameObject here, 
            // or Draw the sprite directly using Graphics.DrawSprite
            GameObject visualSlice = new GameObject("Depth_Slice_" + i);
            visualSlice.transform.position = spawnPos;

            var renderer = visualSlice.AddComponent<SpriteRenderer>();
            renderer.sprite = tile.sprite;
            renderer.color = Color.gray; // Darken deeper layers for a "shadow" effect
            renderer.sortingOrder = -i;  // Ensure they render behind the main tile
        }
    }
}