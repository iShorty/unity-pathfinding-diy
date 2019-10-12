using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JetBrains.Annotations;
using Project.Scripts.Project.Scripts;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Project.Scripts
{
    public class MapData : MonoBehaviour
    {
        [NotNull]
        private static readonly string[] LineBreakDelimiters = { "\r\n", "\n" };

        [SerializeField]
        private int width = 10;

        [SerializeField]
        private int height = 10;

        [SerializeField]
        private TextAsset textAsset;

        [SerializeField]
        private Texture2D textureMap;

        [SerializeField]
        private string resourcePath = "MapData";

        [SerializeField]
        private Color32 openColor = Color.white;

        [SerializeField]
        private Color32 blockedColor = Color.black;

        [SerializeField]
        private Color32 lightTerrainColor = new Color32(124, 194, 78, 255);

        [SerializeField]
        private Color32 mediumTerrainColor = new Color32(252, 255, 52, 255);

        [SerializeField]
        private Color32 heavyTerrainColor = new Color32(255, 129, 12, 255);

        private readonly Dictionary<Color, NodeType> _terrainLookupTable = new Dictionary<Color, NodeType>();

        private void Awake()
        {
            var sceneName = SceneManager.GetActiveScene().name;

            if (textureMap == null)
            {
                // TODO: This break if the file is a text file instead.
                textureMap = Resources.Load<Texture2D>($"{resourcePath}/{sceneName}");
            }

            if (textAsset == null)
            {
                // TODO: This break if the file is a texture instead.
                textAsset = Resources.Load<TextAsset>($"{resourcePath}/{sceneName}");
            }

            SetupLookupTable();
        }

        [NotNull]
        public int[,] MakeMap()
        {
            var lines = textureMap != null
                ? GetMapFromTexture(textureMap)
                : GetMapFromTextFile(textAsset);

            SetDimensions(lines);

            var map = new int[width, height];
            for (var yIndex = 0; yIndex < height; ++yIndex)
            {
                var line = lines[yIndex];
                for (var xIndex = 0; xIndex < width; ++xIndex)
                {
                    // Fill shorter lines with zeros
                    if (line.Length < width) continue;
                    map[xIndex, yIndex] = line[xIndex] - '0';
                }
            }

            return map;
        }

        public void SetDimensions([NotNull] List<string> textLines)
        {
            (height, width) = (textLines.Count, 0);
            foreach (var line in textLines)
            {
                width = Mathf.Max(width, line.Length);
            }
        }

        [NotNull]
        public List<string> GetMapFromTextFile() => GetMapFromTextFile(textAsset);

        [NotNull]
        public List<string> GetMapFromTextFile([NotNull] TextAsset textAsset)
        {
            var lines = new List<string>();
            if (textAsset == null) return lines;

            lines.AddRange(textAsset.text.Split(LineBreakDelimiters, StringSplitOptions.None));
            lines.Reverse();
            return lines;
        }

        [NotNull]
        public List<string> GetMapFromTexture() => GetMapFromTexture(textureMap);

        [NotNull]
        public List<string> GetMapFromTexture([NotNull] Texture2D texture)
        {
            var lines = new List<string>();
            if (texture == null) return lines;

            var sb = new StringBuilder();
            for (var yIndex = 0; yIndex < texture.height; ++yIndex)
            {
                sb.Clear();
                for (var xIndex = 0; xIndex < texture.width; ++xIndex)
                {
                    var color = texture.GetPixel(xIndex, yIndex);
                    if (_terrainLookupTable.ContainsKey(color))
                    {
                        var nodeType = _terrainLookupTable[color];
                        sb.Append((int)nodeType);
                    }
                    else
                    {
                        sb.Append(' ');
                    }
                }

                lines.Add(sb.ToString());
            }

            return lines;
        }

        public Color GetColorFromNodeType([NotNull] Node node) => GetColorFromNodeType(node.Type);

        public Color GetColorFromNodeType(NodeType type)
        {
            // TODO: Can we get rid of ContainsValue since we also do FirstOrDefault?
            if (!_terrainLookupTable.ContainsValue(type)) return Color.white;
            var color = _terrainLookupTable.FirstOrDefault(x => x.Value == type);
            return color.Key;
        }

        private void SetupLookupTable()
        {
            _terrainLookupTable.Clear();
            _terrainLookupTable.Add(openColor, NodeType.Open);
            _terrainLookupTable.Add(blockedColor, NodeType.Blocked);
            _terrainLookupTable.Add(lightTerrainColor, NodeType.LightTerrain);
            _terrainLookupTable.Add(mediumTerrainColor, NodeType.MediumTerrain);
            _terrainLookupTable.Add(heavyTerrainColor, NodeType.HeavyTerrain);
        }
    }
}