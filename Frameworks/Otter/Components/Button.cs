using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SFML.Window;

namespace Otter {
    /// <summary>
    /// Component used for interpreting input as a button. It can recieve input from multiple sources
    /// including keyboard, mouse buttons, or joystick buttons and axes. The button input can also be
    /// controlled in code.
    /// </summary>
    public class Button : Component {

        public List<Key> Keys = new List<Key>();
        public List<List<int>> Buttons = new List<List<int>>();
        public List<MouseButton> MouseButtons = new List<MouseButton>();
        public List<MouseWheelDirection> MouseWheel = new List<MouseWheelDirection>();

        internal bool forceDown = false;
        public bool ForcedInput { get; private set; }

        bool buttonsDown = false,
            currentButtonsDown = false,
            prevButtonsDown = false;

        public bool Enabled = true;

        public string Name = "";

        public Button(string name = "") {
            for (var i = 0; i < Joystick.Count; i++) {
                Buttons.Add(new List<int>());
            }
            Name = name;
        }

        public Button AddKey(params Key[] keys) {
            foreach(var k in keys) {
                Keys.Add(k);
            }
            return this;
        }

        public Button AddMouseButton(params MouseButton[] mouseButtons) {
            foreach (var mb in mouseButtons) {
                MouseButtons.Add(mb);
            }
            return this;
        }

        public Button AddMouseWheel(MouseWheelDirection direction) {
            MouseWheel.Add(direction);
            return this;
        }

        public Button AddButton(int button, int joystick = 0) {
            Buttons[joystick].Add(button);
            return this;
        }

        public Button AddAxisButton(AxisButton button, int joystick = 0) {
            AddButton((int)button, joystick);
            return this;
        }

        /// <summary>
        /// Force the state of the button.  This will override player input.
        /// </summary>
        /// <param name="state">The state of the button, true for down, false for up.</param>
        public void ForceState(bool state) {
            forceDown = state;
            ForcedInput = true;
        }

        /// <summary>
        /// Release the button's state from forced control.  Restores player input.
        /// </summary>
        public void ReleaseState() {
            ForcedInput = false;
        }

        /// <summary>
        /// Check if the button has been pressed.
        /// </summary>
        public bool Pressed {
            get {
                if (!Enabled) return false;

                return currentButtonsDown && !prevButtonsDown;
            }
        }

        /// <summary>
        /// Check if the button has been released.
        /// </summary>
        public bool Released {
            get {
                if (!Enabled) return false;

                return !currentButtonsDown && prevButtonsDown;
            }
        }

        /// <summary>
        /// Check if the button is down.
        /// </summary>
        public bool Down {
            get {
                if (!Enabled) return false;

                return currentButtonsDown;
            }
        }

        /// <summary>
        /// Check if the button is up.
        /// </summary>
        public bool Up {
            get {
                if (!Enabled) return false;

                return !currentButtonsDown;
            }
        }

        public override void UpdateFirst() {
            base.UpdateFirst();

            buttonsDown = false;

            foreach (var k in Keys) {
                if (Input.Instance.KeyDown(k)) {
                    buttonsDown = true;
                }
            }

            for (int i = 0; i < Buttons.Count; i++) {
                foreach (var button in Buttons[i]) {
                    if (Input.Instance.ButtonDown(button, i)) {
                        buttonsDown = true;
                    }
                }
            }

            foreach (var mb in MouseButtons) {
                if (Input.Instance.MouseButtonDown(mb)) {
                    buttonsDown = true;
                }
            }

            foreach (var w in MouseWheel) {
                if (w == MouseWheelDirection.Down) {
                    if (Input.Instance.MouseWheelDelta > 0) {
                        buttonsDown = true;
                    }
                }
                if (w == MouseWheelDirection.Up) {
                    if (Input.Instance.MouseWheelDelta < 0) {
                        buttonsDown = true;
                    }
                }
            }

            if (ForcedInput) {
                buttonsDown = false;
                if (forceDown) buttonsDown = true;
            }

            prevButtonsDown = currentButtonsDown;
            currentButtonsDown = buttonsDown;

        }

    }

    
}
