using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TowerOfInfinity.CharacterBuilder
{
    public static class TextureProcessor
    {
        public static Texture2D MergeLayers(Texture2D texture, params Color32[][] layers)
        {
            if (layers.Length == 0) throw new Exception("No layers to merge.");

            var result = new Color[texture.width * texture.height];

            foreach (var layer in layers.Where(i => i != null))
            {
                if (layer.Length != result.Length) Debug.LogWarning("Invalid layer size.");

                for (var i = 0; i < result.Length; i++)
                {
                    if (layer[i].a > 0) result[i] = layer[i];
                }
            }

            texture.SetPixels(result);
            texture.Apply();

            return texture;
        }

        public static Color32[] Repaint3C(Color32[] pixels, Color32 paint, List<Color32> palette)
        {
            var dict = new Dictionary<Color32, int>();

            for (var i = 0; i < pixels.Length; i++)
            {
                var c = pixels[i];

                if (c.a > 0 && c != Color.white && c != Color.black)
                {
                    if (dict.ContainsKey(c))
                    {
                        dict[c]++;
                    }
                    else
                    {
                        dict.Add(c, 1);
                    }
                }
            }

            var colors = dict.Count > 3 ? dict.OrderByDescending(i => i.Value).Take(3).Select(i => i.Key).ToList() : dict.Keys.ToList();

            float GetBrightness(Color32 color)
            {
                Color.RGBToHSV(color, out _, out _, out var result);
                return result;
            }

            colors = colors.OrderBy(GetBrightness).ToList();

            if (colors.Count != 2 && colors.Count != 3)
            {
                Debug.LogWarning("Sprite should have 2 or 3 colors only (+black outline).");
                return pixels;
            }

            var replacement = palette.GetRange(palette.IndexOf(paint) - 1, 3).OrderBy(i => ((Color)i).grayscale).ToList();
            var match = new Dictionary<Color32, Color32>
            {
                { colors[0], replacement[0] },
                { colors[1], replacement[1] }
            };

            if (colors.Count == 3)
            {
                match.Add(colors[2], replacement[2]);
            }

            for (var i = 0; i < pixels.Length; i++)
            {
                if (pixels[i].a > 0 && pixels[i] != Color.black && match.ContainsKey(pixels[i]))
                {
                    pixels[i] = match[pixels[i]];
                }
            }

            return pixels;
        }

        public static Color AdjustColor(Color color, float hue, float saturation, float value)
        {
            hue /= 180f;
            saturation /= 100f;
            value /= 100f;

            var a = color.a;

            Color.RGBToHSV(color, out var h, out var s, out var v);

            h += hue / 2f;

            if (h > 1) h -= 1;
            else if (h < 0) h += 1;

            color = Color.HSVToRGB(h, s, v);

            var grey = 0.3f * color.r + 0.59f * color.g + 0.11f * color.b;

            color.r = grey + (color.r - grey) * (saturation + 1);
            color.g = grey + (color.g - grey) * (saturation + 1);
            color.b = grey + (color.b - grey) * (saturation + 1);

            if (color.r < 0) color.r = 0;
            if (color.g < 0) color.g = 0;
            if (color.b < 0) color.b = 0;

            color.r += value * color.r;
            color.g += value * color.g;
            color.b += value * color.b;
            color.a = a;

            return color;
        }

        public static Color32[] ApplyPalette(Color32[] pixels, List<Color32> palette)
        {
            var unique = new ColorDistinctor(pixels).UniqueColors.OrderByDescending(i => pixels.Count(j => FastEquals(i, j))).ToList();
            var map = new Dictionary<Color32, Color32>();
            var mapInvert = new Dictionary<Color32, Color32>();

            foreach (var color in unique)
            {
                if (color.a == 0) continue;

                var nearest = FindNearest(color, palette);
                var used = mapInvert.ContainsKey(nearest);

                if (used)
                {
                    var match = mapInvert[nearest];
                    var dist = GetEuclidean(color, match);

                    if (dist > 2500)
                    {
                        Debug.Log($"Bad mapping (glued pixels) found: {color} / {match} dist={dist}.");

                        var alternative = FindNearest(color, palette, palette.IndexOf(nearest));

                        used = mapInvert.ContainsKey(alternative);

                        if (used)
                        {
                            Debug.LogWarning("Bad mapping not fixed.");
                        }
                        else
                        {
                            nearest = alternative;
                            Debug.Log("Bad mapping fixed.");
                        }
                    }
                }
                else
                {
                    mapInvert.Add(nearest, color);
                }

                map.Add(color, nearest);
            }

            for (var i = 0; i < pixels.Length; i++)
            {
                if (pixels[i].a < 255) 
                {
                    pixels[i] = new Color32();
                }
                else
                {
                    var color = map[pixels[i]];

                    color.a = pixels[i].a;
                    pixels[i] = color;
                }
            }

            return pixels;
        }

        public static bool FastEquals(this Color32 a, Color32 b)
        {
            return a.r == b.r && a.g == b.g && a.b == b.b && a.a == b.a;
        }

        public static float GetEuclidean(Color32 a, Color32 b, bool sqrt = false)
        {
            var dr = a.r - b.r;
            var dg = a.g - b.g;
            var db = a.b - b.b;
            var redmean = (a.r + b.r) / 2f;
            var difference2 = redmean < 128f
                ? 2f * dr * dr + 4f * dg * dg + 3f * db * db
                : 3f * dr * dr + 4f * dg * dg + 2f * db * db;

            return sqrt ? Mathf.Sqrt(difference2) : difference2;
        }

        private static Color32 FindNearest(Color32 color, List<Color32> palette, int ignore = -1)
        {
            var nearest = palette[ignore == 0 ? 1 : 0];
            var difference = GetEuclidean(color, nearest);

            for (var j = 1; j < palette.Count; j++)
            {
                if (j == ignore) continue;

                var d = GetEuclidean(color, palette[j]);

                if (d >= difference) continue;

                difference = d;
                nearest = palette[j];
            }

            return nearest;
        }
    }

    public class ColorDistinctor
    {
        public List<Color32> UniqueColors { get; } = new List<Color32>();

        private readonly bool[,,] _matrix = new bool[256, 256, 256];

        public ColorDistinctor()
        {
        }

        public ColorDistinctor(IEnumerable<Color32> colors)
        {
            AddColors(colors);
        }

        public void AddColors(IEnumerable<Color32> colors, bool alpha = false)
        {
            if (alpha) throw new NotImplementedException();

            var transparent = false;

            foreach (var color in colors)
            {
                if (color.a == 0)
                {
                    transparent = true;
                }
                else if (!_matrix[color.r, color.g, color.b])
                {
                    _matrix[color.r, color.g, color.b] = true;
                    UniqueColors.Add(new Color32(color.r, color.g, color.b, 255));
                }
            }

            if (transparent && UniqueColors.Count > 0 && UniqueColors[0].a != 0)
            {
                UniqueColors.Insert(0, new Color32());
            }
        }
    }
}