using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Otter {
    public class Quad {

        public List<Vector2> Vertices = new List<Vector2>();

        public Vector2 this[int index] {
            get {
                return Vertices[index];
            }
        }

        public Vector2 TopLeft {
            get { return Vertices[0]; }
            set { Vertices[0] = value; }
        }

        public Vector2 TopRight {
            get { return Vertices[1]; }
            set { Vertices[1] = value; }
        }

        public Vector2 BottomRight {
            get { return Vertices[2]; }
            set { Vertices[2] = value; }
        }

        public Vector2 BottomLeft {
            get { return Vertices[3]; }
            set { Vertices[3] = value; }
        }


        public Quad() {
            Vertices.Add(new Vector2(0, 0));
            Vertices.Add(new Vector2(0, 0));
            Vertices.Add(new Vector2(0, 0));
            Vertices.Add(new Vector2(0, 0));
        }

        public Quad(float x1, float y1, float x2, float y2, float x3, float y3, float x4, float y4) {
            Vertices.Add(new Vector2(x1, y1));
            Vertices.Add(new Vector2(x2, y2));
            Vertices.Add(new Vector2(x3, y3));
            Vertices.Add(new Vector2(x4, y4));
        }
    }
}
