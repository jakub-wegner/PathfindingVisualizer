using UnityEngine;

public class Agent : MonoBehaviour {
    public static Agent Instance { get; private set; }

    private float speed = 8f;

    private Vector3 targetPos;

    private void Awake() {
        Instance = this;
    }

    private void Update() {
        if (Input.GetMouseButtonDown(0)) {
            targetPos = GetMousePos();
        }
        transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);
    }

    private Vector3 GetMousePos() {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        new Plane(Vector3.up, Vector3.zero).Raycast(ray, out float d);
        Vector3 pos = ray.GetPoint(d);
        pos.x = Mathf.Round(pos.x);
        pos.z = Mathf.Round(pos.z);
        return pos;
    }
}
