using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SFML.Graphics;
using SFML.Window;

namespace Otter {
    /// <summary>
    /// Graphic that renders as a simple gradient between 4 points.
    /// </summary>
    public class Gradient : Image {

        VertexArray vertices;

        List<Color> colors = new List<Color>();
        List<Color> baseColors = new List<Color>();
      
        public Gradient(int width, int height, Color TopLeft, Color TopRight, Color BottomRight, Color BottomLeft) {
            baseColors.Add(TopLeft);
            baseColors.Add(TopRight);
            baseColors.Add(BottomRight);
            baseColors.Add(BottomLeft);

            colors.Add(TopLeft);
            colors.Add(TopRight);
            colors.Add(BottomRight);
            colors.Add(BottomLeft);

            Width = width;
            Height = height;

            Shape = new RectangleShape();
            TransformSource = Shape;
            DrawableSource = Shape;

            UpdateGradient();
        }

        public Gradient(Gradient copy) : this(copy.Width, copy.Height, copy.GetColor(ColorPosition.TopLeft), copy.GetColor(ColorPosition.TopRight), copy.GetColor(ColorPosition.BottomRight), copy.GetColor(ColorPosition.BottomLeft)) { }

        public void UpdateGradient() {
            vertices = new VertexArray(PrimitiveType.Quads);

            var finalColors = new List<Color>() {
                Color.None,
                Color.None,
                Color.None,
                Color.None};
            for (int i = 0; i < baseColors.Count; i++) {
                finalColors[i] = new Color(baseColors[i]);
                finalColors[i].A *= alpha;
            }
            
            vertices.Append(new Vertex(new Vector2f(0, 0), finalColors[0].SFMLColor));
            vertices.Append(new Vertex(new Vector2f(Width, 0), finalColors[1].SFMLColor));
            vertices.Append(new Vertex(new Vector2f(Width, Height), finalColors[2].SFMLColor));
            vertices.Append(new Vertex(new Vector2f(0, Height), finalColors[3].SFMLColor));

            DrawableSource = vertices;
        }

        public override void Update() {
            base.Update();
            if (NeedsUpdate) {
                NeedsUpdate = false;
                UpdateGradient();
            }
        }

        public void SetColor(Color color, ColorPosition position) {
            colors[(int)position] = color;
            UpdateGradient();
        }
        
        public Color GetColor(ColorPosition position) {
            return colors[(int)position];
        }

        public enum ColorPosition {
            TopLeft,
            TopRight,
            BottomRight,
            BottomLeft
        }
    }
}
