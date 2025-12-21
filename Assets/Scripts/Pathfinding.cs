using System.Collections.Generic;
using UnityEngine;

public class Pathfinding : MonoBehaviour {
    public static Pathfinding Instance { get; private set; }
    private UI UI => UI.Instance;
    private MapTiles MapTiles => MapTiles.Instance;
    private MapPath MapPath => MapPath.Instance;
    private Agent Agent => Agent.Instance;

    [SerializeField] private Camera cam;

    private PathfindingNode currentNode;

    private PathfindingNode[] nodes;
    Dictionary<PathfindingNode, PathfindingNodeState> nodeStates;
    List<PathfindingNode> path;

    private void Awake() {
        Instance = this;
    }

    public void Initialize() {
        nodes = new PathfindingNode[Map.mapSize * Map.mapSize];
        for (int x = 0; x < Map.mapSize; x++)
            for (int z = 0; z < Map.mapSize; z++) {
                PathfindingNode node = new PathfindingNode(x, z);
                nodes[z * Map.mapSize + x] = node;
            }
    }
    public void Generate() {
        currentNode = null;
        MapTiles.Hide();
        MapPath.Hide();
    }

    public void SetObstacles(List<Vector3> obstacles) {
        foreach (PathfindingNode node in nodes)
            node.obstacle = false;

        foreach (Vector3 obstacle in obstacles)
            GetNode(obstacle).obstacle = true;

        SetNeighbors_BFS();
    }
    public void SetNeighbors_BFS() {
        for (int x = 0; x < Map.mapSize; x++)
            for (int z = 0; z < Map.mapSize; z++) {
                PathfindingNode node = nodes[z * Map.mapSize + x];
                node.neighbors = new List<PathfindingNode>();
                if (node.obstacle)
                    continue;
                // neighbors
                void AddNeighbor(int x, int z) {
                    if (x >= 0 && x < Map.mapSize && z >= 0 && z < Map.mapSize) {
                        PathfindingNode neighbor = nodes[z * Map.mapSize + x];
                        if (!neighbor.obstacle) {
                            node.neighbors.Add(neighbor);
                            neighbor.neighbors.Add(node);
                        }
                    }
                }
                AddNeighbor(x, z - 1);
                AddNeighbor(x - 1, z);
            }
    }
    
    private void Update() {
        if (UI.mouseOverUI || !Agent.idle)
            return;

        PathfindingNode hovered = GetCurrentNode();
        if (Input.GetMouseButtonDown(0)) {
            MapTiles.Hide();
            MapPath.Hide();
            if (hovered != null && currentNode == hovered && path != null) {
                currentNode = null;
                Agent.SetPath(path);
            }
            else {
                currentNode = hovered;
                if (currentNode != null && !currentNode.obstacle)
                    path = CalculatePath_BFS();
            }
        }
    }

    public PathfindingNode GetNode(Vector3 pos) {
        int x = Mathf.RoundToInt(pos.x);
        int z = Mathf.RoundToInt(pos.z);

        if (x < 0 || x >= Map.mapSize || z < 0 || z >= Map.mapSize)
            return null;
        return nodes[z * Map.mapSize + x];
    }
    private PathfindingNode GetCurrentNode() {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        new Plane(Vector3.up, Vector3.zero).Raycast(ray, out float d);
        return GetNode(ray.GetPoint(d));
    }

    private List<PathfindingNode> CalculatePath_BFS() {
        PathfindingNode origin = Agent.node;
        PathfindingNode target = currentNode;

        nodeStates = new Dictionary<PathfindingNode, PathfindingNodeState>();
        Queue<PathfindingNode> queue = new Queue<PathfindingNode>();

        int[] visitOrder = new int[nodes.Length];
        for (int i = 0; i < nodes.Length; i++)
            visitOrder[i] = -1;
        int visitIndex = 0;

        nodeStates[origin] = new BFSNodeState();
        queue.Enqueue(origin);
        visitOrder[origin.index] = 0;

        // BFS
        while (queue.Count > 0) {
            PathfindingNode current = queue.Dequeue();

            visitOrder[current.index] = visitIndex;
            visitIndex++;

            if (current == target)
                break;

            foreach (PathfindingNode neighbor in current.neighbors) {
                if (nodeStates.ContainsKey(neighbor))
                    continue;
                nodeStates[neighbor] = new BFSNodeState(parent: current);
                queue.Enqueue(neighbor);
            }
        }

        MapTiles.Show(visitOrder);

        // no path
        if (target == null || !nodeStates.ContainsKey(target))
            return null;

        // path
        List<PathfindingNode> path = new List<PathfindingNode>();
        PathfindingNode node = target;
        while (node != origin) {
            path.Add(node);
            node = nodeStates[node].parent;
        }
        path.Add(origin);
        path.Reverse();

        MapPath.Show(path, visitIndex * MapTiles.materialStepDelay + MapTiles.materialFadeInDuration * .5f);

        return path;
    }
}
