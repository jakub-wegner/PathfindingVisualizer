using System.Collections.Generic;
using UnityEngine;

public class MapPath : MonoBehaviour {
    public static MapPath Instance { get; private set; }

    [SerializeField] private Material pathMaterial;

    private ComputeBuffer pathBuffer;

    private void Awake() {
        Instance = this;
        Hide();
    }

    public void Initialize(Mesh mesh) {
        MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();
        meshFilter.mesh = mesh;

        MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();
        meshRenderer.material = pathMaterial;
        pathMaterial.SetFloat("_MapSize", Map.mapSize);

    }

    public void Show(List<PathfindingNode> path, float delay) {
        Vector2[] bufferData = new Vector2[path.Count];
        for (int i = 0; i < path.Count; i++)
            bufferData[i] = new Vector2(path[i].x, path[i].z);

        pathBuffer = new ComputeBuffer(path.Count, sizeof(float) * 2);
        pathBuffer.SetData(bufferData);

        pathMaterial.SetBuffer("_Path", pathBuffer);
        pathMaterial.SetFloat("_PathSize", path.Count);
        pathMaterial.SetFloat("_StartTime", Time.time + delay);

        gameObject.SetActive(true);
    }
    public void Hide() {
        gameObject.SetActive(false);
    }

    private void OnDestroy() {
        if (pathBuffer != null)
            pathBuffer.Release();
    }
}
