using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Otter {
    /// <summary>
    /// Component that controls a sine wave.  Can be useful for special effects and such.
    /// </summary>
    public class SineWave : Component {

        public float
            Rate,
            Amplitude = 0,
            Offset,
            Min,
            Max;

        public SineWave(float rate = 1, float amp = 1, float offset = 0) {
            Rate = rate;
            Amplitude = amp;
            Offset = offset;
        }

        public SineWave(float rate = 1, float min = -1, float max = 1, float offset = 0) {
            Rate = rate;
            Min = min;
            Max = max;
            Offset = offset;
        }

        public float Value {
            get {
                if (Amplitude == 0) {
                    return Util.SinScaleClamp((Timer + Offset) * Rate, Min, Max);
                }
                else {
                    return Util.Sin((Timer + Offset) * Rate) * Amplitude;
                }
            }
        }

        public static implicit operator float(SineWave s) {
            return s.Value;
        }
        public static implicit operator int(SineWave s) {
            return (int)s.Value;
        }
    }
}
