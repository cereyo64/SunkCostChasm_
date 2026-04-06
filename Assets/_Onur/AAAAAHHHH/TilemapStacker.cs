using UnityEngine;
using UnityEngine.Tilemaps;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class TilemapStacker : MonoBehaviour
{
    [Header("Setup")]
    [Tooltip("The Tilemap layer you want to extrude (e.g., your Ground or Walls)")]
    public Tilemap sourceTilemap;

    [Header("Stack Settings")]
    [Range(1, 30)] public int layerCount = 5;
    [Tooltip("Small offsets create the 'solid' look. Try Y: -0.05, Z: 0.1 for a 5-degree tilt.")]
    public Vector3 offsetPerLayer = new Vector3(0, -0.05f, 0.1f);

    [Header("Cleanup")]
    public bool removeCollidersOnCopies = true;
    public string containerName = "Tilemap_Depth_Container";

    public void GenerateStack()
    {
        if (sourceTilemap == null)
        {
            Debug.LogError("Assign a Source Tilemap first!");
            return;
        }

        // 1. Clear old container to prevent overlapping stacks
        GameObject existingContainer = GameObject.Find(containerName);
        if (existingContainer != null)
        {
            DestroyImmediate(existingContainer);
        }

        GameObject container = new GameObject(containerName);
        // Match the container to the grid position
        container.transform.position = sourceTilemap.transform.position;
        container.transform.parent = sourceTilemap.transform.parent;

        // 2. Loop to create layers
        for (int i = 0; i < layerCount; i++)
        {
            // Duplicate the entire GameObject
            GameObject layerObj = Instantiate(sourceTilemap.gameObject, container.transform);
            layerObj.name = $"Depth_Layer_{i + 1}";

            // Position with cumulative offset
            layerObj.transform.localPosition = offsetPerLayer * (i + 1);

            // 3. Strip logic from the visual copies
            if (removeCollidersOnCopies)
            {
                // Remove physics components so they don't interfere with the player
                foreach (var col in layerObj.GetComponentsInChildren<Collider2D>()) DestroyImmediate(col);
                var rb = layerObj.GetComponent<Rigidbody2D>();
                if (rb != null) DestroyImmediate(rb);
            }

            // 4. Adjust Rendering Order
            TilemapRenderer tr = layerObj.GetComponent<TilemapRenderer>();
            if (tr != null)
            {
                // Push the copy behind the original in the sorting layer
                tr.sortingOrder = sourceTilemap.GetComponent<TilemapRenderer>().sortingOrder - (i + 1);
            }

            // Ensure the copy isn't accidentally disabled
            layerObj.SetActive(true);
        }

        Debug.Log($"Generated {layerCount} layers for {sourceTilemap.name}.");
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(TilemapStacker))]
public class TilemapStackerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        TilemapStacker script = (TilemapStacker)target;

        GUILayout.Space(10);
        if (GUILayout.Button("GENERATE 3D STACK", GUILayout.Height(35)))
        {
            script.GenerateStack();
        }
    }
}
#endif