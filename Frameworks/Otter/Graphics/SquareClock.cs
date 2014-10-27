using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SFML.Graphics;
using SFML.Window;

namespace Otter {
    public class SquareClock : Image {

        VertexArray vertices;

        /// <summary>
        /// Determines the fill of the clock.
        /// </summary>
        public float Fill {
            set {
                fill = Util.Clamp(value, 0, 1);
                NeedsUpdate = true;
            }
            get {
                return fill;
            }
        }
        float fill = 1;

        public SquareClock(int size, Color color) {
            Width = size;
            Height = size;

            Color = color;

            NeedsUpdate = true;
            UpdateClock();
        }

        void UpdateClock() {
            if (!NeedsUpdate) return;


            if (fill == 1) {
                //draw box
                vertices = new VertexArray(PrimitiveType.Quads);
                Append(vertices, 0, 0);
                Append(vertices, Width, 0);
                Append(vertices, Width, Height);
                Append(vertices, 0, Height);
            }
            else {

                vertices = new VertexArray(PrimitiveType.TrianglesFan);

                if (fill > 0) {
                    //draw center
                    Append(vertices, HalfWidth, HalfHeight);
                    //draw middle top
                    Append(vertices, HalfWidth, 0);
                    if (fill >= 0.125f) {
                        //draw left top
                        Append(vertices, 0, 0); 
                    }
                    if (fill >= 0.375f) {
                        //draw left bottom
                        Append(vertices, 0, Height);
                    }
                    if (fill >= 0.625f) {
                        //draw right bottom
                        Append(vertices, Width, Height);
                    }
                    if (fill >= 0.875f) {
                        //draw right top
                        Append(vertices, Width, 0);
                    }

                    // get vector of angle
                    var v = new Vector2(Util.PolarX(FillAngle, HalfWidth), Util.PolarY(FillAngle, HalfHeight));
                    // adjust length of vector to meet square
                    var l = (float)Math.Max(Math.Abs(v.X), Math.Abs(v.Y));
                    if (l <= HalfWidth) {
                        v.X /= l;
                        v.Y /= l;
                    }
                    // append the vector
                    Append(vertices, HalfWidth + (float)v.X * HalfWidth, HalfHeight + (float)v.Y * HalfHeight);

                }
            }

            DrawableSource = vertices;

            NeedsUpdate = false;
        }

        public float FillAngle {
            get { return (fill * 360) + 90; }
        }

        void Append(VertexArray v, float x, float y) {
            v.Append(x, y, Color);

        }
        public override void Update() {
            base.Update();
            UpdateClock();
        }

    }
}
