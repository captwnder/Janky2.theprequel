using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Otter {
    /// <summary>
    /// Class used for tracking an X and Y speed of an object. Speed also has an XMax and YMax that can
    /// be used to clamp the X and Y values automatically.
    /// </summary>
    public class Speed {
        public float XMax, YMax;
        float x, y;

        public bool HardClamp;

        public Speed(float x, float y, float xMax, float yMax, bool hardClamp) {
            this.x = x;
            this.y = y;
            XMax = xMax;
            YMax = yMax;
            HardClamp = hardClamp;
        }

        public Speed(float x, float y, float xMax, float yMax) : this(x, y, xMax, yMax, true) { }
        public Speed(float xMax, float yMax) : this(0, 0, xMax, yMax, true) { }
        public Speed(float xMax, float yMax, bool hardClamp) : this(0, 0, xMax, yMax, hardClamp) { }
        public Speed(float max, bool hardClamp) : this(0, 0, max, max, hardClamp) { }
        public Speed(float max) : this(0, 0, max, max, true) { }

        public float X {
            get {
                if (HardClamp) {
                    return Util.Clamp(x, -XMax, XMax);
                }
                else {
                    return x;
                }
            }
            set {
                x = value;
            }
        }

        public float Y {
            get {
                if (HardClamp) {
                    return Util.Clamp(y, -YMax, YMax);
                }
                else {
                    return y;
                }
            }
            set {
                y = value;
            }
        }

        public float Max {
            set {
                XMax = value; YMax = value;
            }
        }

        public override string ToString() {
            return "X: " + X + " Y: " + Y;
        }

        public float Length {
            get {
                return (float)Math.Sqrt(X * X + Y * Y);
            }
        }
    }
}
