using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Otter {
    /// <summary>
    /// Component used for a value with built in min, max, and wrapping capabilities. Can be useful for making
    /// menus.
    /// </summary>
    public class Counter : Component {

        public int Value = 0;
        public bool Wrap = false;
        public bool Cap = true;
        public int Min = 0;
        public int Max = 0;
        public int InitialValue = 0;

        public Action OnIncrement;
        public Action OnDecrement;
        public Action OnMax;
        public Action OnMin;

        public Counter(int value, int min, int max, bool wrap = false, bool cap = true) {
            InitialValue = value;
            Value = value;
            if (min > max) throw new ArgumentException("Min must be lower than max!");

            Min = min;
            Max = max;
            Wrap = wrap;
            Cap = cap;
        }

        public void Reset() {
            Value = InitialValue;
        }

        public int Increment(int value = 1) {
            Value += value;
            if (Cap) {
                if (Value > Max) {
                    if (Wrap) {
                        while (Value > Max) {
                            Value -= (Length);
                        }
                    }
                    else Value = Max;
                }
            }
            return Value;
        }

        public int Decrement(int value = 1) {
            Value -= value;
            if (Cap) {
                if (Value < Min) {
                    if (Wrap) {
                        while (Value < Min) {
                            Value += (Length);
                        }
                    }
                    else Value = Min;
                }
            }
            return Value;
        }

        public override void Update() {
            if (Cap) {
                if (Value > Max) {
                    if (Wrap) {
                        while (Value > Max) {
                            Value -= (Length);
                        }
                    }
                    else Value = Max;
                }
                if (Value < Min) {
                    if (Wrap) {
                        while (Value < Min) {
                            Value += (Length);
                        }
                    }
                    else Value = Min;
                }
            }
        }

        public bool AtMax {
            get { return Value >= Max; }
        }
        public bool AtMin {
            get { return Value <= Min; }
        }

        public int Length {
            get { return Math.Abs(Max - Min)+1; }
        }

        public void GoToMax() {
            Value = Max;
        }

        public void GoToMin() {
            Value = Min;
        }

        public static implicit operator float(Counter counter) {
            return counter.Value;
        }
        public static implicit operator int(Counter counter) {
            return (int)counter.Value;
        }
    }
}
