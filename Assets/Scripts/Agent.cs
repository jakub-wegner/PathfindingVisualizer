using System.Collections.Generic;
using UnityEngine;

public class Agent : MonoBehaviour {
    public static Agent Instance { get; private set; }

    private float speed = 8f;
    private List<PathfindingNode> path;

    private void Awake() {
        Instance = this;
        path = new List<PathfindingNode>();
    }

    private void Update() {
        if (path.Count > 0) {
            Vector3 targetPos = new Vector3(path[0].x, 0f, path[0].z);
            transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);
            if (transform.position == targetPos)
                path.RemoveAt(0);
        }
    }

    public void SetPath(List<PathfindingNode> newPath) {
        if (newPath == null)
            path.RemoveRange(1, path.Count - 1);
        else
            path = newPath;
    }
}
