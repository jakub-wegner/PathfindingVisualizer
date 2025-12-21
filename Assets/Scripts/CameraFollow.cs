using UnityEngine;

public class CameraFollow : MonoBehaviour {
    public static CameraFollow Instance { get; private set; }
    private Agent Agent => Agent.Instance;

    private float followSpeed = 3f;

    private void Awake() {
        Instance = this;
    }

    public void Generate() {
        transform.position = Agent.transform.position;
    }

    private void LateUpdate() {
        transform.position = Vector3.Lerp(transform.position, Agent.transform.position, followSpeed * Time.deltaTime);
    }
}
