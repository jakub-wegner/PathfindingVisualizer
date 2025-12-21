using UnityEngine;
using UnityEngine.EventSystems;

public class UI : MonoBehaviour {
    public static UI Instance { get; private set; }

    [SerializeField] private EventSystem eventSystem;

    [SerializeField] private GameObject buttonsHolder;
    [SerializeField] private GameObject settings;

    private void Awake() {
        Instance = this;
        CloseSettings();
    }

    public bool mouseOverUI => eventSystem.IsPointerOverGameObject() || settings.activeSelf;

    public void OpenSettings() {
        buttonsHolder.SetActive(false);
        settings.SetActive(true);
    }

    public void CloseSettings() {
        buttonsHolder.SetActive(true);
        settings.SetActive(false);
    }
}
