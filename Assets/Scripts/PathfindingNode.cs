using System.Collections.Generic;

public class PathfindingNode {
    public int x;
    public int z;
    public List<PathfindingNode> neighbors;

    public PathfindingNode(int x, int z) {
        this.x = x;
        this.z = z;
        neighbors = new List<PathfindingNode>();
    }
}
