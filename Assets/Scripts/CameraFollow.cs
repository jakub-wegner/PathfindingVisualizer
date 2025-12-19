using UnityEngine;

public class CameraFollow : MonoBehaviour {
    private Agent Agent => Agent.Instance;

    private float followSpeed = 5f;
    
    private void LateUpdate() {
        transform.position = Vector3.Lerp(transform.position, Agent.transform.position, followSpeed * Time.deltaTime);
    }
}
