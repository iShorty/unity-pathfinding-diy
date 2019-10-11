using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;
using UnityEngine;

namespace Project.Scripts
{
    public class NodeView : MonoBehaviour
    {
        [SerializeField]
        private GameObject tile;

        [SerializeField]
        [Range(0, 0.5f)]
        private float borderSize = 0.15f;

        [SerializeField]
        private GameObject arrow;

        private Node _node;

        [SuppressMessage("ReSharper", "ConditionIsAlwaysTrueOrFalse")]
        public void Initialize([NotNull] Node node)
        {
            if (node == null) return;
            if (tile == null) return;

            _node = node;

            var go = gameObject;
            go.name = $"Node {node.Position}";
            go.transform.position = node.Position;
            tile.transform.localScale = new Vector3(1f - borderSize, 1f, 1f - borderSize);

            EnableObject(arrow, false);
        }

        public void ColorNode(Color color) => ColorNode(color, tile);

        public void ColorNode(Color color, [NotNull] GameObject gameObject)
        {
            if (gameObject == null) return;

            var renderer = gameObject.GetComponent<Renderer>();
            if (renderer == null) return;

            renderer.material.color = color;
        }

        public void ShowArrow()
        {
            if (_node == null || arrow == null || _node.Previous == null) return;

            EnableObject(arrow);

            var direction = (_node.Previous.Position - _node.Position).normalized;
            arrow.transform.rotation = Quaternion.LookRotation(direction);
        }

        private void EnableObject([NotNull] GameObject obj, bool state = true)
        {
            if (obj != null) obj.SetActive(state);
        }
    }
}
