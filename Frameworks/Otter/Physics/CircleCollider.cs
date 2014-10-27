using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Otter {
    /// <summary>
    /// Circle Collider.
    /// </summary>
    public class CircleCollider : Collider {

        public int Radius;

        public override float Width {
            get { return Radius * 2; }
            set { }
        }

        public override float Height {
            get { return Radius * 2; }
            set { }
        }

        public CircleCollider(int radius, params int[] tags) {
            Radius = radius;
            AddTag(tags);
        }

        public override void Render() {
            base.Render();

            Draw.Circle(Left + 1, Top + 1, Radius - 1, Color.None, Color.Red, 1f);
        }
    }
}
