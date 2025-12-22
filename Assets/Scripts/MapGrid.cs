using UnityEngine;

public class MapGrid : MonoBehaviour {
    public static MapGrid Instance { get; private set; }

    [SerializeField] private Material gridMaterial;

    private void Awake() {
        Instance = this;
    }

    public void Initialize(Mesh mesh) {
        MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();
        meshFilter.mesh = mesh;

        MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();
        meshRenderer.material = gridMaterial;
        gridMaterial.SetFloat("_MapSize", Map.size);
    }
}
