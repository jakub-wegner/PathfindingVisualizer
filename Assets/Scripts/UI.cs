using UnityEngine;
using UnityEngine.EventSystems;

public class UI : MonoBehaviour {
    public static UI Instance { get; private set; }

    [SerializeField] private EventSystem eventSystem;

    private void Awake() {
        Instance = this;
    }

    public bool mouseOverUI => eventSystem.IsPointerOverGameObject();
}
