using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class MapPath : MonoBehaviour {
    public static MapPath Instance { get; private set; }

    [SerializeField] private Material pathMaterial;

    private ComputeBuffer pathBuffer;

    private float materialFadeInSpeed;

    private void Awake() {
        Instance = this;
        Hide();
    }

    public void Initialize(Mesh mesh) {
        MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();
        meshFilter.mesh = mesh;

        MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();
        meshRenderer.material = pathMaterial;
        pathMaterial.SetFloat("_MapSize", Map.size);

    }

    public void Show(List<PathfindingNode> path, float delay) {
        if (path == null)
            return;

        Texture2D pathTex = new Texture2D(path.Count, 1, TextureFormat.RGFloat, false, true);
        pathTex.filterMode = FilterMode.Point;
        pathTex.wrapMode = TextureWrapMode.Clamp;

        Vector2[] points = new Vector2[path.Count];
        for (int i = 0; i < path.Count; i++)
            points[i] = new Vector2(path[i].x, path[i].z);

        pathTex.SetPixelData(points, 0);
        pathTex.Apply(false, false);

        pathMaterial.SetTexture("_PathTex", pathTex);
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

    public void SetSpeed(int preset) {
        switch (preset) {
            case 0:
                materialFadeInSpeed = 3f;
                break;
            case 1:
                materialFadeInSpeed = 8f;
                break;
            case 2:
                materialFadeInSpeed = 25f;
                break;
            case 3:
                materialFadeInSpeed = 100f;
                break;
        }
        pathMaterial.SetFloat("_FadeInSpeed", materialFadeInSpeed);
    }
}
