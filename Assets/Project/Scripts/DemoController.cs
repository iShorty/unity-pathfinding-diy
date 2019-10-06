using UnityEngine;

namespace Project.Scripts
{
    public class DemoController : MonoBehaviour
    {
        [SerializeField]
        private MapData mapData;

        [SerializeField]
        private Graph graph;

        private void Start()
        {
            if (mapData == null || graph == null) return;

            var mapInstance = mapData.MakeMap();
            graph.Initialize(mapInstance);

            var graphView = graph.gameObject.GetComponent<GraphView>();
            if (graphView == null) return;

            graphView.Initialize(graph);
        }
    }
}
