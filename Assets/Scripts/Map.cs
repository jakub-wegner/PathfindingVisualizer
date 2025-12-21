using UnityEngine;

public class Map : MonoBehaviour {
    public static Map Instance { get; private set; }
    private MapGrid MapGrid => MapGrid.Instance;
    private MapTiles MapTiles => MapTiles.Instance;
    private MapPath MapPath => MapPath.Instance;

    public static readonly int mapSize = 10;

    private void Awake() {
        Instance = this;
    }

    public void Initialize() {
        transform.position = new Vector3(mapSize - 1, 0f, mapSize - 1) * .5f;

        float meshSize = mapSize * .5f + 100f;
        Mesh mesh = new Mesh() {
            vertices = new Vector3[4] {
                new Vector3(-meshSize, 0f, -meshSize),
                new Vector3( meshSize, 0f, -meshSize),
                new Vector3( meshSize, 0f,  meshSize),
                new Vector3(-meshSize, 0f,  meshSize),
            },
            triangles = new int[6] {
                0, 2, 1, 
                0, 3, 2,
            },
            normals = new Vector3[4] {
                Vector3.up,
                Vector3.up,
                Vector3.up,
                Vector3.up,
            },
        };

        MapGrid.Initialize(mesh);
        MapTiles.Initialize(mesh);
        MapPath.Initialize(mesh);
    }
}
