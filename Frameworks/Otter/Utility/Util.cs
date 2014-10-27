using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ionic.Zip;
using System.IO;
using System.Xml;
using System.Reflection;
using System.IO.Compression;
using System.Text.RegularExpressions;
using System.Security.Cryptography;
using System.Net;

namespace Otter {
    /// <summary>
    /// Main utility function class. Various useful functions for 2d game development.
    /// </summary>
    public class Util {

        public const float RIGHT = 0;
        public const float UP = (float)Math.PI * -.5f;
        public const float LEFT = (float)Math.PI;
        public const float DOWN = (float)Math.PI * .5f;
        public const float UP_RIGHT = (float)Math.PI * -.25f;
        public const float UP_LEFT = (float)Math.PI * -.75f;
        public const float DOWN_RIGHT = (float)Math.PI * .25f;
        public const float DOWN_LEFT = (float)Math.PI * .75f;
        public const float DEG_TO_RAD = (float)Math.PI / 180f;
        public const float RAD_TO_DEG = 180f / (float)Math.PI;
        private const string HEX = "0123456789ABCDEF";

        static Random rand = new Random();

        /// <summary>
        /// A shortcut function to send text to the debugger log.
        /// </summary>
        /// <param name="str">The string to send.</param>
        public static void Log(object str) {
            if (Debugger.Instance == null) return;
            Debugger.Instance.Log(str);
        }

        public static void ShowDebugger() {
            if (Debugger.Instance == null) return;
            Debugger.Instance.Summon();
        }

        /// <summary>
        /// A shortcut function to register a command with the debugger.
        /// </summary>
        /// <param name="func">The function to register.</param>
        /// <param name="types">The list of types for the arguments.</param>
        public static void RegisterCommand(Debugger.CommandFunction func, params CommandType[] types) {
            if (Debugger.Instance == null) return;
            Debugger.Instance.RegisterCommand(func, types);
        }

        /// <summary>
        /// A shortcut function to register a command with the debugger.
        /// </summary>
        /// <param name="func">The function to register.</param>
        /// <param name="help">The help text that will appear with the function.</param>
        /// <param name="types">The list of types for the arguments.</param>
        public static void RegisterCommand(Debugger.CommandFunction func, string help, params CommandType[] types) {
            if (Debugger.Instance == null) return;
            Debugger.Instance.RegisterCommand(func, help, types);
        }

        /// <summary>
        /// A shortcut function to watch a value in the debugger. This must be called every update to
        /// keep the value updated (not an automatic watch.)
        /// </summary>
        /// <param name="str">The name of the value.</param>
        /// <param name="obj">The value.</param>
        public static void Watch(string str, object obj) {
            if (Debugger.Instance == null) return;
            Debugger.Instance.Watch(str, obj);
        }

        /// <summary>
        /// Produces a random number.
        /// </summary>
        /// <returns></returns>
        public static float Random() {
            return (float)rand.NextDouble();
        }

        /// <summary>
        /// Returns a number inside a range.
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static float RandomRange(float min, float max) {
            if (min == max) return min;
            if (min > max) {
                float temp = max;
                min = max;
                max = min;
            }
            return min + (float)rand.NextDouble() * (max - min);
        }

        /// <summary>
        /// Returns a number inside a range.
        /// </summary>
        /// <param name="max"></param>
        /// <returns></returns>
        public static float RandomRange(float max) {
            return RandomRange(0, max);
        }

        /// <summary>
        /// Returns a vector of two random numbers in a range.
        /// </summary>
        /// <param name="xMax"></param>
        /// <param name="yMax"></param>
        /// <returns></returns>
        public static Vector2 RandomXY(float xMax, float yMax) {
            return new Vector2(RandomRange(0, xMax), RandomRange(0, yMax));
        }

        /// <summary>
        /// Returns a vector of two random numbers in a range.
        /// </summary>
        /// <param name="xMin"></param>
        /// <param name="xMax"></param>
        /// <param name="yMin"></param>
        /// <param name="yMax"></param>
        /// <returns></returns>
        public static Vector2 RandomXY(float xMin, float xMax, float yMin, float yMax) {
            return new Vector2(RandomRange(xMin, xMax), RandomRange(yMin, yMax));
        }

        /// <summary>
        /// Returns a random int in a range.
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static int RandomRangeInt(int min, int max) {
            return rand.Next(min, max + 1);
        }

        /// <summary>
        /// Returns a random int in a range.
        /// </summary>
        /// <param name="max"></param>
        /// <returns></returns>
        public static int RandomRangeInt(int max) {
            return RandomRangeInt(0, max);
        }

        public static float Lerp(float a, float b, float t = 1) {
			return a + (b - a) * t;
		}

        public static Color LerpColor(Color from, Color to, float amount) {
            if (amount <= 0) return new Color(from);
			if (amount >= 1) return new Color(to);

            var c = new Color(from);
            c.R = from.R + (to.R - from.R) * amount;
            c.G = from.G + (to.G - from.G) * amount;
            c.B = from.B + (to.B - from.B) * amount;
            c.A = from.A + (to.A - from.A) * amount;

            return c;
        }

        public static Color LerpColor(float amount, params Color[] colors) {
            if (amount <= 0) return colors[colors.Length - 1];
            if (amount >= 1) return colors[0];

            int fromIndex = (int)Util.ScaleClamp(amount, 0, 1, 0, colors.Length - 1);
            int toIndex = fromIndex + 1;

            float length = 1f / (colors.Length - 1);
            float lerp = Util.ScaleClamp(amount % length, 0, length, 0, 1);

            Console.WriteLine("Lerp {0} to {1} at {2}", colors[fromIndex], colors[toIndex], lerp);
            return LerpColor(colors[fromIndex], colors[toIndex], lerp);
        }

        /// <summary>
        /// Produces a random Color.
        /// </summary>
        /// <returns>A random color.</returns>
        public static Color RandomColor() {
            return new Color(Random(), Random(), Random());
        }

        /// <summary>
        /// Produces a random Color with random alpha.
        /// </summary>
        /// <returns>A random color with alpha.</returns>
        public static Color RandomColorAlpha() {
            return new Color(Random(), Random(), Random(), Random());
        }

        /// <summary>
        /// A random chance of true or false with controlled odds.
        /// </summary>
        /// <param name="percent">How likely to return true.</param>
        /// <returns>True or false based on the random percent.</returns>
        public static bool RandomChance(float percent) {
            return percent >= RandomRange(100);
        }

        /// <summary>
        /// Clamps a value inside a range.
        /// </summary>
        /// <param name="value">The value to clamp.</param>
        /// <param name="min">Min clamp.</param>
        /// <param name="max">Max clamp.</param>
        /// <returns>The new value between min and max.</returns>
        public static float Clamp(float value, float min, float max) {
            if (value < min) return min;
            if (value > max) return max;
            return value;
        }

        /// <summary>
        /// Clamps a value inside a range.
        /// </summary>
        /// <param name="value">The value to clamp.</param>
        /// <param name="max">Max clamp</param>
        /// <returns>The new value between 0 and max.</returns>
        public static float Clamp(float value, float max) {
            return Clamp(value, 0, max);
        }

        /// <summary>
        /// Clamps a value inside a range.
        /// </summary>
        /// <param name="value">The value to clamp.</param>
        /// <param name="range">The range.</param>
        /// <returns>The clamped value in the range.</returns>
        public static float Clamp(float value, Range range) {
            return Clamp(value, range.Min, range.Max);
        }

        /// <summary>
        /// Steps a value toward a target based on a certain amount.
        /// </summary>
        /// <param name="val">The value to step.</param>
        /// <param name="target">The target to approach.</param>
        /// <param name="maxMove">The maximum increment toward the target.</param>
        /// <returns>The new value approaching the target.</returns>
        static public float Approach(float val, float target, float maxMove) {
            return val > target ? Math.Max(val - maxMove, target) : Math.Min(val + maxMove, target);
        }

        static public float SnapToGrid(float value, float increment, float offset = 0) {
            return ((float)Math.Floor(value / increment) * increment) + offset;
        }

        static public byte HexToByte(char c) {
            return (byte)HEX.IndexOf(char.ToUpper(c));
        }

        static public float Min(params float[] values) {
            float min = values[0];
            for (int i = 1; i < values.Length; i++)
                min = Math.Min(values[i], min);
            return min;
        }

        static public float Max(params float[] values) {
            float max = values[0];
            for (int i = 1; i < values.Length; i++)
                max = Math.Max(values[i], max);
            return max;
        }

        public static float Scale(float value, float min, float max, float min2, float max2) {
            return min2 + ((value - min) / (max - min)) * (max2 - min2);
        }

        public static float ScaleClamp(float value, float min, float max, float min2, float max2) {
            value = min2 + ((value - min) / (max - min)) * (max2 - min2);
            if (max2 > min2) {
                value = value < max2 ? value : max2;
                return value > min2 ? value : min2;
            }
            value = value < min2 ? value : min2;
            return value > max2 ? value : max2;
        }

        public static float SinScale(float value, float min, float max) {
            return Scale((float)Math.Sin(value * DEG_TO_RAD), -1f, 1f, min, max);
        }

        public static float SinScaleClamp(float value, float min, float max) {
            return ScaleClamp((float)Math.Sin(value * DEG_TO_RAD), -1f, 1f, min, max);
        }

        public static float Floor(float x) {
            return (float)Math.Floor(x);
        }

        public static float Ceil(float x) {
            return (float)Math.Ceiling(x);
        }

        public static float Round(float x) {
            return (float)Math.Round(x);
        }

        /// <summary>
        /// The angle of a x and y coordinate.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static float Angle(float x, float y) {
            //y is negative since y is DOWN in video games
            //return degrees by default
            return (float)Math.Atan2(-y, x) * Util.RAD_TO_DEG;
        }

        /// <summary>
        /// The angle of a vector.
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public static float Angle(Vector2 vector) {
            return Angle((float)vector.X, (float)vector.Y);
        }

        public static float Angle(Vector2 from, Vector2 to) {
            return Angle((float)(to.X - from.X), (float)(to.Y - from.Y));
        }

        public static float Angle(float x1, float y1, float x2, float y2) {
            return Angle(x2 - x1, y2 - y1);
        }

        public static Vector2 Rotate(Vector2 vector, float amount) {
            var v = new Vector2(vector.X, vector.Y);
            var length = vector.Length;
            var angle = Math.Atan2(vector.Y, vector.X) + amount;
            v.X = Math.Cos(angle) * length;
            v.Y = Math.Sin(angle) * length;
            return v;
        }

        /// <summary>
        /// Distance check.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="x2"></param>
        /// <param name="y2"></param>
        /// <returns></returns>
        public static float Distance(float x, float y, float x2, float y2) {
            return (float)Math.Sqrt((x2 - x) * (x2 - x) + (y2 - y) * (y2 - y));
        }

        public static float Distance(Entity e1, Entity e2) {
            return Distance(e1.X, e1.Y, e2.X, e2.Y);
        }

        /// <summary>
        /// Check for a point in a rectangle defined by min and max points.
        /// </summary>
        /// <param name="p"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static bool InRect(Vector2 p, Vector2 min, Vector2 max) {
            return p.X > min.X && p.Y > min.Y && p.X < max.X && p.Y < max.Y;
        }

        /// <summary>
        /// Check for a point inside of a circle.
        /// </summary>
        /// <param name="p"></param>
        /// <param name="circleP"></param>
        /// <param name="radius"></param>
        /// <returns></returns>
        public static bool InCircle(Vector2 p, Vector2 circleP, float radius) {
            return Distance((float)p.X, (float)p.Y, (float)circleP.X, (float)circleP.Y) <= radius;
        }

        /// <summary>
        /// Check for a point inside of a circle.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="circleX"></param>
        /// <param name="circleY"></param>
        /// <param name="radius"></param>
        /// <returns></returns>
        public static bool InCircle(float x, float y, float circleX, float circleY, float radius) {
            return Distance(x, y, circleX, circleY) <= radius;
        }

        /// <summary>
        /// Check an intersection between two rectangles.
        /// </summary>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="w1"></param>
        /// <param name="h1"></param>
        /// <param name="x2"></param>
        /// <param name="y2"></param>
        /// <param name="w2"></param>
        /// <param name="h2"></param>
        /// <returns></returns>
        public static bool IntersectRectangles(float x1, float y1, float w1, float h1, float x2, float y2, float w2, float h2) {
            if (x1 + w1 <= x2) return false;
            if (x1 >= x2 + w2) return false;
            if (y1 + h1 <= y2) return false;
            if (y1 >= y2 + h2) return false;
            return true;
        }

        /// <summary>
        /// The x component of a vector represented by an angle and radius.
        /// </summary>
        /// <param name="angle"></param>
        /// <param name="radius"></param>
        /// <returns></returns>
        public static float PolarX(float angle, float radius) {
            return Cos(angle) * radius;
        }

        /// <summary>
        /// The y component of a vector represented by an angle and radius.
        /// </summary>
        /// <param name="angle"></param>
        /// <param name="radius"></param>
        /// <returns></returns>
        public static float PolarY(float angle, float radius) {
            //Radius is negative since Y positive is DOWN in video games.
            return Sin(angle) * -radius;
        }

        /// <summary>
        /// Wrapper for the sin function that uses degrees.
        /// </summary>
        /// <param name="degrees"></param>
        /// <returns></returns>
        public static float Sin(float degrees) {
            return (float)Math.Sin(degrees * DEG_TO_RAD);
        }

        /// <summary>
        /// Wrapper for the cos function that uses degrees.
        /// </summary>
        /// <param name="degrees"></param>
        /// <returns></returns>
        public static float Cos(float degrees) {
            return (float)Math.Cos(degrees * DEG_TO_RAD);
        }

        /// <summary>
        /// Convert a two dimensional position to a one dimensional index.
        /// </summary>
        /// <param name="width"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static int OneDee(int width, int x, int y) {
            return y * width + x;
        }

        /// <summary>
        /// Convert a one dimensional index to a two dimensional position.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="width"></param>
        /// <returns></returns>
        public static Vector2 TwoDee(int index, int width) {
            if (width == 0) {
                return new Vector2(index, 0);
            }
            return new Vector2(index % width, index / width);
        }

        /// <summary>
        /// The x position from converting an index to a two dimensional position.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="width"></param>
        /// <returns></returns>
        public static int TwoDeeX(int index, int width) {
            return (int)TwoDee(index, width).X;
        }

        /// <summary>
        /// The y position from converting an index to a two dimensional position.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="width"></param>
        /// <returns></returns>
        public static int TwoDeeY(int index, int width) {
            return (int)TwoDee(index, width).Y;
        }

        /// <summary>
        /// Normal vector.
        /// </summary>
        /// <param name="angle"></param>
        /// <returns></returns>
        public static Vector2 Normal(float angle) {
            return new Vector2(Cos(angle), Sin(angle));
        }

        /// <summary>
        /// Checks if an object contains a method.
        /// </summary>
        /// <param name="objectToCheck"></param>
        /// <param name="methodName"></param>
        /// <returns></returns>
        public static bool HasMethod(object objectToCheck, string methodName) {
            var type = objectToCheck.GetType();
            return type.GetMethod(methodName) != null || type.BaseType.GetMethod(methodName) != null;
        }

        /// <summary>
        /// Checks if an object contains a property.
        /// </summary>
        /// <param name="objectToCheck"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public static bool HasProperty(object objectToCheck, string propertyName) {
            var type = objectToCheck.GetType();
            return type.GetProperty(propertyName) != null || type.BaseType.GetProperty(propertyName) != null;
        }

        /// <summary>
        /// Get the value of a property from an object.
        /// </summary>
        /// <param name="src"></param>
        /// <param name="propName"></param>
        /// <returns></returns>
        public static object GetPropValue(object src, string propName) {
            return src.GetType().GetProperty(propName).GetValue(src, null);
        }

        /// <summary>
        /// Checks to see if an object has a field by name.
        /// </summary>
        /// <param name="src"></param>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public static bool HasField(object src, string fieldName) {
            return src.GetType().GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public) != null;
        }

        /// <summary>
        /// Get the value of a field by name from an object.
        /// </summary>
        /// <param name="src"></param>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public static object GetFieldValue(object src, string fieldName) {
            var fi = src.GetType().GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
            return fi.GetValue(src);
        }

        public static object GetFieldValue(object src, string fieldName, object returnOnNull) {
            if (src == null) return returnOnNull;
            if (!Util.HasField(src, fieldName)) return returnOnNull;
            return GetFieldValue(src, fieldName);
        }

        /// <summary>
        /// Set the value of a field on an object by name.
        /// </summary>
        /// <param name="src">The object.</param>
        /// <param name="fieldName">The name of the field.</param>
        /// <param name="value">The new value of the field.</param>
        public static void SetFieldValue(object src, string fieldName, object value) {
            var fi = src.GetType().GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
            fi.SetValue(src, value);
        }

        /// <summary>
        /// Checks if a point is in a rectangle.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="rect"></param>
        /// <returns></returns>
        public static bool PointInRectangle(float x, float y, Rectangle rect) {
            if (x <= rect.X) return false;
            if (x >= rect.X + rect.Width) return false;
            if (y <= rect.Y) return false;
            if (y >= rect.Y + rect.Height) return false;
            return true;
        }

        /// <summary>
        /// Checks if a point is in a rectangle.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="rx"></param>
        /// <param name="ry"></param>
        /// <param name="rw"></param>
        /// <param name="rh"></param>
        /// <returns></returns>
        public static bool PointInRectangle(float x, float y, float rx, float ry, float rw, float rh) {
            if (x <= rx) return false;
            if (x >= rx + rw) return false;
            if (y <= ry) return false;
            if (y >= ry + rh) return false;
            return true;
        }

        /// <summary>
        /// Checks if a point is in a rectangle.
        /// </summary>
        /// <param name="xy"></param>
        /// <param name="rect"></param>
        /// <returns></returns>
        public static bool PointInRectangle(Vector2 xy, Rectangle rect) {
            return PointInRectangle((float)xy.X, (float)xy.Y, rect);
        }

        /// <summary>
        /// Checks if a value is in a specified range.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static bool InRange(float x, float min, float max) {
            if (x >= min && x <= max) return true;
            return false;
        }

        /// <summary>
        /// Returns a value according to a value's position in a range.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static int Subset(float x, float min, float max) {
            if (x < min) return -1;
            if (x > max) return 1;
            return 0;
        }

        /// <summary>
        /// Wrap and angle and keep it within the range of 0 to 360.
        /// </summary>
        /// <param name="angle"></param>
        /// <returns></returns>
        public static float WrapAngle(float angle) {
            angle %= 360;
            if (angle > 180)
                return angle - 360;
            else if (angle <= -180)
                return angle + 360;
            else
                return angle;
        }

        /// <summary>
        /// Find the distance between a point and a rectangle. Returns 0 if the point is within the rectangle.
        /// </summary>
        /// <param name="px">The x-position of the point.</param>
        /// <param name="py">The y-position of the point.</param>
        /// <param name="rx">The x-position of the rect.</param>
        /// <param name="ry">The y-position of the rect.</param>
        /// <param name="rw">The width of the rect.</param>
        /// <param name="rh">The height of the rect.</param>
        /// <returns>The distance.</returns>
        public static float DistanceRectPoint(float px, float py, float rx, float ry, float rw, float rh) {
            if (px >= rx && px <= rx + rw) {
                if (py >= ry && py <= ry + rh) return 0;
                if (py > ry) return py - (ry + rh);
                return ry - py;
            }
            if (py >= ry && py <= ry + rh) {
                if (px > rx) return px - (rx + rw);
                return rx - px;
            }
            if (px > rx) {
                if (py > ry) return Distance(px, py, rx + rw, ry + rh);
                return Distance(px, py, rx + rw, ry);
            }
            if (py > ry) return Distance(px, py, rx, ry + rh);
            return Distance(px, py, rx, ry);
        }

        public static float DistanceRectPoint(float px, float py, Rectangle rect) {
            return DistanceRectPoint(px, py, rect.X, rect.Y, rect.Width, rect.Height);
        }

        /// <summary>
        /// Returns the contents of a zip file by file name.
        /// </summary>
        /// <param name="zip"></param>
        /// <param name="source"></param>
        /// <returns></returns>
        public static MemoryStream GetZipFile(string zip, string source) {
            MemoryStream output = new MemoryStream();
            ZipFile z = ZipFile.Read(zip);
            if (z[source] != null) {
                if (z[source].UsesEncryption) {
                    z[source].ExtractWithPassword(output, Game.Instance.ZipPassword);
                }
                else {
                    z[source].Extract(output);
                }
                output.Seek(0, SeekOrigin.Begin);
            }
            else {
                return null;
            }
            return output;
        }

        /// <summary>
        /// Convert XML attributes to a dictionary.
        /// </summary>
        /// <param name="attributes"></param>
        /// <returns></returns>
        public static Dictionary<string, string> XMLAttributesToDictionary(XmlAttributeCollection attributes) {
            var d = new Dictionary<string, string>();
            foreach (XmlAttribute attr in attributes) {
                d.Add(attr.Name, attr.Value);
            }
            return d;
        }

        /// <summary>
        /// Searches all known assemblies for a type and returns that type.
        /// </summary>
        /// <param name="type">The type to search for.</param>
        /// <returns>The type found.  Null if no match.</returns>
        public static Type GetTypeFromAllAssemblies(string type) {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assembly in assemblies) {
                var types = assembly.GetTypes();
                foreach (var t in types) {
                    if (t.Name == type) {
                        return t;
                    }
                }
            }
            return null;
        }

        public static string GetBasicTypeName(object obj) {
            var strarr = obj.GetType().ToString().Split('.');
            return strarr[strarr.Length - 1];
        }

        /// <summary>
        /// Compresses a string and base64 encodes it.  Use "DecompressString" to restore it.
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string CompressString(string str) {
            var bytes = Encoding.UTF8.GetBytes(str);

            using (var msi = new MemoryStream(bytes))
            using (var mso = new MemoryStream()) {
                using (var gs = new GZipStream(mso, CompressionMode.Compress)) {
                    CopyStream(msi, gs);
                }

                return Convert.ToBase64String(mso.ToArray());
            }
        }

        /// <summary>
        /// Copies a stream from source to destination.
        /// </summary>
        /// <param name="src"></param>
        /// <param name="dest"></param>
        public static void CopyStream(Stream src, Stream dest) {
            byte[] bytes = new byte[4096];

            int cnt;

            while ((cnt = src.Read(bytes, 0, bytes.Length)) != 0) {
                dest.Write(bytes, 0, cnt);
            }
        }

        /// <summary>
        /// Decompresses a string compressed with "CompressString"
        /// </summary>
        /// <param name="base64str"></param>
        /// <returns></returns>
        public static string DecompressString(string base64str) {
            byte[] bytes;
            try {
                bytes = Convert.FromBase64String(base64str);
            }
            catch {
                return null;
            }

            using (var msi = new MemoryStream(bytes))
            using (var mso = new MemoryStream()) {
                using (var gs = new GZipStream(msi, CompressionMode.Decompress)) {
                    CopyStream(gs, mso);
                }

                return Encoding.UTF8.GetString(mso.ToArray());
            }
        }

        /// <summary>
        /// Converts a dictionary of string, string into a string of data.
        /// </summary>
        /// <param name="dictionary"></param>
        /// <param name="keydelim"></param>
        /// <param name="valuedelim"></param>
        /// <returns></returns>
        public static string DictionaryToString(Dictionary<string, string> dictionary, string keydelim, string valuedelim) {
            string str = "";

            foreach (var s in dictionary) {
                str += s.Key + keydelim + s.Value + valuedelim;
            }

            str = str.Substring(0, str.Length - valuedelim.Length);

            return str;
        }

        /// <summary>
        /// Convert a string into a dictionary of string, string.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="keydelim"></param>
        /// <param name="valuedelim"></param>
        /// <returns></returns>
        public static Dictionary<string, string> StringToDictionary(string source, string keydelim, string valuedelim) {
            var d = new Dictionary<string, string>();

            string[] split = Regex.Split(source, valuedelim);

            foreach (var s in split) {
                string[] entry = Regex.Split(s, keydelim);
                d.Add(entry[0], entry[1]);
            }

            return d;
        }

        /// <summary>
        /// Calculate an MD5 hash of a string.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string MD5Hash(string input) {
            // step 1, calculate MD5 hash from input
            MD5 md5 = System.Security.Cryptography.MD5.Create();
            byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
            byte[] hash = md5.ComputeHash(inputBytes);

            // step 2, convert byte array to hex string
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hash.Length; i++) {
                sb.Append(hash[i].ToString("X2"));
            }
            return sb.ToString();
        }

        public static float DistributedPosition(float size, float padding, int count, int index, bool centered = false) {
            if (centered) {
                var pos = index * (size + padding);
                var width = count * size + (count - 1) * padding;
                return pos - width * 0.5f;
            }
            return index * (size + padding);
        }

        /// <summary>
        /// Get all the values of an enum without a giant mess of code.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IEnumerable<T> EnumValues<T>() {
            return Enum.GetValues(typeof(T)).Cast<T>();
        }

        /// <summary>
        /// Shortcut function for adding multiple items to a list in one line.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="items"></param>
        public static void AddToList<T>(List<T> list, params T[] items) {
            foreach (var i in items) {
                list.Add(i);
            }
        }

        /// <summary>
        /// Convert a string number to a float. If the string contains a % char it will be parsed as a percentage.
        /// For example "50%" => 0.5f, or 50 => 50f.
        /// </summary>
        /// <param name="percent">The string to parse.</param>
        /// <returns>If the string contained % a float of the percent on the scale of 0 to 1. Otherwise the float.</returns>
        public static float ParsePercent(string percent) {
            if (percent.Contains('%')) {
                percent = percent.TrimEnd('%');
                return float.Parse(percent) * 0.01f;
            }
            return float.Parse(percent);
        }

        /// <summary>
        /// Download data from a URL. This method will stall the thread. Probably
        /// should use something like a BackgroundWorker or something but I haven't
        /// figured that out yet.
        /// </summary>
        /// <param name="url">The url to download.</param>
        /// <returns>The string downloaded from the url.</returns>
        public static string GetURLString(string url) {
            string s = "";
            using (WebClient client = new WebClient()) {
                s = client.DownloadString(url);
            }
            return s;
        }

        public static T ParseEnum<T>(string str) {
            return (T)Enum.Parse(typeof(T), str);
        }

        public static string EnumValueToString(Enum value) {
            return string.Format("{0}.{1}", value.GetType(), value);
        }
        

    }
}
