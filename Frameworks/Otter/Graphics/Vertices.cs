using SFML.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Otter {
    public class Vertices : Image {

        public List<Vert> Verts = new List<Vert>();

        public VertexPrimitiveType PrimitiveType = VertexPrimitiveType.Quads;

        int prevCount = -1;

        VertexArray vertices;

        public Vertices(Texture texture, params Vert[] vertices) : this(vertices) {
            Texture = texture;
        }

        public Vertices(params Vert[] vertices) : base() {
            Add(vertices);
            NeedsUpdate = true;
            UpdateVertices();
        }

        public void Clear() {
            Verts.Clear();
            NeedsUpdate = true;
        }

        public void Add(params Vert[] vertices) {
            foreach (var v in vertices) {
                Verts.Add(v);
            }
            NeedsUpdate = true;
        }

        public override void Update() {
            base.Update();

            UpdateVertices();
        }

        void UpdateVertices() {
            if (!NeedsUpdate) return;

            NeedsUpdate = false;

            if (prevCount != Verts.Count) {
                vertices = new VertexArray((SFML.Graphics.PrimitiveType)PrimitiveType, (uint)Verts.Count);
                prevCount = Verts.Count;

                foreach (var v in Verts) {
                    vertices.Append(v);
                }
            }
            else {
                uint i = 0;
                foreach (var v in Verts) {
                    vertices[i] = v.SFMLVertex;

                    i++;
                }
            }

            DrawableSource = vertices;
        }

        public SFML.Graphics.VertexArray SFMLVertexArray {
            get {
                Update(); //update if needed
                return vertices;
            }
        }
       
    }

    public enum VertexPrimitiveType {
        Points = 0,
        Lines = 1,
        LinesStrip = 2,
        Triangles = 3,
        TrianglesStrip = 4,
        TrianglesFan = 5,
        Quads = 6,
    }
}
