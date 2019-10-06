using JetBrains.Annotations;
using Project.Scripts.Project.Scripts;
using UnityEngine;

namespace Project.Scripts
{
    [RequireComponent(typeof(Graph))]
    public class GraphView : MonoBehaviour
    {
        [SerializeField]
        private GameObject nodeViewPrefab;

        [SerializeField]
        private Color baseColor = Color.white;

        [SerializeField]
        private Color wallColor = Color.black;

        public void Initialize([NotNull] Graph graph)
        {
            if (graph == null) return;

            foreach (var node in graph.Nodes)
            {
                var instance = Instantiate(nodeViewPrefab, Vector3.zero, Quaternion.identity, transform);
                instance.name = nodeViewPrefab.name;

                var nodeView = instance.GetComponent<NodeView>();
                if (nodeView == null) continue;

                nodeView.Initialize(node);
                switch (node.Type)
                {
                    case NodeType.Open:
                        nodeView.ColorNode(baseColor);
                        break;
                    case NodeType.Blocked:
                        nodeView.ColorNode(wallColor);
                        break;
                }
            }
        }
    }
}
