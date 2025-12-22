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

    private enum Algorithm {
        BFS,
        Dijkstra,
        AStar,
    }
    private Algorithm algorithm;

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
    public void ResetPathfinding() {
        currentNode = null;
        MapTiles.Hide();
        MapPath.Hide();
    }

    public void SetAlgorithm(int index) {
        switch (index) {
            case 0:
                algorithm = Algorithm.BFS; 
                break;
            case 1:
                algorithm = Algorithm.Dijkstra; 
                break;
            case 2:
                algorithm = Algorithm.AStar; 
                break;
        }
        SetNeighbors();
    }

    public void SetObstacles(List<Vector3> obstacles) {
        foreach (PathfindingNode node in nodes)
            node.obstacle = false;

        foreach (Vector3 obstacle in obstacles)
            GetNode(obstacle).obstacle = true;

        SetNeighbors();
    }
    public void SetNeighbors() {
        bool diagonal = algorithm == Algorithm.AStar || algorithm == Algorithm.Dijkstra;

        for (int x = 0; x < Map.mapSize; x++)
            for (int z = 0; z < Map.mapSize; z++) {
                PathfindingNode node = nodes[z * Map.mapSize + x];
                node.neighbors = new List<PathfindingNode>();

                if (node.obstacle)
                    continue;

                void AddNeighbor(int x, int z) {
                    if (x >= 0 && x < Map.mapSize && z >= 0 && z < Map.mapSize) {
                        PathfindingNode neighbor = nodes[z * Map.mapSize + x];
                        if (neighbor.obstacle)
                            return;
                        if (node.x != x && node.z != z)
                            if (nodes[node.z * Map.mapSize + x].obstacle || nodes[z * Map.mapSize + node.x].obstacle)
                                return;
                        node.neighbors.Add(neighbor);
                        neighbor.neighbors.Add(node);
                    }
                }

                AddNeighbor(x, z - 1);
                AddNeighbor(x - 1, z);
                if (diagonal) {
                    AddNeighbor(x - 1, z - 1);
                    AddNeighbor(x - 1, z + 1);
                }
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
                    RunPathfinding();
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

    private void RunPathfinding() {
        PathfindingNode origin = Agent.node;
        PathfindingNode target = currentNode;

        nodeStates = new Dictionary<PathfindingNode, PathfindingNodeState>();

        int visitedCount = 0;
        int[] visitOrder = new int[nodes.Length];
        for (int i = 0; i < nodes.Length; i++)
            visitOrder[i] = -1;

        switch (algorithm) {
            case Algorithm.BFS:
                BFS(origin, target, visitOrder, out visitedCount);
                break;
            case Algorithm.Dijkstra:
                Dijkstra(origin, target, visitOrder, out visitedCount);
                break;
            case Algorithm.AStar:
                AStar(origin, target, visitOrder, out visitedCount);
                break;
        }

        ReconstructPath(origin, target);

        MapTiles.Show(visitOrder);
        MapPath.Show(path, visitedCount * MapTiles.materialStepDelay + MapTiles.materialFadeInDuration * .5f);
    }

    private void BFS(PathfindingNode origin, PathfindingNode target, int[] visitOrder, out int visistedCount) {
        visistedCount = 0;

        Queue<PathfindingNode> queue = new Queue<PathfindingNode>();
        queue.Enqueue(origin);
        nodeStates[origin] = new BFSNodeState();

        while (queue.Count > 0) {
            PathfindingNode current = queue.Dequeue();

            visitOrder[current.index] = visistedCount++;

            if (current == target)
                break;

            foreach (PathfindingNode neighbor in current.neighbors) {
                if (nodeStates.ContainsKey(neighbor))
                    continue;
                nodeStates[neighbor] = new BFSNodeState(parent: current);
                queue.Enqueue(neighbor);
            }
        }
    }

    private void Dijkstra(PathfindingNode origin, PathfindingNode target, int[] visitOrder, out int visistedCount) {
        visistedCount = 0;

        List<PathfindingNode> openSet = new List<PathfindingNode> { origin };
        nodeStates[origin] = new DijkstraNodeState();

        while (openSet.Count > 0) {
            openSet.Sort((a, b) => ((DijkstraNodeState)nodeStates[a]).cost.CompareTo(((DijkstraNodeState)nodeStates[b]).cost));

            PathfindingNode current = openSet[0];
            openSet.RemoveAt(0);

            visitOrder[current.index] = visistedCount++;

            if (current == target)
                break;

            foreach (PathfindingNode neighbor in current.neighbors) {
                float cost = ((DijkstraNodeState)nodeStates[current]).cost + Distance(current, neighbor);
                if (!nodeStates.ContainsKey(neighbor)) {
                    nodeStates[neighbor] = new DijkstraNodeState(current, cost);
                    openSet.Add(neighbor);
                }
                else {
                    DijkstraNodeState state = (DijkstraNodeState)nodeStates[neighbor];
                    if (cost < state.cost) {
                        state.parent = current;
                        state.cost = cost;

                        if (!openSet.Contains(neighbor))
                            openSet.Add(neighbor);
                    }
                }
            }
        }
    }

    private void AStar(PathfindingNode origin, PathfindingNode target, int[] visitOrder, out int visistedCount) {
        visistedCount = 0;

        List<PathfindingNode> openSet = new List<PathfindingNode> { origin };
        nodeStates[origin] = new AStarNodeState(hCost: Distance(origin, target));

        while (openSet.Count > 0) {
            openSet.Sort((a, b) => {
                AStarNodeState sa = (AStarNodeState)nodeStates[a];
                AStarNodeState sb = (AStarNodeState)nodeStates[b];
                return sa.fCost == sb.fCost ? sa.hCost.CompareTo(sb.hCost) : sa.fCost.CompareTo(sb.fCost);
            });

            PathfindingNode current = openSet[0];
            openSet.RemoveAt(0);

            visitOrder[current.index] = visistedCount++;

            if (current == target)
                break;

            foreach (PathfindingNode neighbor in current.neighbors) {
                float gCost = ((AStarNodeState)nodeStates[current]).gCost + Distance(current, neighbor);
                if (!nodeStates.ContainsKey(neighbor)) {
                    nodeStates[neighbor] = new AStarNodeState(current, gCost, Distance(neighbor, target));
                    openSet.Add(neighbor);
                }
                else {
                    AStarNodeState state = (AStarNodeState)nodeStates[neighbor];
                    if (gCost < state.gCost) {
                        state.parent = current;
                        state.gCost = gCost;

                        if (!openSet.Contains(neighbor))
                            openSet.Add(neighbor);
                    }
                }
            }
        }
    }

    private float Distance(PathfindingNode a, PathfindingNode b) {
        float x = Mathf.Abs(a.x - b.x);
        float z = Mathf.Abs(a.z - b.z);
        return Mathf.Min(x, z) * 1.4f + Mathf.Abs(x - z);
    }

    private void ReconstructPath(PathfindingNode origin, PathfindingNode target) {
        if (target == null || !nodeStates.ContainsKey(target)) {
            path = null;
            return;
        }

        path = new List<PathfindingNode>();
        PathfindingNode node = target;
        while (node != origin) {
            path.Add(node);
            node = nodeStates[node].parent;
        }
        path.Add(origin);
        path.Reverse();
    }
}
