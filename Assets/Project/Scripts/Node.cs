using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Project.Scripts.Project.Scripts;
using UnityEngine;

namespace Project.Scripts
{
    public sealed class Node : IComparable<Node>
    {
        public readonly NodeType Type;
        public readonly Vector2Int Index;

        public float DistanceTraveled = Mathf.Infinity;

        public float Priority = 0f;

        public Node(Vector2Int index, NodeType type)
        {
            Index = index;
            Type = type;
            Position = new Vector3(index.x, 0, index.y);
        }

        public Vector3 Position { get; set; }

        [NotNull]
        public List<Node> Neighbors { get; } = new List<Node>();

        public Node Previous { get; set; }

        public void Reset()
        {
            Previous = null;
        }

        public int CompareTo([CanBeNull] Node other)
        {
            if (ReferenceEquals(this, other)) return 0;
            if (ReferenceEquals(null, other)) return 1;
            return Priority.CompareTo(other.Priority);
        }
    }
}
