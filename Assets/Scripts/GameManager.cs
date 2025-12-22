using UnityEngine;

public class GameManager : MonoBehaviour {
    private Map Map => Map.Instance;
    private Pathfinding Pathfinding => Pathfinding.Instance;
    private UI UI => UI.Instance;
    private Obstacles Obstacles => Obstacles.Instance;
    private Agent Agent => Agent.Instance;
    private CameraFollow CameraFollow => CameraFollow.Instance;

    private void Start() {
        Map.Initialize();
        Pathfinding.Initialize();
        UI.Initialize();
        GenerateNew();
    }

    public void GenerateNew() {
        Pathfinding.ResetPathfinding();
        Obstacles.Generate();
        Agent.ResetAgent();
        CameraFollow.ResetCamera();
    }
}
