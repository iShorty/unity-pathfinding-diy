using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;
using UnityEngine;

namespace Project.Scripts
{
    [RequireComponent(typeof(Graph))]
    public class GraphView : MonoBehaviour
    {
        [SerializeField]
        private GameObject nodeViewPrefab;

        [SerializeField]
        private MapData mapData;

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

                var color = mapData.GetColorFromNodeType(node);
                nodeView.ColorNode(color);
            }
        }

        public void ColorNodes([NotNull] IEnumerable<Node> nodes, Color color, float lerpValue = 0)
        {
            foreach (var node in nodes)
            {
                if (node == null) continue;

                var view = Views[node.Index.x, node.Index.y];
                if (view == null) continue;

                var newColor = color;
                if (lerpValue < 1.0f)
                {
                    var originalColor = mapData.GetColorFromNodeType(node);
                    newColor = Color.Lerp(originalColor, newColor, lerpValue);
                }

                view.ColorNode(newColor);
            }
        }

        [SuppressMessage("ReSharper", "ConditionIsAlwaysTrueOrFalse")]
        public void ShowNodeArrows([NotNull] Node node, Color color)
        {
            if (node == null) return;

            var view = GetView(node);
            if (view == null) return;

            view.ShowArrow(color);
        }

        public void ShowNodeArrows([NotNull] IEnumerable<Node> nodes, Color color)
        {
            foreach (var node in nodes)
            {
                ShowNodeArrows(node, color);
            }
        }

        public NodeView GetView([NotNull] Node node) => GetView(node.Index);

        public NodeView GetView(in Vector2Int index) => Views[index.x, index.y];
    }
}
