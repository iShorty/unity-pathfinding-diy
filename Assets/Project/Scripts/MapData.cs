using System;
using System.Collections.Generic;
using System.ComponentModel;
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
        private string resourcePath = "MapData";

        [NotNull]
        public int[,] MakeMap()
        {
            var lines = GetTextFromFile();
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
        public List<string> GetTextFromFile()
        {
            if (textAsset == null)
            {
                var sceneName = SceneManager.GetActiveScene().name;
                textAsset = Resources.Load<TextAsset>($"{resourcePath}/{sceneName}");
            }
            return GetTextFromFile(textAsset);
        }

        [NotNull]
        public List<string> GetTextFromFile([NotNull] TextAsset textAsset)
        {
            var lines = new List<string>();
            if (textAsset == null) return lines;

            lines.AddRange(textAsset.text.Split(LineBreakDelimiters, StringSplitOptions.None));
            lines.Reverse();
            return lines;
        }
    }
}