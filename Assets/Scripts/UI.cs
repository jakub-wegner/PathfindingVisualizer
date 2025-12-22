using UnityEngine;
using UnityEngine.EventSystems;
using JakubWegner.UIEngine;

public class UI : MonoBehaviour {
    public static UI Instance { get; private set; }
    private Pathfinding Pathfinding => Pathfinding.Instance;
    private MapTiles MapTiles => MapTiles.Instance;
    private MapPath MapPath => MapPath.Instance;

    [SerializeField] private EventSystem eventSystem;

    [SerializeField] private GameObject buttonsHolder;
    [SerializeField] private GameObject settings;

    [SerializeField] private Button[] speedButtons;
    [SerializeField] private Button[] algorithmsButtons;

    public bool mouseOverUI => eventSystem.IsPointerOverGameObject() || settings.activeSelf;

    private void Awake() {
        Instance = this;
        CloseSettings();
    }

    public void Initialize() {
        for (int i = 0; i < speedButtons.Length; i++) {
            int preset = i;
            speedButtons[i].onClick.AddListener(() => SetSpeed(preset));
        }
        for (int i = 0; i < algorithmsButtons.Length; i++) {
            int index = i;
            algorithmsButtons[i].onClick.AddListener(() => SetAlgorithm(index));
        }
        SetSpeed(1);
        SetAlgorithm(2);
    }

    public void OpenSettings() {
        buttonsHolder.SetActive(false);
        settings.SetActive(true);
    }

    public void CloseSettings() {
        buttonsHolder.SetActive(true);
        settings.SetActive(false);
    }

    public void SetSpeed(int preset) {
        Pathfinding.ResetPathfinding();
        for (int i = 0; i < speedButtons.Length; i++)
            speedButtons[i].SetActive(preset != i);
        MapTiles.SetSpeed(preset);
        MapPath.SetSpeed(preset);
    }

    public void SetAlgorithm(int index) {
        Pathfinding.ResetPathfinding();
        for (int i = 0; i < algorithmsButtons.Length; i++)
            algorithmsButtons[i].SetActive(index != i);
        Pathfinding.SetAlgorithm(index);
    }
}
