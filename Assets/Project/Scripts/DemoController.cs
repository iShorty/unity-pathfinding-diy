using UnityEngine;

namespace Project.Scripts
{
    public class DemoController : MonoBehaviour
    {
        [SerializeField]
        private MapData mapData;

        [SerializeField]
        private Graph graph;

        [SerializeField]
        private Pathfinder pathfinder;

        [SerializeField]
        private Vector2Int startCoordinates = Vector2Int.zero;

        [SerializeField]
        private Vector2Int goalCoordinates = new Vector2Int(15, 1);

        [SerializeField]
        private float timeStep = 0.1f;

        private void Start()
        {
            if (mapData == null || graph == null) return;

            var mapInstance = mapData.MakeMap();
            graph.Initialize(mapInstance);

            var graphView = graph.gameObject.GetComponent<GraphView>();
            if (graphView == null) return;

            graphView.Initialize(graph);

            if (!graph.IsWithinBounds(startCoordinates) || !graph.IsWithinBounds(goalCoordinates) ||
                pathfinder == null) return;
            var startNode = graph.GetNode(startCoordinates);
            var goalNode = graph.GetNode(goalCoordinates);
            pathfinder.Initialize(graph, graphView, startNode, goalNode);

            StartCoroutine(pathfinder.Search(timeStep));
        }
    }
}
