using Otter;
using SFML.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Otter {
    public class BetterImage : Image {

        VertexArray vertices;

        public BetterImage(Color color, int width, int height) {
            Color = color;
            Width = width;
            Height = height;

            Init();
        }

        public BetterImage(int width, int height) {
            Color = Color.White;
            Width = width;
            Height = height;

            Init();
        }

        public BetterImage(Texture source) {
            if (source != null) {
                Texture = source;
            }

            Width = source.Width;
            Height = source.Height;

            Init();
        }

        void Init() {
            NeedsUpdate = true;
            UpdateVertices();
        }

        public void UpdateVertices() {
            if (!NeedsUpdate) return;

            vertices = new VertexArray(PrimitiveType.Quads);

            vertices.Append(0, 0, color, atlasRect.Left, atlasRect.Top);
            vertices.Append(Width, 0, color, atlasRect.Left + Width, atlasRect.Top);
            vertices.Append(Width, Height, color, atlasRect.Left + Width, atlasRect.Top + Height);
            vertices.Append(0, Height, color, atlasRect.Left, atlasRect.Top + Height);

            DrawableSource = vertices;

            NeedsUpdate = false;
        }
    }
}
