public class PathfindingNodeState {
    public PathfindingNode parent;
}

public class BFSNodeState : PathfindingNodeState {
    public BFSNodeState(PathfindingNode parent = null) {
        this.parent = parent;
    }
}

public class DijkstraNodeState : PathfindingNodeState {
    public float cost;
    public DijkstraNodeState(PathfindingNode parent = null, float cost = 0f) {
        this.parent = parent;
        this.cost = cost;
    }
}

public class AStarNodeState : PathfindingNodeState {
    public float gCost;
    public float hCost;
    public float fCost => gCost + hCost;
    public AStarNodeState(PathfindingNode parent = null, float gCost = 0f, float hCost = 0f) {
        this.parent = parent;
        this.gCost = gCost;
        this.hCost = hCost;
    }
}
