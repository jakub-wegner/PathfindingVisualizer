using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.PlayerSettings;
using static UnityEngine.UIElements.UxmlAttributeDescription;

public class Agent : MonoBehaviour {
    public static Agent Instance { get; private set; }
    private Pathfinding Pathfinding => Pathfinding.Instance;

    private readonly float speed = 8f;

    private List<PathfindingNode> path;

    public PathfindingNode node => Pathfinding.GetNode(transform.position);
    public bool idle => path == null || path.Count == 0;

    private void Awake() {
        Instance = this;
    }

    public void Generate() {
        Vector3 pos;
        do {
            pos = new Vector3(Random.Range(0, Map.mapSize), 0f, Random.Range(0, Map.mapSize));
        } while (Pathfinding.GetNode(pos).obstacle);
        transform.position = pos;
        path = null;
    }

    private void Update() {
        if (path != null && path.Count > 0) {
            Vector3 targetPos = new Vector3(path[0].x, 0f, path[0].z);
            transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);
            if (transform.position == targetPos)
                path.RemoveAt(0);
        }
    }

    public void SetPath(List<PathfindingNode> newPath) {
        path = newPath;
    }
}
