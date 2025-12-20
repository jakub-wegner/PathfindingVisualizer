using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinding : MonoBehaviour {
    private Agent Agent => Agent.Instance;

    [SerializeField] private Camera cam;

    private Coroutine pathfindingRoutine;
    private PathfindingNode currentNode;

    private PathfindingNode[] nodes;

    private void Start() {
        Initialize();
    }

    public void Initialize() {
        nodes = new PathfindingNode[Map.mapSize * Map.mapSize];
        for (int x = 0; x < Map.mapSize; x++)
            for (int z = 0; z < Map.mapSize; z++) {
                PathfindingNode node = new PathfindingNode(x, z);
                nodes[z * Map.mapSize + x] = node;

                // neighbors
                void AddNeighbor(int x, int z) {
                    if (x >= 0 && x < Map.mapSize && z >= 0 && z < Map.mapSize) {
                        PathfindingNode neighbor = nodes[z * Map.mapSize + x];
                        node.neighbors.Add(neighbor);
                        neighbor.neighbors.Add(node);
                    }
                }
                //AddNeighbor(x - 1, z - 1);
                AddNeighbor(x, z - 1);
                AddNeighbor(x - 1, z);
                //AddNeighbor(x - 1, z + 1);
            }
    }
    
    private void Update() {
        PathfindingNode node = GetCurrentNode();
        if (node != currentNode) {
            if (pathfindingRoutine != null) {
                StopCoroutine(pathfindingRoutine);
                pathfindingRoutine = null;
            }
            currentNode = node;
            if (node != null)
                pathfindingRoutine = StartCoroutine(PathfindingRoutine());
        }
    }

    private IEnumerator PathfindingRoutine() {
        yield return new WaitForSeconds(.01f);
        Debug.Log("start calculations");
        List<PathfindingNode> path = CalculatePath_BFS();
        Debug.Log("calculated");
        yield return new WaitUntil(() => Input.GetMouseButtonDown(0));
        Agent.SetPath(path);
        Debug.Log("set path");
        pathfindingRoutine = null;
    }

    private PathfindingNode GetNode(Vector3 pos) {
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
        PathfindingNode origin = GetNode(Agent.transform.position);
        PathfindingNode target = currentNode;

        Dictionary<PathfindingNode, BFSNodeState> nodeStates = new Dictionary<PathfindingNode, BFSNodeState>();
        Queue<PathfindingNode> queue = new Queue<PathfindingNode>();

        queue.Enqueue(origin);
        nodeStates[origin] = new BFSNodeState();

        // BFS
        while (queue.Count > 0) {
            PathfindingNode current = queue.Dequeue();

            if (current == target)
                break;

            foreach (PathfindingNode neighbor in current.neighbors) {
                if (nodeStates.ContainsKey(neighbor))
                    continue;
                nodeStates[neighbor] = new BFSNodeState(parent: current);
                queue.Enqueue(neighbor);
            }
        }

        // no path
        if (!nodeStates.ContainsKey(target))
            return null;

        // create path
        List<PathfindingNode> path = new List<PathfindingNode>();
        PathfindingNode node = target;
        while (node != origin) {
            path.Add(node);
            node = nodeStates[node].parent;
        }
        path.Reverse();

        return path;
    }
}
