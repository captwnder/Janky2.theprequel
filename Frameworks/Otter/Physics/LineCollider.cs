using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Otter {
    /// <summary>
    /// Line Collider.
    /// </summary>
    public class LineCollider : Collider {
        public float X2, Y2;

        public LineCollider(float x1, float y1, float x2, float y2) {
            X = x1;
            Y = y1;
            X2 = x2;
            Y2 = y2;
        }

        public override float Width {
            get { return Math.Abs(X - X2); }
            set { }
        }

        public override float Height {
            get { return Math.Abs(Y - Y2); }
            set { }
        }

        public override float Bottom {
            get { return Math.Max(Y, Y2) + OriginY + Entity.Y; }
        }

        public override float Top {
            get { return Math.Min(Y, Y2) + OriginY + Entity.Y; }
        }

        public override float Left {
            get { return Math.Min(X, X2) + OriginX + Entity.X; }
        }

        public override float Right {
            get { return Math.Max(X, X2) + OriginX + Entity.X; }
        }

        public Line2 Line2 {
            get { return new Line2(X + OriginX + Entity.X, Y + OriginY + Entity.Y, X2 + OriginX + Entity.X, Y2 + OriginY + Entity.Y); }
            private set { }
        }
    }
}
