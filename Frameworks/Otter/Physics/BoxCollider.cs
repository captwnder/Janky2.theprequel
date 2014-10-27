using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Otter {
    /// <summary>
    /// Rectangle Collider.
    /// </summary>
    public class BoxCollider : Collider {

        public BoxCollider(int width, int height, params int[] tags) {
            Width = width;
            Height = height;
            AddTag(tags);
        }

        public override void Render() {
            base.Render();

            if (Entity == null) return;

            Draw.Rectangle(Left + 1, Top + 1, Width - 2, Height - 2, Color.None, Color.Red, 1f);
        }
    }
}
