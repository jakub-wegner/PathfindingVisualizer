using UnityEngine;

public class MapTiles : MonoBehaviour {
    public static MapTiles Instance { get; private set; }

    [SerializeField] private Material tilesMaterial;

    public float materialStepDelay;
    public float materialFadeInDuration;

    private void Awake() {
        Instance = this;
        Hide();
    }

    public void Initialize(Mesh mesh) {
        MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();
        meshFilter.mesh = mesh;

        MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();
        meshRenderer.material = tilesMaterial;
        tilesMaterial.SetFloat("_MapSize", Map.size);
    }

    public void Show(int[] visitOrder) {
        Texture2D visitTex = new Texture2D(Map.size, Map.size, TextureFormat.RFloat, false, true);
        visitTex.filterMode = FilterMode.Point;
        visitTex.wrapMode = TextureWrapMode.Clamp;

        float[] pixels = new float[Map.size * Map.size];
        for (int i = 0; i < visitOrder.Length; i++)
            pixels[i] = visitOrder[i];

        visitTex.SetPixelData(pixels, 0);
        visitTex.Apply(false, false);

        tilesMaterial.SetTexture("_VisitTex", visitTex);
        tilesMaterial.SetFloat("_StartTime", Time.time);

        gameObject.SetActive(true);
    }
    public void Hide() {
        gameObject.SetActive(false);
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
