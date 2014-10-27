using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

//thanks to chevy ray for this class
namespace Otter {
    /// <summary>
    /// Class full of random number generation related functions.
    /// </summary>
    public static class Rand {
        static List<Random> randoms = new List<Random>();

        public static void PushSeed(int seed) {
            randoms.Add(new Random(seed));
        }

        static Random random {
            get {
                if (randoms.Count == 0) {
                    randoms.Add(new Random());
                }
                return randoms[randoms.Count - 1];
            }
        }

        public static Random PopSeed() {
            var r = random;
            randoms.RemoveAt(randoms.Count - 1);
            return r;
        }

        public static int Int() {
            return random.Next();
        }
        public static int Int(int max) {
            return random.Next(max);
        }
        public static int Int(int min, int max) {
            return random.Next(min, max);
        }

        public static float Float(float max) {
            return max * Value;
        }
        public static float Float(float min, float max) {
            return min + (max - min) * Value;
        }
        public static float Float(Range range) {
            return range.Min + (range.Max - range.Min) * Value;
        }

        public static Vector2 CircleXY(float radius, float x = 0, float y = 0) {
            var v2 = new Vector2(Rand.Float(-1, 1), Rand.Float(-1, 1));
            v2.Normalize(Rand.Float(radius));
            v2.X += x;
            v2.Y += y;
            return v2;
        }

        public static T Choose<T>(params T[] choices) {
            return choices[Int(choices.Length)];
        }
        public static T ChooseElement<T>(IEnumerable<T> choices) {
            return choices.ElementAt(Int(choices.Count()));
        }
        public static string Choose(string str) {
            return str.Substring(Int(str.Length), 1);
        }
        public static T ChooseRemove<T>(ICollection<T> choices) {
            var choice = ChooseElement(choices);
            choices.Remove(choice);
            return choice;
        }

        public static void Shuffle<T>(T[] list) {
            int i = list.Length;
            int j;
            T item;
            while (--i > 0) {
                item = list[i];
                list[i] = list[j = Int(i + 1)];
                list[j] = item;
            }
        }
        public static void Shuffle<T>(List<T> list) {
            int i = list.Count;
            int j;
            T item;
            while (--i > 0) {
                item = list[i];
                list[i] = list[j = Int(i + 1)];
                list[j] = item;
            }
        }

        public static bool Chance(float percent) {
            return Value < percent * 0.01f;
        }

        public static float Value {
            get { return (float)random.NextDouble(); }
        }

        public static float Angle {
            get { return Float(360); }
        }

        public static Vector2 Direction {
            get { return Util.Normal(Angle); }
        }

        public static bool Bool {
            get { return random.Next(2) > 0; }
        }

        public static Color Color {
            get { return new Color(Float(1), Float(1), Float(1), 1); }
        }

        public static Color ColorAlpha {
            get { return new Color(Float(1), Float(1), Float(1), Float(1)); }
        }

        public static string String(int length, string charSet = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789") {
            var str = "";
            for (var i = 0; i < length; i++) {
                str += charSet[Int(charSet.Length)].ToString();
            }
            return str;
        }

        public static bool Flip() {
            return Float(1) >= 0.5f;
        }
    }
}
