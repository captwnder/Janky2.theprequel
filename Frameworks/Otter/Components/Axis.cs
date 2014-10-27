using System;
using System.Collections.Generic;
using SFML.Window;

namespace Otter {
    /// <summary>
    /// Component that represents an axis of input.  Interprets both X and Y from -1 to 1.  Can use multiple
    /// sources of input like keyboard, mouse buttons, or joystick axes and buttons.  Input can also be delivered from code.
    /// </summary>
    public class Axis : Component {

        /// <summary>
        /// The x position of the axis from -1 to 1.
        /// </summary>
        public float X;

        /// <summary>
        /// The y position of the axis from -1 to 1.
        /// </summary>
        public float Y;

        /// <summary>
        /// The previous x position of the axis.
        /// </summary>
        public float LastX;

        /// <summary>
        /// The previous y position of the axis.
        /// </summary>
        public float LastY;

        Dictionary<Direction, List<Key>> keys = new Dictionary<Direction, List<Key>>();
        Dictionary<Direction, List<List<int>>> buttons = new Dictionary<Direction, List<List<int>>>();

        List<List<JoyAxis>> xAxes = new List<List<JoyAxis>>();
        List<List<JoyAxis>> yAxes = new List<List<JoyAxis>>();

        /// <summary>
        /// Determines if the axis is currently enabled.  If false, X and Y will report 0.
        /// </summary>
        public bool Enabled = true;

        /// <summary>
        /// The range that must be exceeded by joysticks in order for their input to register.
        /// </summary>
        public float DeadZone = 0.15f;

        /// <summary>
        /// Determines if the DeadZone will be treated as 0 for joysticks.
        /// If true, remaps the range DeadZone to 100 to 0 to 1.
        /// If false, remaps the range 0 to 100 to 0 to 1.
        /// </summary>
        public bool RemapRange = true;

        /// <summary>
        /// Determines if raw data coming from the joysticks should be rounded to 2 digits.
        /// </summary>
        public bool RoundInput = true;

        /// <summary>
        /// Determines if input has any effect on the axis.  When set to true the axis will remain at
        /// the X and Y it was at when locked.
        /// </summary>
        public bool Locked = false;

        /// <summary>
        /// Check if the axis is currently forced.
        /// </summary>
        public bool ForcedInput { get; private set; }

        public Axis() {
            ForcedInput = false;

            foreach (Direction d in Enum.GetValues(typeof(Direction))) {
                keys[d] = new List<Key>();
                buttons.Add(d, new List<List<int>>());
                for (int i = 0; i < Joystick.Count; i++) {
                    buttons[d].Add(new List<int>());
                }
            }

            for (int i = 0; i < Joystick.Count; i++) {
                xAxes.Add(new List<JoyAxis>());
                yAxes.Add(new List<JoyAxis>());
            }
        }

        public Axis(Key up, Key right, Key down, Key left) : this() {
            AddKeys(up, right, down, left);
        }

        public Axis(JoyAxis x, JoyAxis y, int joystick = 0) : this() {
            AddAxis(x, y, joystick);
        }

        public Axis(AxisButton up, AxisButton right, AxisButton down, AxisButton left, int joystick = 0) : this() {
            AddButton(up, Direction.Up, joystick);
            AddButton(right, Direction.Right, joystick);
            AddButton(down, Direction.Down, joystick);
            AddButton(left, Direction.Left, joystick);
        }

        /// <summary>
        /// Add a joystick axis.
        /// </summary>
        /// <param name="x">The x axis of the joystick.</param>
        /// <param name="y">The y axis of the joystick.</param>
        /// <param name="joystick">The joystick id.</param>
        /// <returns>The Axis.</returns>
        public Axis AddAxis(JoyAxis x, JoyAxis y, int joystick = 0) {
            xAxes[joystick].Add(x);
            yAxes[joystick].Add(y);
            return this;
        }

        /// <summary>
        /// Add a joystick button.
        /// </summary>
        /// <param name="button">The joystick button id.</param>
        /// <param name="direction">The direction this button should effect.</param>
        /// <param name="joystick">The joystick id.</param>
        /// <returns>The Axis.</returns>
        public Axis AddButton(int button, Direction direction, int joystick = 0) {
            buttons[direction][joystick].Add(button);
            return this;
        }

        /// <summary>
        /// Add a joystick axis button.
        /// </summary>
        /// <param name="button">The joystick axis button.</param>
        /// <param name="direction">The direction this axis button should effect.</param>
        /// <param name="joystick">The joystick id.</param>
        /// <returns>The Axis.</returns>
        public Axis AddButton(AxisButton button, Direction direction, int joystick = 0) {
            AddButton((int)button, direction, joystick);
            return this;
        }

        /// <summary>
        /// Add a key.
        /// </summary>
        /// <param name="key">The keyboard key.</param>
        /// <param name="direction">The direction this key should effect.</param>
        /// <returns>The Axis.</returns>
        public Axis AddKey(Key key, Direction direction) {
            keys[direction].Add(key);

            return this;
        }

        /// <summary>
        /// Add keys.
        /// </summary>
        /// <param name="k">Four keys to create a pair of axes from (Up, Right, Down, Left).</param>
        /// <returns>The Axis.</returns>
        public Axis AddKeys(params Key[] k) {
            if (k.Length != 4) {
                throw new ArgumentException("Must use four keys for an axis!");
            }

            AddKey(k[0], Direction.Up);
            AddKey(k[1], Direction.Right);
            AddKey(k[2], Direction.Down);
            AddKey(k[3], Direction.Left);

            return this;
        }

        /// <summary>
        /// Force the axis state.
        /// </summary>
        /// <param name="x">The forced x state.</param>
        /// <param name="y">The forced y state.</param>
        public void ForceState(float x, float y) {
            ForcedInput = true;
            X = x;
            Y = y;
        }

        /// <summary>
        /// Force the axis x state.
        /// </summary>
        /// <param name="x">The forced x state.</param>
        public void ForceStateX(float x) {
            ForceState(x, Y);
        }

        /// <summary>
        /// Force the axis y state.
        /// </summary>
        /// <param name="y">The forced y state.</param>
        public void ForceStateY(float y) {
            ForceState(X, y);
        }

        /// <summary>
        /// Relinquish control of the axis back to input.
        /// </summary>
        public void ReleaseState() {
            ForcedInput = false;
        }

        public override void UpdateFirst() {
            base.UpdateFirst();

            LastX = X;
            LastY = Y;

            if (Locked) return;

            if (!Enabled) {
                X = 0;
                Y = 0;
                return;
            }

            if (ForcedInput) {
                return;
            }

            X = 0;
            Y = 0;

            for (int i = 0; i < Joystick.Count; i++) {
                bool live = false;
                foreach (JoyAxis axis in xAxes[i]) {
                    float a = Input.Instance.GetAxis(axis, i) * 0.01f;
                    if (Math.Abs(a) >= DeadZone) live = true;
                }
                foreach (JoyAxis axis in yAxes[i]) {
                    float a = Input.Instance.GetAxis(axis, i) * 0.01f;
                    if (Math.Abs(a) >= DeadZone) live = true;
                }

                foreach (JoyAxis axis in xAxes[i]) {
                    float a = Input.Instance.GetAxis(axis, i) * 0.01f;
                    if (live) {
                        if (RemapRange) {
                            if (a > 0) {
                                X += Util.ScaleClamp(a, 0, 1, 0, 1);
                            }
                            else {
                                X += Util.ScaleClamp(a, -1, -0, -1, 0);
                            }
                        }
                        else {
                            X += a;
                        }
                    }
                    if (RoundInput) X = (float)Math.Round(X, 2);
                }
                foreach (JoyAxis axis in yAxes[i]) {
                    float a = Input.Instance.GetAxis(axis, i) * 0.01f;
                    if (live) {
                        if (RemapRange) {
                            if (a > 0) {
                                Y += Util.ScaleClamp(a, 0, 1, 0, 1);
                            }
                            else {
                                Y += Util.ScaleClamp(a, -1, -0, -1, 0);
                            }
                        }
                        else {
                            Y += a;
                        }
                    }
                    if (RoundInput) Y = (float)Math.Round(Y, 2);
                }
            }

            foreach (Key k in keys[Direction.Up]) {
                if (Input.Instance.KeyDown(k)) {
                    Y -= 1;
                }
            }
            foreach (Key k in keys[Direction.Down]) {
                if (Input.Instance.KeyDown(k)) {
                    Y += 1;
                }
            }
            foreach (Key k in keys[Direction.Left]) {
                if (Input.Instance.KeyDown(k)) {
                    X -= 1;
                }
            }
            foreach (Key k in keys[Direction.Right]) {
                if (Input.Instance.KeyDown(k)) {
                    X += 1;
                }
            }

            for(int i = 0; i < Joystick.Count; i++) {
                foreach (int b in buttons[Direction.Up][i]) {
                    if (Input.Instance.ButtonDown(b, i)) {
                        Y -= 1;
                    }
                }
                foreach (int b in buttons[Direction.Down][i]) {
                    if (Input.Instance.ButtonDown(b, i)) {
                        Y += 1;
                    }
                }
                foreach (int b in buttons[Direction.Left][i]) {
                    if (Input.Instance.ButtonDown(b, i)) {
                        X -= 1;
                    }
                }
                foreach (int b in buttons[Direction.Right][i]) {
                    if (Input.Instance.ButtonDown(b, i)) {
                        X += 1;
                    }
                }
            }

            X = Util.Clamp(X, -1, 1);
            Y = Util.Clamp(Y, -1, 1);
        }

        /// <summary>
        /// Check if the axis has any means of input currently registered to it.
        /// </summary>
        public bool HasInput {
            get {
                if (ForcedInput) return true;
                if (keys.Count > 0) return true;
                if (buttons.Count > 0) return true;
                if (xAxes.Count > 0) return true;
                if (yAxes.Count > 0) return true;
                return false;
            }
        }
    
        /// <summary>
        /// Check of the axis is completely neutral.
        /// </summary>
        public bool Neutral {
            get {
                return X == 0 && Y == 0;
            }
        }

        public override string ToString() {
            return "[Axis X: " + X.ToString() + " Y: " + Y.ToString() + "]";
        }
    }
}
