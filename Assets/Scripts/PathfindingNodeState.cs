public class PathfindingNodeState {
    public PathfindingNode parent;
}

public class BFSNodeState : PathfindingNodeState {
    public BFSNodeState(PathfindingNode parent = null) {
        this.parent = parent;
    }
}
