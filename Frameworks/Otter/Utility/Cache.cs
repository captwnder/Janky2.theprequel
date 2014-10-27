using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SFML.Graphics;
using SFML.Audio;
using System.IO;

namespace Otter {
    /// <summary>
    /// Class that manages the cache of sounds, fonts, and textures.
    /// </summary>
    class Sounds {
        static Dictionary<string, SoundBuffer> sounds = new Dictionary<string, SoundBuffer>();

        public static SoundBuffer Load(string source) {
            if (!File.Exists(source)) return Load(Game.Instance.ZipAssets, source);
            if (sounds.ContainsKey(source)) {
                return sounds[source];
            }
            sounds.Add(source, new SoundBuffer(source));
            return sounds[source];
        }

        public static SoundBuffer Load(string zip, string source) {
            if (sounds.ContainsKey(source)) {
                return sounds[source];
            }
            SoundBuffer buffer = new SoundBuffer(Util.GetZipFile(zip, source));
            sounds.Add(source, buffer);
            return sounds[source];
        }
    }

    class Fonts {
        static Dictionary<string, Font> fonts = new Dictionary<string, Font>();

        public static Font Load(string source) {
            if (!File.Exists(source)) return Load(Game.Instance.ZipAssets, source);
            if (fonts.ContainsKey(source)) {
                return fonts[source];
            }
            fonts.Add(source, new Font(source));
            return fonts[source];
        }

        public static Font Load(string zip, string source) {
            if (fonts.ContainsKey(source)) {
                return fonts[source];
            }
            fonts.Add(source, new Font(Util.GetZipFile(zip, source)));
            return fonts[source];
        }
    }

    public class Textures {

        static Dictionary<string, SFML.Graphics.Texture> textures = new Dictionary<string, SFML.Graphics.Texture>();
        static Dictionary<Stream, SFML.Graphics.Texture> texturesStreamed = new Dictionary<Stream, SFML.Graphics.Texture>();

        public static SFML.Graphics.Texture Load(string source) {
            if (!File.Exists(source)) {
                if (Game.Instance.UseZipResources) {
                    var zipload = Util.GetZipFile(Game.Instance.ZipAssets, source);
                    if (zipload == null) {

                    }
                    else {
                        return Load(Game.Instance.ZipAssets, source);
                    }
                }
                else {
                    throw new FileNotFoundException("The texture \"" + source + "\" could not be found.");
                }
            }
            if (textures.ContainsKey(source)) {
                return textures[source];
            }
            textures.Add(source, new SFML.Graphics.Texture(source));
            return textures[source];
        }

        /// <summary>
        /// This doesn't really work right now.  Textures in images wont update
        /// if you do this.
        /// </summary>
        /// <param name="source"></param>
        public static void Reload(string source) {
            textures.Remove(source);
            Load(source);
        }

        /// <summary>
        /// This doesn't work right now.  Textures in images wont update if you
        /// do this.
        /// </summary>
        public static void ReloadAll() {
            var keys = textures.Keys;
            textures.Clear();
            foreach (var k in keys) {
                Load(k);
            }
        }

        public static SFML.Graphics.Texture Load(Stream stream) {
            if (texturesStreamed.ContainsKey(stream)) {
                return texturesStreamed[stream];
            }
            texturesStreamed.Add(stream, new SFML.Graphics.Texture(stream));

            return texturesStreamed[stream];
        }

        public static SFML.Graphics.Texture Load(string zip, string source) {
            if (textures.ContainsKey(source)) {
                return textures[source];
            }
            textures.Add(source, new SFML.Graphics.Texture(Util.GetZipFile(zip, source)));
            return textures[source];
        }

        /// <summary>
        /// Tests to see if a file exists using multiple sources.  This also checks to see if
        /// the zip file for the game exists and contains the file.
        /// </summary>
        /// <param name="source">The filepath.</param>
        /// <returns>True if the file exists.</returns>
        public static bool Exists(string source) {
            if (source == null) {
                return false;
            }

            if (File.Exists(source)) {
                return true;
            }

            if (!File.Exists(Game.Instance.ZipAssets)) {
                return false;
            }
            var zipload = Util.GetZipFile(Game.Instance.ZipAssets, source);
            if (zipload != null) {
                return true;
            }

            return false;
        }
    }
}
