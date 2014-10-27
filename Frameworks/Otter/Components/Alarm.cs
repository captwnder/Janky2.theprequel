using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Otter {
    /// <summary>
    /// Component that counts down and executes a function.  After it has executed it removes itself.
    /// </summary>
    public class Alarm : Component {
        public int Delay;

        public Action Function;

        public Alarm(Action function, int delay) {
            Function = function;
            Delay = delay;
        }

        public override void Update() {
            base.Update();

            if (Timer >= Delay) {
                if (Function != null) {
                    Function();
                    RemoveSelf();
                }
            }
        }
    }
}
