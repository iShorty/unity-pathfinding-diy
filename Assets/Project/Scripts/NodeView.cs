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

        public void Initialize([NotNull] Node node)
        {
            if (tile == null) return;

            var go = gameObject;
            go.name = $"Node {node.Position}";
            go.transform.position = node.Position;
            tile.transform.localScale = new Vector3(1f - borderSize, 1f, 1f - borderSize);
        }

        public void ColorNode(Color color) => ColorNode(color, tile);

        public void ColorNode(Color color, [NotNull] GameObject gameObject)
        {
            if (gameObject == null) return;

            var renderer = gameObject.GetComponent<Renderer>();
            if (renderer == null) return;

            renderer.material.color = color;
        }
    }
}
