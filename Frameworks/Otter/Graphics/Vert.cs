using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SFML.Graphics;
using SFML.Window;

namespace Otter {
    /// <summary>
    /// Class that represents a Vertex.  Just a wrapper for an SFML Vertex.
    /// </summary>
    public class Vert {

        Vertex vertex;

        public Vert(float x, float y, Color color, float textureX, float textureY) {
            vertex = new SFML.Graphics.Vertex(new Vector2f(x, y), color.SFMLColor, new Vector2f(textureX, textureY));
        }

        public Vert() : this(0, 0, Color.Black, 0, 0) { }
        public Vert(float x, float y) : this(x, y, Color.White, 0, 0) { }
        public Vert(float x, float y, float textureX, float textureY) : this(x, y, Color.Black, textureX, textureY) { }
        public Vert(float x, float y, Color color) : this(x, y, color, 0, 0) { }

        public Color Color {
            get { return new Color(vertex.Color); }
            set { vertex.Color = value.SFMLColor; }
        }

        public float X {
            get { return vertex.Position.X; }
            set { vertex.Position = new Vector2f(value, vertex.Position.Y); }
        }

        public float Y {
            get { return vertex.Position.Y; }
            set { vertex.Position = new Vector2f(vertex.Position.X, value); }
        }

        public Vector2 Position {
            get { return new Vector2(vertex.Position.X, vertex.Position.Y); }
            set { vertex.Position = new Vector2f((float)value.X, (float)value.Y); }
        }

        public Vector2 TexCoords {
            get { return new Vector2(vertex.TexCoords.X, vertex.TexCoords.Y); }
            set { vertex.TexCoords = new Vector2f((float)value.X, (float)value.Y); }
        }

        public float TextureX {
            get { return vertex.TexCoords.X; }
            set { vertex.TexCoords = new Vector2f(value, vertex.TexCoords.Y); }
        }

        public float TextureY {
            get { return vertex.TexCoords.Y; }
            set { vertex.TexCoords = new Vector2f(vertex.TexCoords.X, value); }
        }

        internal Vertex SFMLVertex {
            get { return vertex; }
        }
    }
}
