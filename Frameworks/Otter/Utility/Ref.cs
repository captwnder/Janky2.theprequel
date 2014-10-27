using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

//special thanks to chevy ray for this class <3

namespace Otter {
    /// <summary>
    /// Class of utility functions for ref related things.
    /// </summary>
    public static class Ref {
        public static void Swap<T>(ref T a, ref T b) {
            var temp = a;
            a = b;
            b = temp;
        }

        public static void Shift<T>(ref T a, ref T b, ref T c) {
            var temp = a;
            a = b;
            b = c;
            c = temp;
        }
        public static void Shift<T>(ref T a, ref T b, ref T c, ref T d) {
            var temp = a;
            a = b;
            b = c;
            c = d;
            d = temp;
        }
        public static void Shift<T>(ref T a, ref T b, ref T c, ref T d, ref T e) {
            var temp = a;
            a = b;
            b = c;
            c = d;
            d = e;
            e = temp;
        }
        public static void Shift<T>(ref T a, ref T b, ref T c, ref T d, ref T e, ref T f) {
            var temp = a;
            a = b;
            b = c;
            c = d;
            d = e;
            e = f;
            f = temp;
        }

        public static bool EqualsAny<T>(ref T p, params T[] values) {
            foreach (var val in values)
                if (val.Equals(p))
                    return true;
            return false;
        }

        public static bool EqualsAll<T>(ref T p, params T[] values) {
            foreach (var val in values)
                if (!val.Equals(p))
                    return false;
            return true;
        }
    }
}
