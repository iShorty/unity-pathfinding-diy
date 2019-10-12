using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;
using Project.Scripts.Project.Scripts;
using UnityEngine;

namespace Project.Scripts
{
    public class Graph : MonoBehaviour
    {
        [NotNull]
        public static readonly Vector2Int[] AllDirections = {
            new Vector2Int(0, -1),
            new Vector2Int(0, 1),

            new Vector2Int(1, -1),
            new Vector2Int(1, 0),
            new Vector2Int(1, 1),

            new Vector2Int(-1, -1),
            new Vector2Int(-1, 0),
            new Vector2Int(-1, 1)
        };

        [NotNull]
        private List<Node> _walls = new List<Node>();
        private int[,] _mapData;
        private int _mapWidth;
        private int _mapHeight;

        public Node[,] Nodes { get; private set; }

        public int Width => _mapWidth;

        public int Height => _mapHeight;

        public void Initialize([NotNull] int[,] mapData)
        {
            _mapData = mapData;
            _mapWidth = mapData.GetLength(0);
            _mapHeight = mapData.GetLength(1);

            // Build the map.
            Nodes = new Node[_mapWidth, _mapHeight];
            for (var yIndex = 0; yIndex < _mapHeight; ++yIndex)
            {
                for (var xIndex = 0; xIndex < _mapWidth; ++xIndex)
                {
                    // TODO: Check the indexing order - seems inefficient.
                    var type = (NodeType)_mapData[xIndex, yIndex];
                    var node = new Node(new Vector2Int(xIndex, yIndex), type);

                    Nodes[xIndex, yIndex] = node;
                    if (type == NodeType.Blocked)
                    {
                        _walls.Add(node);
                    }
                }
            }

            // Obtain the neighbors.
            for (var yIndex = 0; yIndex < _mapHeight; ++yIndex)
            {
                for (var xIndex = 0; xIndex < _mapWidth; ++xIndex)
                {
                    // TODO: Check the indexing order - seems inefficient.
                    var node = Nodes[xIndex, yIndex];
                    if (node.Type == NodeType.Blocked) continue;
                    GetNeighbors(new Vector2Int(xIndex, yIndex), node.Neighbors);
                }
            }
        }

        [SuppressMessage("ReSharper", "ConditionIsAlwaysTrueOrFalse")]
        public float GetNodeDistance([NotNull] Node a, [NotNull] Node b)
        {
            if (a == null || b == null) return Mathf.Infinity;
            var dx = Mathf.Abs(a.Index.x - b.Index.x);
            var dy = Mathf.Abs(a.Index.y - b.Index.y);

            var min = Mathf.Min(dx, dy);
            var max = Mathf.Max(dx, dy);

            var diagonalSteps = min;
            var straightSteps = max - min;

            // Here, 1.4 is an approximation of sqrt(1^2 + 1^2) = sqrt(2)
            return 1.4f * diagonalSteps + straightSteps;
        }

        public bool IsWithinBounds(in Vector2Int pos) => pos.x >= 0 && pos.x < _mapWidth &&
                                                      pos.y >= 0 && pos.y < _mapHeight;

        public Node GetNode(in Vector2Int pos) => Nodes[pos.x, pos.y];

        [NotNull]
        public void GetNeighbors<TNodes>(in Vector2Int pos, [NotNull] TNodes nodeNeighbors)
            where TNodes : class, ICollection<Node>
            => GetNeighbors(pos, Nodes, AllDirections, nodeNeighbors);

        private void GetNeighbors<TDirections, TNodes>(in Vector2Int pos, [NotNull] Node[,] nodes, [NotNull] TDirections directions, [NotNull] TNodes nodeNeighbors)
            where TDirections : class, IReadOnlyList<Vector2Int>
            where TNodes : class, ICollection<Node>
        {
            Debug.Assert(nodeNeighbors != null, "nodeNeighbors != null");
            for (var i = 0; i < directions.Count; ++i)
            {
                var direction = directions[i];

                var neighborPos = pos + direction;
                if (!IsWithinBounds(neighborPos)) continue;

                var neighbor = nodes[neighborPos.x, neighborPos.y];
                if (neighbor == null || neighbor.Type == NodeType.Blocked) continue;

                nodeNeighbors.Add(neighbor);
            }
        }
    }
}
