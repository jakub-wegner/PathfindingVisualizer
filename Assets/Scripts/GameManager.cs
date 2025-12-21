using UnityEngine;

public class GameManager : MonoBehaviour {
    private Map Map => Map.Instance;
    private Pathfinding Pathfinding => Pathfinding.Instance;
    private Obstacles Obstacles => Obstacles.Instance;
    private Agent Agent => Agent.Instance;
    private CameraFollow CameraFollow => CameraFollow.Instance;

    private void Start() {
        Map.Initialize();
        Pathfinding.Initialize();
        NewLayout();
    }

    public void NewLayout() {
        Pathfinding.Generate();
        Obstacles.Generate();
        Agent.Generate();
        CameraFollow.Generate();
    }
}
