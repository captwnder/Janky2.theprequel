﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Otter {
    /// <summary>
    /// Class that is used for debug input. Wraps the Input class but only works when debug input
    /// is enabled.
    /// </summary>
    public class DebugInput {

        /// <summary>
        /// Determines if debug input will be used.  If false all checks will return false.
        /// </summary>
        public bool Enabled = false;

        /// <summary>
        /// The active instance of DebugInput.
        /// </summary>
        public static DebugInput Instance;

        /// <summary>
        /// The parent Game.
        /// </summary>
        public Game Game;

        internal DebugInput(Game game) {
            Game = game;

            Instance = this;
        }

        /// <summary>
        /// Check if a key was pressed.
        /// </summary>
        /// <param name="k">The key to check.</param>
        /// <returns>True if that key was pressed.</returns>
        public bool KeyPressed(Key k) {
            if (!Enabled) return false;

            return Game.Input.KeyPressed(k);
        }

        /// <summary>
        /// Check if a key was released.
        /// </summary>
        /// <param name="k">The key to check.</param>
        /// <returns>True if that key was released.</returns>
        public bool KeyReleased(Key k) {
            if (!Enabled) return false;

            return Game.Input.KeyReleased(k);
        }

        /// <summary>
        /// Check if a key is down.
        /// </summary>
        /// <param name="k">The key to check.</param>
        /// <returns>True if that key is down.</returns>
        public bool KeyDown(Key k) {
            if (!Enabled) return false;

            return Game.Input.KeyDown(k);
        }

        /// <summary>
        /// Check if a key is up.
        /// </summary>
        /// <param name="k">The key to check.</param>
        /// <returns>True if that key is up.</returns>
        public bool KeyUp(Key k) {
            if (!Enabled) return false;

            return Game.Input.KeyUp(k);
        }

    }
}
