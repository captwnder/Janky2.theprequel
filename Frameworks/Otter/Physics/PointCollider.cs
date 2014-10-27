using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Otter {
    /// <summary>
    /// Point Collider.
    /// </summary>
    public class PointCollider : Collider {

        public PointCollider(int x, int y, params int[] tags) {
            Width = 0;
            Height = 0;
        }

        public override float Width {
            get {
                return 0;
            }
            set {
            }
        }

        public override float Height {
            get {
                return 0;
            }
            set {
            }
        }
    }
}
