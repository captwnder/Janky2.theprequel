using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Otter {
    /// <summary>
    /// Counter in which the value can be moved in both an X and Y direction.  Probably most useful
    /// for making menus that are grids which the player can move around in.
    /// </summary>
    public class GridCounter : Component {

        public int
            Width,
            Height;

        int x, y;

        public bool
            WrapX,
            WrapY;

        public GridCounter(int value, int width = 1, int height = 1, bool wrapX = false, bool wrapY = false) {
            if (width < 1) {
                throw new ArgumentException("Width must be at least 1!");
            }
            if (height < 1) {
                throw new ArgumentException("Height must be at least 1!");
            }

            Width = width;
            Height = height;
            WrapX = wrapX;
            WrapY = wrapY;

            X = Util.TwoDeeX(value, width);
            Y = Util.TwoDeeY(value, width);
        }

        public int Index {
            get { return Util.OneDee(Width, X, Y); }
            set {
                X = Util.TwoDeeX(value, Width);
                Y = Util.TwoDeeY(value, Width);
            }
        }

        public bool Wrap {
            set { WrapX = value; WrapY = value; }
        }

        public int Count {
            get { return Width * Height; }
        }

        public int X {
            set {
                if (WrapX) {
                    x = value;
                    while (x < 0) {
                        x += Width;
                    }
                    while (x > Width - 1) {
                        x -= Width;
                    }
                }
                else {
                    x = (int)Util.Clamp(value, Width - 1);
                }
            }
            get { return x; }
        }

        public int Y {
            set {
                if (WrapY) {
                    y = value;
                    while (y < 0) {
                        y += Height;
                    }
                    while (y > Height - 1) {
                        y -= Height;
                    }
                }
                else {
                    y = (int)Util.Clamp(value, Height - 1);
                }
            }
            get { return y; }
        }

        public static implicit operator float(GridCounter counter) {
            return counter.Index;
        }
        public static implicit operator int(GridCounter counter) {
            return (int)counter.Index;
        }
    }
}
