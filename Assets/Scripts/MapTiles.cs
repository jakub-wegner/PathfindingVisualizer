using UnityEngine;

public class MapTiles : MonoBehaviour {
    public static MapTiles Instance { get; private set; }

    public float materialStepDelay;
    public float materialFadeInDuration;

    [SerializeField] private Material tilesMaterial;

    private ComputeBuffer visitOrderBuffer;

    private void Awake() {
        Instance = this;
        Hide();
    }

    public void Initialize(Mesh mesh) {
        MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();
        meshFilter.mesh = mesh;

        MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();
        meshRenderer.material = tilesMaterial;
        tilesMaterial.SetFloat("_MapSize", Map.mapSize);
    }

    public void Show(int[] visitOrder) {
        float[] bufferData = new float[visitOrder.Length];
        for (int i = 0; i < visitOrder.Length; i++)
            bufferData[i] = visitOrder[i];

        visitOrderBuffer = new ComputeBuffer(Map.mapSize * Map.mapSize, sizeof(float));
        visitOrderBuffer.SetData(bufferData);

        tilesMaterial.SetBuffer("_VisitOrder", visitOrderBuffer);
        tilesMaterial.SetFloat("_StartTime", Time.time);

        gameObject.SetActive(true);
    }
    public void Hide() {
        gameObject.SetActive(false);
    }

    private void OnDestroy() {
        if (visitOrderBuffer != null)
            visitOrderBuffer.Release();
    }

    public void SetSpeed(int preset) {
        switch (preset) {
            case 0:
                materialStepDelay = .4f;
                materialFadeInDuration = .5f;
                break;
            case 1:
                materialStepDelay = .12f;
                materialFadeInDuration = .3f;
                break;
            case 2:
                materialStepDelay = .03f;
                materialFadeInDuration = .2f;
                break;
            case 3:
                materialStepDelay = 0.002f;
                materialFadeInDuration = .1f;
                break;
        }
        tilesMaterial.SetFloat("_StepDelay", materialStepDelay);
        tilesMaterial.SetFloat("_FadeInDuration", materialFadeInDuration);
    }
}
