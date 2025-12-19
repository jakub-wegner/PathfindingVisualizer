using UnityEngine;

public class Map : MonoBehaviour {
    private readonly int mapSize = 40;

    [SerializeField] private Material gridMaterial;

    private void Start() {
        CreateMap();
    }
    public void CreateMap() {
        float meshSize = mapSize * .5f + 5f;
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

        MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();
        meshFilter.mesh = mesh;

        MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();
        meshRenderer.material = gridMaterial;
        gridMaterial.SetFloat("_MapSize", mapSize);
    }
}
