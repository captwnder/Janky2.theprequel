using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Otter {
    /// <summary>
    /// Class used to represent a range using a min and max.
    /// </summary>
    public class Range {
        /// <summary>
        /// The minimum of the range.
        /// </summary>
        public float Min;
        /// <summary>
        /// The maximum of the range.
        /// </summary>
        public float Max;

        public Range(float min, float max) {
            Min = min;
            Max = max;
        }

        public Range(float max) : this(-max, max) { }
    }
}
