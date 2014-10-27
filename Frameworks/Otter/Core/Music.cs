using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Otter {
    /// <summary>
    /// Class used to load and play music files. Music is streamed from the file source, or an IO stream.
    /// </summary>
    public class Music {

        SFML.Audio.Music music;

        /// <summary>
        /// The global volume to play all music at.
        /// </summary>
        public static float GlobalVolume {
            get {
                return globalVolume;
            }
            set {
                globalVolume = value;
                foreach (var m in musics) {
                    m.Volume = m.Volume; //update music volume
                }
            }
        }

        static float globalVolume = 1f;

        /// <summary>
        /// The local volume to play this music at.
        /// </summary>
        public float Volume {
            get {
                return volume;
            }
            set {
                volume = value;
                music.Volume = Util.Clamp(GlobalVolume * volume, 0f, 1f) * 100f;
            }
        }

        float volume = 1f;

        static List<Music> musics = new List<Music>();

        /// <summary>
        /// Load a music file from a file path.
        /// </summary>
        /// <param name="source"></param>
        public Music(string source, bool loop = true) {
            music = new SFML.Audio.Music(source);
            music.Loop = loop;
            music.RelativeToListener = false;
            music.Attenuation = 100;
            
            // keeping track of music for volume changes
            // this might be a bad idea.
            musics.Add(this);
        }

        /// <summary>
        /// Load a music stream from an IO stream.
        /// </summary>
        /// <param name="stream"></param>
        public Music(Stream stream) {
            music = new SFML.Audio.Music(stream);
            music.Loop = true;
        }

        /// <summary>
        /// Play the music.
        /// </summary>
        public void Play() {
            music.Volume = Util.Clamp(GlobalVolume * Volume, 0f, 1f) * 100f;
            music.Play();
        }

        /// <summary>
        /// Stop the music!
        /// </summary>
        public void Stop() {
            music.Stop();
        }

        /// <summary>
        /// Pause the music.
        /// </summary>
        public void Pause() {
            music.Pause();
        }

        /// <summary>
        /// Adjust the pitch of the music.  Default value is 1.
        /// </summary>
        public float Pitch {
            set { music.Pitch = value; }
            get { return music.Pitch; }
        }

        /// <summary>
        /// Set the playback offset of the music.
        /// </summary>
        public int Offset {
            set { music.PlayingOffset = new TimeSpan(0, 0, 0, 0, value); }
            get { return music.PlayingOffset.Milliseconds; }
        }

        /// <summary>
        /// Determines if the music should loop or not.
        /// </summary>
        public bool Loop {
            set { music.Loop = value; }
            get { return music.Loop; }
        }

        /// <summary>
        /// The duration in milliseconds of the music.
        /// </summary>
        public int Duration {
            get { return music.Duration.Milliseconds; }
        }

        /// <summary>
        /// Clear the list of internally tracked Music objects.  The list is used to
        /// adjust the volume of music when the global volume is changed, so any music
        /// currently tracked will no longer auto-adjust on global volume changes.
        /// </summary>
        public static void Clear() {
            musics.Clear();
        }
    }
}
