using System.Collections.Generic;

public class PathfindingNode {
    public int x;
    public int z;

    public bool obstacle;
    public List<PathfindingNode> neighbors;

    public int index => z * Map.size + x;

    public PathfindingNode(int x, int z) {
        this.x = x;
        this.z = z;
    }
}
