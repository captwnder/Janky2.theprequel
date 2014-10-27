using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SFML.Graphics;
using SFML.Window;

namespace Otter {
    /// <summary>
    /// Graphic that renders as a checkerboard type grid that fills the defined area using two alternating
    /// colors.
    /// </summary>
    public class Grid : Image {
        VertexArray vertices = new VertexArray(PrimitiveType.Quads);

        public Color ColorA, ColorB;
        public float GridWidth, GridHeight;

        public Grid(int width, int height, int gridWidth, int gridHeight, Color colorA, Color colorB = null) {
            Width = width;
            Height = height;

            ColorA = colorA;

            if (colorB == null) {
                ColorB = colorA.Copy();
                ColorB.R -= 0.02f;
                ColorB.G -= 0.02f;
                ColorB.B -= 0.02f;
            }
            else {
                ColorB = colorB;
            }

            GridWidth = gridWidth;
            GridHeight = gridHeight;

            UpdateGrid();
        }

        public override void Update() {
            base.Update();

            UpdateGrid();
        }

        void UpdateGrid() {
            if (!NeedsUpdate) return;

            Color nextColor = ColorA;
            Color rowColor = nextColor;
            vertices = new VertexArray(PrimitiveType.Quads);
            for (float j = 0; j < Height; j += GridHeight) {
                for (float i = 0; i < Width; i += GridWidth) {
                    vertices.Append(new Vertex(new Vector2f(i, j), nextColor.SFMLColor));
                    vertices.Append(new Vertex(new Vector2f(i + GridWidth, j), nextColor.SFMLColor));
                    vertices.Append(new Vertex(new Vector2f(i + GridWidth, j + GridHeight), nextColor.SFMLColor));
                    vertices.Append(new Vertex(new Vector2f(i, j + GridHeight), nextColor.SFMLColor));
                    nextColor = nextColor == ColorA ? ColorB : ColorA;
                }
                rowColor = nextColor = rowColor == ColorA ? ColorB : ColorA;        
            }

            NeedsUpdate = false;

            DrawableSource = vertices;
        }
    }
}
