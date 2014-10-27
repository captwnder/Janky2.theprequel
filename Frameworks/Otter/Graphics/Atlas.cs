using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using SFML.Graphics;
using System.IO;

namespace Otter {
    /// <summary>
    /// Class used for loading textures from an Atlas, or a set of Atlases. This class is built to support
    /// atlases created with Sparrow/Starling exporting from TexturePacker http://www.codeandweb.com/texturepacker
    /// </summary>
    public class Atlas {

        Dictionary<string, AtlasTexture> subtextures = new Dictionary<string, AtlasTexture>();

        static Dictionary<string, SFML.Graphics.Texture> textures = new Dictionary<string, SFML.Graphics.Texture>();

        /// <summary>
        /// Designed for Sparrow/Starling exporting from TexturePacker http://www.codeandweb.com/texturepacker
        /// </summary>
        /// <param name="source">The reltive path to the atlas data file.  The png should also be in the same directory.</param>
        public Atlas(string source = "") {
            if (source != "") {
                Add(source);
            }
        }

        /// <summary>
        /// Add another atlas to the collection of textures.  Duplicate names will destroy this.
        /// </summary>
        /// <param name="source">The relative path to the data file.  The png should be in the same directory.</param>
        public Atlas Add(string source) {
            var xml = new XmlDocument();
            xml.Load(source);

            var atlas = xml.GetElementsByTagName("TextureAtlas");

            var imagePath = Path.GetDirectoryName(source) + "/";

            if (imagePath == "/") imagePath = "";

            foreach (XmlElement a in xml.GetElementsByTagName("TextureAtlas")) {
                foreach (XmlElement e in xml.GetElementsByTagName("SubTexture")) {
                    var name = e.AttributeString("name");
                    var uniqueName = true;

                    foreach (var atest in subtextures.Values) {
                        if (atest.Name == name) {
                            uniqueName = false;
                            break;
                        }
                    }

                    if (uniqueName) {
                        var atext = new AtlasTexture();
                        atext.X = e.AttributeInt("x");
                        atext.Y = e.AttributeInt("y");
                        atext.Width = e.AttributeInt("width");
                        atext.Height = e.AttributeInt("height");
                        atext.FrameHeight = e.AttributeInt("frameHeight", atext.Height);
                        atext.FrameWidth = e.AttributeInt("frameWidth", atext.Width);
                        atext.FrameX = e.AttributeInt("frameX", 0);
                        atext.FrameY = e.AttributeInt("frameY", 0);
                        atext.Name = name;
                        atext.Source = imagePath + a.AttributeString("imagePath");
                        atext.Texture = new Texture(atext.Source);
                        subtextures.Add(e.AttributeString("name"), atext);
                    }
                }
            }
            return this;
        }

        public Atlas AddMultiple(params string[] sources) {
            foreach (string s in sources) {
                Add(s);
            }
            return this;
        }

        /// <summary>
        /// Add multiple atlases from a set created by texture packer.
        /// Note: This only supports up to 10 atlases (0 - 9)
        /// </summary>
        /// <param name="source">The path until the number.  For example: "assets/atlas" if the path is "assets/atlas0.xml"</param>
        /// <param name="extension">The extension of the source without a dot</param>
        public Atlas AddNumbered(string source, string extension = "xml") {
            var i = 0;

            while (File.Exists(source + i + "." + extension)) {
                Add(source + i + "." + extension);
                i++;
            }

            return this;
        }

        internal AtlasTexture GetTexture(string name) {
            var a = subtextures[name];

            return a;
        }

        public Image GetImage(string name) {
            return new Image(this, name);
        }

        public Spritemap<T> GetSpritemap<T>(string name, int width, int height) {
            return new Spritemap<T>(this, name, width, height);
        }

        public AtlasTexture this[string name] {
            get {
                return GetTexture(name);
            }
        }

        public bool Exists(string name) {
            return GetTexture(name) != null;
        }
    }

    public class AtlasTexture {
        public int X, Y, Width, Height, FrameWidth, FrameHeight, FrameX, FrameY;
        public string Name, Source;
        public Texture Texture;

        public override string ToString() {
            return Name + " " + Source + " X: " + X + " Y: " + Y + " Width: " + Width + " Height: " + Height + " FrameX: " + FrameX + " FrameY: " + FrameY + " FrameWidth: " + FrameWidth + " FrameHeight: " + FrameHeight;
        }
    }
}
