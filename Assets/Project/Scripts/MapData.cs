using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using JetBrains.Annotations;
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
                    if (color == Color.black)
                    {
                        sb.Append('1');
                    }
                    else if (color == Color.white)
                    {
                        sb.Append('0');
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
    }
}