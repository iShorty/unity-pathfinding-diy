using System.Collections.Generic;
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

        public NodeView[,] Views { get; private set; }

        public void Initialize([NotNull] Graph graph)
        {
            if (graph == null) return;

            Views = new NodeView[graph.Width, graph.Height];

            foreach (var node in graph.Nodes)
            {
                var instance = Instantiate(nodeViewPrefab, Vector3.zero, Quaternion.identity, transform);
                instance.name = nodeViewPrefab.name;

                var nodeView = instance.GetComponent<NodeView>();
                if (nodeView == null) continue;

                nodeView.Initialize(node);
                Views[node.Index.x, node.Index.y] = nodeView;

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

        public void ColorNodes([NotNull] List<Node> nodes, Color color)
        {
            for (var i = 0; i < nodes.Count; ++i)
            {
                var node = nodes[i];
                if (node == null) continue;

                var view = Views[node.Index.x, node.Index.y];
                if (view == null) continue;

                view.ColorNode(color);
            }
        }

        public NodeView GetView([NotNull] Node node) => GetView(node.Index);

        public NodeView GetView(in Vector2Int index) => Views[index.x, index.y];
    }
}
