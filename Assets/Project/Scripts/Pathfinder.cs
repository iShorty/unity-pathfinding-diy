using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;
using Project.Scripts.Project.Scripts;
using UnityEngine;

namespace Project.Scripts
{
    public class Pathfinder : MonoBehaviour
    {
        [SerializeField]
        private GraphSearchMode mode = GraphSearchMode.BreadthFirstSearch;

        [SerializeField]
        private Color startColor = Color.green;

        [SerializeField]
        private Color goalColor = Color.red;

        [SerializeField]
        private Color frontierColor = Color.magenta;

        [SerializeField]
        private Color exploredColor = Color.gray;

        [SerializeField]
        private Color pathColor = Color.cyan;

        [SerializeField]
        private Color arrowColor = new Color(0.85f, 0.85f, 0.85f, 1f);

        [SerializeField]
        private Color highlightColor = new Color(1f, 1f, 0.5f, 1f);

        [SerializeField]
        private bool showIterations = true;

        [SerializeField]
        private bool showColors = true;

        [SerializeField]
        private bool showArrows = true;

        [SerializeField]
        private bool exitOnGoal = true;

        private Node _startNode;
        private Node _goalNode;

        private Graph _graph;
        private GraphView _graphView;

        private Queue<Node> _frontierNodes;
        private List<Node> _exploredNodes;
        private List<Node> _pathNodes;

        private int _iterations;

        public bool IsComplete { get; private set; }

        [SuppressMessage("ReSharper", "ConditionIsAlwaysTrueOrFalse")]
        public void Initialize([NotNull] Graph graph, [NotNull] GraphView graphView, [NotNull] Node start,
            [NotNull] Node goal)
        {
            if (graph == null || graphView == null || start == null || goal == null)
            {
                Debug.LogWarning("Invalid data passed to initialize.");
                return;
            }

            if (start.Type == NodeType.Blocked || goal.Type == NodeType.Blocked)
            {
                Debug.LogWarning("Invalid start or end nodes selected.");
                return;
            }

            _graph = graph;
            _graphView = graphView;
            _startNode = start;
            _goalNode = goal;

            ShowColors();

            _frontierNodes = new Queue<Node>();
            _frontierNodes.Enqueue(start);

            _exploredNodes = new List<Node>();
            _pathNodes = new List<Node>();

            for (var x = 0; x < graph.Width; ++x)
            {
                for (var y = 0; y < graph.Height; ++y)
                {
                    var node = graph.Nodes[x, y];
                    node?.Reset();
                }
            }

            IsComplete = false;
            _iterations = 0;
            _startNode.DistanceTraveled = 0;
        }

        public IEnumerator Search(float timeStep = 0.1f)
        {
            var timeStart = Time.time;

            yield return null;
            while (_frontierNodes.Count > 0)
            {
                var node = _frontierNodes.Dequeue();
                ++_iterations;

                if (!_exploredNodes.Contains(node))
                {
                    _exploredNodes.Add(node);
                }

                switch (mode)
                {
                    case GraphSearchMode.BreadthFirstSearch:
                        ExpandFrontierBFS(node);
                        break;
                    case GraphSearchMode.Dijkstra:
                        ExpandFrontierDijkstra(node);
                        break;
                }

                var foundGoal = _frontierNodes.Contains(_goalNode);
                if (foundGoal)
                {
                    _pathNodes = GetPathNodes(_goalNode);
                }

                if (showIterations)
                {
                    ShowDiagnostics();
                    yield return new WaitForSeconds(timeStep);
                }

                if (foundGoal && exitOnGoal) break;
            }

            IsComplete = true;
            Debug.Log($"Elapsed time: {Time.time - timeStart} seconds.");

            ShowDiagnostics();
        }

        private void ShowDiagnostics()
        {
            if (showColors) ShowColors();
            if (showArrows) ShowNodeArrows();
        }

        private void ShowNodeArrows()
        {
            if (_graphView == null) return;
            if (_frontierNodes != null) _graphView.ShowNodeArrows(_frontierNodes, arrowColor);
            if (_pathNodes != null) _graphView.ShowNodeArrows(_pathNodes, highlightColor);
        }

        private void ExpandFrontierBFS([NotNull] Node node)
        {
            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            if (node == null) return;
            foreach (var neighbor in node.Neighbors)
            {
                if (_exploredNodes.Contains(neighbor)) continue;
                if (_frontierNodes.Contains(neighbor)) continue;

                neighbor.Previous = node;
                _frontierNodes.Enqueue(neighbor);
            }
        }

        private void ExpandFrontierDijkstra([NotNull] Node node)
        {
            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            if (node == null) return;
            foreach (var neighbor in node.Neighbors)
            {
                if (_exploredNodes.Contains(neighbor)) continue;

                var distanceToNeighbor = _graph.GetNodeDistance(node, neighbor);
                var newDistanceTraveled = distanceToNeighbor + node.DistanceTraveled;

                if (float.IsInfinity(neighbor.DistanceTraveled) || neighbor.DistanceTraveled > newDistanceTraveled)
                {
                    neighbor.DistanceTraveled = newDistanceTraveled;
                    neighbor.Previous = node;
                }

                if (!_frontierNodes.Contains(neighbor)) _frontierNodes.Enqueue(neighbor);
            }
        }

        private void ShowColors() => ShowColors(_graphView, _startNode, _goalNode);

        [SuppressMessage("ReSharper", "ConditionIsAlwaysTrueOrFalse")]
        private void ShowColors([NotNull] GraphView graphView, [NotNull] Node start, [NotNull] Node goal)
        {
            if (graphView == null || start == null || goal == null) return;

            if (_frontierNodes != null) graphView.ColorNodes(_frontierNodes, frontierColor);
            if (_exploredNodes != null) graphView.ColorNodes(_exploredNodes, exploredColor);
            if (_pathNodes != null) graphView.ColorNodes(_pathNodes, pathColor);

            var startNodeView = graphView.GetView(start);
            if (startNodeView != null) startNodeView.ColorNode(startColor);

            var goalNodeView = graphView.GetView(goal);
            if (goalNodeView != null) goalNodeView.ColorNode(goalColor);
        }

        [SuppressMessage("ReSharper", "ConditionIsAlwaysTrueOrFalse")]
        [NotNull]
        private List<Node> GetPathNodes([NotNull] Node endNode)
        {
            var path = new List<Node>();
            while (endNode != null)
            {
                path.Add(endNode);
                endNode = endNode.Previous;
            }
            path.Reverse();
            return path;
        }
    }
}