﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SFML;
using SFML.Window;
using System.Globalization;

namespace Otter {
    /// <summary>
    /// Class used for managing input from the keyboard, mouse, and joysticks. Updated by the active Game.
    /// </summary>
    public class Input {

        #region Public
        /// <summary>
        /// A reference to the current active instance.
        /// </summary>
        public static Input Instance;

        /// <summary>
        /// The maximum size of the string of recent key presses.
        /// </summary>
        public static int KeystringSize = 500;

        /// <summary>
        /// Determines if the mouse should be locked to the center of the screen.
        /// </summary>
        public static bool CenteredMouse = false;

        /// <summary>
        /// The reference to the game that owns this class.
        /// </summary>
        public Game Game { get; internal set; }

        /// <summary>
        /// The last known key that was pressed.
        /// </summary>
        public Key LastKey { get; private set; }

        /// <summary>
        /// The last known mouse button that was pressed.
        /// </summary>
        public MouseButton LastMouseButton { get; private set; }

        /// <summary>
        /// The last known button pressed on each joystick.
        /// </summary>
        public List<int> LastButton { get; private set; }
        
        /// <summary>
        /// The current string of keys that were pressed.
        /// </summary>
        public string KeyString = "";

        /// <summary>
        /// The current number of joysticks connected.
        /// </summary>
        public int JoysticksConnected {
            get {
                int count = 0;
                for (uint i = 0; i < Joystick.Count; i++) {
                    if (Joystick.IsConnected(i)) {
                        count++;
                    }
                }
                return count;
            }
        }

        #endregion

        #region Private

        int mouseWheelDelta = 0, currentMouseWheelDelta = 0;

        int
            keysPressed = 0,
            currentKeysPressed = 0,
            prevKeysPressed = 0,
            mouseButtonsPressed = 0,
            currentMouseButtonsPressed = 0,
            prevMouseButtonsPressed = 0;

        List<int>
            buttonsPressed = new List<int>(),
            prevButtonsPressed = new List<int>();

        internal bool bufferReleases = true;

        Dictionary<Key, bool> activeKeys = new Dictionary<Key, bool>();
        Dictionary<Key, bool> currentKeys = new Dictionary<Key, bool>();
        Dictionary<Key, bool> previousKeys = new Dictionary<Key, bool>();

        Dictionary<MouseButton, bool> activeMouseButtons = new Dictionary<MouseButton, bool>();
        Dictionary<MouseButton, bool> currentMouseButtons = new Dictionary<MouseButton, bool>();
        Dictionary<MouseButton, bool> previousMouseButtons = new Dictionary<MouseButton, bool>();

        List<Dictionary<uint, bool>> activeButtons = new List<Dictionary<uint, bool>>();
        List<Dictionary<uint, bool>> currentButtons = new List<Dictionary<uint, bool>>();
        List<Dictionary<uint, bool>> previousButtons = new List<Dictionary<uint, bool>>();

        List<Key> keyReleaseBuffer = new List<Key>();
        List<MouseButton> mouseReleaseBuffer = new List<MouseButton>();
        List<List<uint>> buttonReleaseBuffer = new List<List<uint>>();

        Dictionary<JoyAxis, float> axisThreshold = new Dictionary<JoyAxis, float>();

        Dictionary<JoyAxis, AxisSet> axisSet = new Dictionary<JoyAxis, AxisSet>();

        struct AxisSet {
            public AxisButton Plus;
            public AxisButton Minus;
        };

        int gameMouseX;
        int gameMouseY;

        #endregion

        internal Input(Game game) {
            Game = game;
            Instance = this;
            Init();
        }

        internal void WindowInit() {
            Game.Window.KeyPressed += OnKeyPressed;
            Game.Window.TextEntered += OnTextEntered;
            Game.Window.MouseButtonPressed += OnMousePressed;
            Game.Window.KeyReleased += OnKeyReleased;
            Game.Window.MouseButtonReleased += OnMouseReleased;
            Game.Window.JoystickButtonPressed += OnButtonPressed;
            Game.Window.JoystickButtonReleased += OnButtonReleased;
            Game.Window.JoystickConnected += OnJoystickConnected;
            Game.Window.JoystickMoved += OnJoystickMoved;
            Game.Window.MouseWheelMoved += OnMouseWheelMoved;
           
        }

        internal void GameMouseUpdate(int x, int y) {
            gameMouseX += x;
            gameMouseY += y;
            gameMouseX = (int)Util.Clamp(gameMouseX, 0, Game.Width);
            gameMouseY = (int)Util.Clamp(gameMouseY, 0, Game.Height);
        }

        internal void Init() {
            LastButton = new List<int>();

            for (uint i = 0; i < Joystick.Count; i++) {
                activeButtons.Add(new Dictionary<uint, bool>());
                currentButtons.Add(new Dictionary<uint, bool>());
                previousButtons.Add(new Dictionary<uint, bool>());

                for (uint j = 0; j < Joystick.ButtonCount; j++) {
                    activeButtons[(int)i][j] = false;
                    currentButtons[(int)i][j] = false;
                    previousButtons[(int)i][j] = false;
                }
                foreach(AxisButton axisButton in Enum.GetValues(typeof(AxisButton))) {
                    activeButtons[(int)i][(uint)axisButton] = false;
                    currentButtons[(int)i][(uint)axisButton] = false;
                    previousButtons[(int)i][(uint)axisButton] = false;
                }

                buttonsPressed.Add(0);
                prevButtonsPressed.Add(0);

                LastButton.Add(-1);

                buttonReleaseBuffer.Add(new List<uint>());

                Joystick.Update();
            }

            foreach (Key key in Enum.GetValues(typeof(Key))) {
                activeKeys.Add(key, false);
                currentKeys.Add(key, false);
                previousKeys.Add(key, false);
            }

            foreach (MouseButton button in Enum.GetValues(typeof(MouseButton))) {
                activeMouseButtons.Add(button, false);
                currentMouseButtons.Add(button, false);
                previousMouseButtons.Add(button, false);
            }

            foreach (JoyAxis axis in Enum.GetValues(typeof(JoyAxis))) {
                axisThreshold.Add(axis, 0.5f);
            }

            AxisSet ax;

            ax.Plus = AxisButton.XPlus;
            ax.Minus = AxisButton.XMinus;
            axisSet.Add(JoyAxis.X, ax);

            ax.Plus = AxisButton.YPlus;
            ax.Minus = AxisButton.YMinus;
            axisSet.Add(JoyAxis.Y, ax);

            ax.Plus = AxisButton.ZPlus;
            ax.Minus = AxisButton.ZMinus;
            axisSet.Add(JoyAxis.Z, ax);

            ax.Plus = AxisButton.RPlus;
            ax.Minus = AxisButton.RMinus;
            axisSet.Add(JoyAxis.R, ax);

            ax.Plus = AxisButton.UPlus;
            ax.Minus = AxisButton.UMinus;
            axisSet.Add(JoyAxis.U, ax);

            ax.Plus = AxisButton.VPlus;
            ax.Minus = AxisButton.VMinus;
            axisSet.Add(JoyAxis.V, ax);

            ax.Plus = AxisButton.PovXPlus;
            ax.Minus = AxisButton.PovXMinus;
            axisSet.Add(JoyAxis.PovX, ax);

            ax.Plus = AxisButton.PovYPlus;
            ax.Minus = AxisButton.PovYMinus;
            axisSet.Add(JoyAxis.PovY, ax);

        }

        #region EventHandlers

        void OnJoystickConnected(object sender, JoystickConnectEventArgs e) {
        }

        void OnTextEntered(object sender, TextEventArgs e) {
            //convert unicode to ascii to check range later
            string hexValue = (Encoding.ASCII.GetBytes(e.Unicode)[0].ToString("X"));
            int ascii = (int.Parse(hexValue, NumberStyles.HexNumber));

            if (e.Unicode == "\b") {
                if (KeyString.Length > 0) {
                    KeyString = KeyString.Remove(KeyString.Length - 1, 1);
                }
            }
            else if (e.Unicode == "\r") {
                KeyString += "\n";
            }
            else if (ascii >= 32 && ascii < 128) { //only add to keystring if actual character
                KeyString += e.Unicode;
            }
        }

        void OnKeyPressed(object sender, KeyEventArgs e) {
            if (!activeKeys[(Key)e.Code]) {
                keysPressed++;
            }
            activeKeys[(Key)e.Code] = true;
            LastKey = (Key)e.Code;
        }

        void OnKeyReleased(object sender, KeyEventArgs e) {
            if (bufferReleases) {
                keyReleaseBuffer.Add((Key)e.Code);
            }
            else {
                activeKeys[(Key)e.Code] = false;
            }
            keysPressed--;
        }

        void OnMousePressed(object sender, MouseButtonEventArgs e) {
            activeMouseButtons[(MouseButton)e.Button] = true;
            mouseButtonsPressed++;
            LastMouseButton = (MouseButton)e.Button;
        }

        void OnMouseReleased(object sender, MouseButtonEventArgs e) {
            if (bufferReleases) {
                mouseReleaseBuffer.Add((MouseButton)e.Button);
            }
            else {
                activeMouseButtons[(MouseButton)e.Button] = false;
            }
            mouseButtonsPressed--;
        }

        void OnButtonPressed(object sender, JoystickButtonEventArgs e) {
            activeButtons[(int)e.JoystickId][e.Button] = true;
            buttonsPressed[(int)e.JoystickId]++;
            LastButton[(int)e.JoystickId] = (int)e.Button;
            //Console.WriteLine("{0} pressed on joy {1}", e.Button, e.JoystickId);
        }

        void OnButtonReleased(object sender, JoystickButtonEventArgs e) {
            if (bufferReleases) {
                buttonReleaseBuffer[(int)e.JoystickId].Add(e.Button);
            }
            else {
                activeButtons[(int)e.JoystickId][e.Button] = false;
            }
            buttonsPressed[(int)e.JoystickId]--;
        }

        void OnJoystickMoved(object sender, JoystickMoveEventArgs e) {
            //Console.WriteLine("Joystick " + e.JoystickId + " moved axis " + e.Axis + " to " + e.Position);
        }

        void OnMouseWheelMoved(object sender, MouseWheelEventArgs e) {
            currentMouseWheelDelta = e.Delta;
        }

        #endregion

        #region Keyboard Checks

        /// <summary>
        /// Check if a key has been pressed this update.
        /// </summary>
        /// <param name="k">The key to check.</param>
        /// <returns>True if the key has been pressed.</returns>
        public bool KeyPressed(Key k) {
            if (k == Key.Any) {
                return keysPressed > prevKeysPressed;
            }
            return currentKeys[k] && !previousKeys[k];
        }

        /// <summary>
        /// Check if a key has been released this update.
        /// </summary>
        /// <param name="k">The key to check.</param>
        /// <returns>True if the key has been released.</returns>
        public bool KeyReleased(Key k) {
            if (k == Key.Any) {
                return keysPressed < prevKeysPressed;
            }
            return !currentKeys[k] && previousKeys[k];
        }

        /// <summary>
        /// Check if a key is currently down.
        /// </summary>
        /// <param name="k">The key to check.</param>
        /// <returns>True if the key is down.</returns>
        public bool KeyDown(Key k) {
            if (k == Key.Any) {
                return keysPressed > 0;
            }
            return currentKeys[k];
        }

        /// <summary>
        /// Check if a key is currently up.
        /// </summary>
        /// <param name="k">The key to check.</param>
        /// <returns>True if the key is up.</returns>
        public bool KeyUp(Key k) {
            return !KeyDown(k);
        }

        #endregion

        #region Controller Checks

        public bool ButtonPressed(int button, int joystick = 0) {
            return currentButtons[joystick][(uint)button] && !previousButtons[joystick][(uint)button];
        }

        public bool ButtonPressed(AxisButton button, int joystick = 0) {
            return ButtonPressed((int)button, joystick);
        }

        public bool ButtonReleased(int button, int joystick = 0) {
            return !currentButtons[joystick][(uint)button] && previousButtons[joystick][(uint)button];
        }

        public bool ButtonReleased(AxisButton button, int joystick = 0) {
            return ButtonReleased((int)button, joystick);
        }

        public bool ButtonDown(int button, int joystick = 0) {
            return currentButtons[joystick][(uint)button];
        }

        public bool ButtonDown(AxisButton button, int joystick = 0) {
            return ButtonDown((int)button, joystick);
        }

        public bool ButtonUp(int button, int joystick = 0) {
            return !currentButtons[joystick][(uint)button];
        }

        public bool ButtonUp(AxisButton button, int joystick = 0) {
            return ButtonUp((int)button, joystick);
        }

        /// <summary>
        /// Get the value of a joystick axis from -100 to 100.
        /// </summary>
        /// <param name="axis">The axis to check.</param>
        /// <param name="joystick">The joystick to check.</param>
        /// <returns>The axis value from -100 to 100.</returns>
        public float GetAxis(JoyAxis axis, int joystick = 0) {
            if (Joystick.HasAxis((uint)joystick, (Joystick.Axis)axis)) {
                if (axis == JoyAxis.PovY) { //special case for dpad y
                    return Joystick.GetAxisPosition((uint)joystick, (Joystick.Axis)axis) * -1;
                }
                return Joystick.GetAxisPosition((uint)joystick, (Joystick.Axis)axis);
            }
            return 0;
        }

        /// <summary>
        /// Set the threshold for an axis to act as an AxisButton.  Defaults to 50 or one half of the joystick's total range.
        /// </summary>
        /// <param name="axis">The axis to set.</param>
        /// <param name="threshold">The threshold that the axis must pass to act as a button press.</param>
        public void SetAxisThreshold(JoyAxis axis, float threshold) {
            axisThreshold[axis] = threshold;
        }


        #endregion

        #region Mouse Checks

        public bool MouseButtonPressed(MouseButton b) {
            if (b == MouseButton.Any) {
                return mouseButtonsPressed > prevMouseButtonsPressed;
            }
            return currentMouseButtons[b] && !previousMouseButtons[b];
        }

        public bool MouseButtonReleased(MouseButton b) {
            if (b == MouseButton.Any) {
                return mouseButtonsPressed < prevMouseButtonsPressed;
            }
            return !currentMouseButtons[b] && previousMouseButtons[b];
        }

        public bool MouseButtonDown(MouseButton b) {
            if (b == MouseButton.Any) {
                return mouseButtonsPressed > 0;
            }
            return currentMouseButtons[b];
        }

        public bool MouseButtonUp(MouseButton b) {
            return !MouseButtonDown(b);
        }

        public int MouseX {
            get {
                float pos = 0;

                if (Game.LockMouseCenter) {
                    pos = gameMouseX;
                }
                else {
                    pos = Game.Window.InternalGetMousePosition().X;
                    pos -= Game.Surface.X - Game.Surface.ScaledWidth * 0.5f;
                    pos /= Game.Surface.ScaleX;
                }

                

                return (int)pos;
            }
        }

        public int MouseY {
            get {
                float pos = 0;

                if (Game.LockMouseCenter) {
                    pos = gameMouseY;
                }
                else {
                    pos = Game.Window.InternalGetMousePosition().Y;
                    pos -= Game.Surface.Y - Game.Surface.ScaledHeight * 0.5f;
                    pos /= Game.Surface.ScaleY;
                }

                return (int)pos;
            }
        }

        public int MouseRawX {
            get {
                if (Game.LockMouseCenter) {
                    return gameMouseX;
                }
                
                return Game.Window.InternalGetMousePosition().X;
            }
            set {
                if (Game.LockMouseCenter) {
                    gameMouseX = value;
                }
                else {
                    Game.Window.InternalSetMousePosition(new Vector2i(value, MouseRawY));
                }
            }
        }

        public int MouseRawY {
            get {
                if (Game.LockMouseCenter) {
                    return gameMouseY;
                }
                
                return Game.Window.InternalGetMousePosition().Y;
            }
            set {
                if (Game.LockMouseCenter) {
                    gameMouseY = value;
                }
                else {
                    Game.Window.InternalSetMousePosition(new Vector2i(MouseRawX, value));
                }
            }
        }

        public float MouseScreenX {
            get { return MouseX + Game.Scene.CameraX; }
        }

        public float MouseScreenY {
            get { return MouseY + Game.Scene.CameraY; }
        }

        public int MouseWheelDelta {
            get { return mouseWheelDelta; }
        }

        public int GameMouseX {
            get {
                return gameMouseX;
            }
            set {
                gameMouseX = value;
            }
        }

        public int GameMouseY {
            get {
                return gameMouseY;
            }
            set {
                gameMouseY = value;
            }
        }

      

        #endregion

        public void ClearKeystring() {
            KeyString = "";
        }

        internal void Update() {
            // Set instance pointer to this object.
            Instance = this;

            // Force update all joysticks.
            Joystick.Update();

            // Update the previous button dictionaries.
            previousKeys = new Dictionary<Key, bool>(currentKeys);
            previousMouseButtons = new Dictionary<MouseButton, bool>(currentMouseButtons);
            for (int i = 0; i < previousButtons.Count; i++) {
                previousButtons[i] = new Dictionary<uint, bool>(currentButtons[i]);
            }

            // Update the previous press counts
            prevKeysPressed = currentKeysPressed;
            prevMouseButtonsPressed = currentMouseButtonsPressed;
            for (int i = 0; i < prevButtonsPressed.Count; i++) {
                prevButtonsPressed[i] = buttonsPressed[i];
            }

            // Update the current to the active keys.
            currentKeys = new Dictionary<Key, bool>(activeKeys);
            currentMouseButtons = new Dictionary<MouseButton, bool>(activeMouseButtons);
            for (int i = 0; i < currentButtons.Count; i++) {
                currentButtons[i] = new Dictionary<uint, bool>(activeButtons[i]);
            }

            foreach (var k in keyReleaseBuffer) {
                activeKeys[k] = false;
            }

            currentKeysPressed = keysPressed;

            keyReleaseBuffer.Clear();

            foreach (var m in mouseReleaseBuffer) {
                activeMouseButtons[m] = false;
            }

            currentMouseButtonsPressed = mouseButtonsPressed;

            mouseReleaseBuffer.Clear();

            for (int i = 0; i < Joystick.Count; i++) {
                foreach (var b in buttonReleaseBuffer[i]) {
                    activeButtons[i][b] = false;
                }

                buttonReleaseBuffer[i].Clear();
            }
            
            // Update the Joystick axes to use as buttons
            for (int i = 0; i < Joystick.Count; i++) {
                if (Joystick.IsConnected((uint)i)) {
                    foreach (JoyAxis axis in Enum.GetValues(typeof(JoyAxis))) {
                        float a = GetAxis(axis, i) * 0.01f;
                        if (a >= axisThreshold[axis]) {
                            if (!currentButtons[i][(uint)axisSet[axis].Plus]) {
                                buttonsPressed[i]++;
                            }
                            currentButtons[i][(uint)axisSet[axis].Plus] = true;
                        }
                        else {
                            if (currentButtons[i][(uint)axisSet[axis].Plus]) {
                                buttonsPressed[i]--;
                            }
                            currentButtons[i][(uint)axisSet[axis].Plus] = false;
                        }

                        if (a <= -axisThreshold[axis]) {
                            if (!currentButtons[i][(uint)axisSet[axis].Minus]) {
                                buttonsPressed[i]++;
                            }
                            currentButtons[i][(uint)axisSet[axis].Minus] = true;
                        }
                        else {
                            if (currentButtons[i][(uint)axisSet[axis].Minus]) {
                                buttonsPressed[i]--;
                            }
                            currentButtons[i][(uint)axisSet[axis].Minus] = false;
                        }
                    }
                }
            }

            mouseWheelDelta = currentMouseWheelDelta;
            currentMouseWheelDelta = 0;
        }
    }

    #region ControllerButtons

    public enum AxisButton {
        XPlus = 100,
        XMinus,
        YPlus,
        YMinus,
        ZPlus,
        ZMinus,
        RPlus,
        RMinus,
        UPlus,
        UMinus,
        VPlus,
        VMinus,
        PovXPlus,
        PovXMinus,
        PovYPlus,
        PovYMinus,
        Any = 1000
    }

    #endregion

    #region MouseButtons

    public enum MouseButton {
        Left = 0,
        Right = 1,
        Middle = 2,
        XButton1 = 3,
        XButton2 = 4,
        ButtonCount = 5,
        Any = 1000
    }

    public enum MouseWheelDirection {
        Up = 0,
        Down
    }

    #endregion

    #region Keys

    public enum Key {
        Unknown = -1,
        A = 0,
        B = 1,
        C = 2,
        D = 3,
        E = 4,
        F = 5,
        G = 6,
        H = 7,
        I = 8,
        J = 9,
        K = 10,
        L = 11,
        M = 12,
        N = 13,
        O = 14,
        P = 15,
        Q = 16,
        R = 17,
        S = 18,
        T = 19,
        U = 20,
        V = 21,
        W = 22,
        X = 23,
        Y = 24,
        Z = 25,
        Num0 = 26,
        Num1 = 27,
        Num2 = 28,
        Num3 = 29,
        Num4 = 30,
        Num5 = 31,
        Num6 = 32,
        Num7 = 33,
        Num8 = 34,
        Num9 = 35,
        Escape = 36,
        LControl = 37,
        LShift = 38,
        LAlt = 39,
        LSystem = 40,
        RControl = 41,
        RShift = 42,
        RAlt = 43,
        RSystem = 44,
        Menu = 45,
        LBracket = 46,
        RBracket = 47,
        SemiColon = 48,
        Comma = 49,
        Period = 50,
        Quote = 51,
        Slash = 52,
        BackSlash = 53,
        Tilde = 54,
        Equal = 55,
        Dash = 56,
        Space = 57,
        Return = 58,
        Back = 59,
        Tab = 60,
        PageUp = 61,
        PageDown = 62,
        End = 63,
        Home = 64,
        Insert = 65,
        Delete = 66,
        Add = 67,
        Subtract = 68,
        Multiply = 69,
        Divide = 70,
        Left = 71,
        Right = 72,
        Up = 73,
        Down = 74,
        Numpad0 = 75,
        Numpad1 = 76,
        Numpad2 = 77,
        Numpad3 = 78,
        Numpad4 = 79,
        Numpad5 = 80,
        Numpad6 = 81,
        Numpad7 = 82,
        Numpad8 = 83,
        Numpad9 = 84,
        F1 = 85,
        F2 = 86,
        F3 = 87,
        F4 = 88,
        F5 = 89,
        F6 = 90,
        F7 = 91,
        F8 = 92,
        F9 = 93,
        F10 = 94,
        F11 = 95,
        F12 = 96,
        F13 = 97,
        F14 = 98,
        F15 = 99,
        Pause = 100,
        KeyCount = 101,
        Any = 1000
    }

    #endregion

    #region Direction

    public enum Direction {
        Up,
        Right,
        Down,
        Left
    }

    #endregion

    #region JoyAxis

    public enum JoyAxis {
        X,
        Y,
        Z,
        R,
        U,
        V,
        PovX,
        PovY
    }

    #endregion

    public enum JoyButton {
        Any = 1000
    }

}
