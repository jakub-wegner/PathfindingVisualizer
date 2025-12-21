using System.Collections.Generic;
using UnityEngine;

public class Obstacles : MonoBehaviour {
    public static Obstacles Instance { get; private set; }
    private Pathfinding Pathfinding => Pathfinding.Instance;

    private readonly int obstaclesCount = 20;

    [SerializeField] private GameObject obstaclePrefab;

    private void Awake() {
        Instance = this;
    }

    public void Generate() {
        for (int i = 0; i < transform.childCount; i++)
            Destroy(transform.GetChild(i).gameObject);

        List<Vector3> obstacles = new List<Vector3>();
        for (int i = 0; i < obstaclesCount; i++) {
            Vector3 pos;
            bool used;
            do {
                pos = new Vector3(Random.Range(0, Map.mapSize), 0f, Random.Range(0, Map.mapSize));
                used = false;
                for (int j = 0; j < i; j++)
                    if (obstacles[j] == pos) {
                        used = true;
                        break;
                    }
            } while (used);
            Instantiate(obstaclePrefab, pos, Quaternion.identity, transform);
            obstacles.Add(pos);
        }

        Pathfinding.SetObstacles(obstacles);
    }
}
