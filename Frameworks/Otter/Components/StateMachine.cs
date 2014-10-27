using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Otter {
    /// <summary>
    /// Component that runs a basic state machine.
    /// </summary>
    public class StateMachine : Component {

        Dictionary<int, State> states = new Dictionary<int, State>();
        Dictionary<State, int> invertedstates = new Dictionary<State, int>();

        Dictionary<int, Dictionary<int, Action>> transitions = new Dictionary<int, Dictionary<int, Action>>();

        public int CurrentState = -1;

        int nextStateId = 0;
    
        public StateMachine() {

        }

        public int AddState(State state) {
            states.Add(nextStateId, state);
            invertedstates.Add(state, nextStateId);
            state.Id = nextStateId;
            state.Machine = this;
            nextStateId++;
            return nextStateId - 1;
        }

        public int AddState(State state, int id) {
            states.Add(id, state);
            invertedstates.Add(state, id);
            state.Machine = this;
            return id;
        }

        public void AddTransition(int fromState, int toState, Action function) {
            if (transitions[fromState] == null) {
                transitions.Add(fromState, new Dictionary<int, Action>());
            }
            transitions[fromState].Add(toState, function);
        }
        
        /// <summary>
        /// Add a state and automatically grab functions by their name.
        /// </summary>
        /// <param name="names">The common name in all the functions, like "Walking" in "UpdateWalking" "EnterWalking" and "ExitWalking"</param>
        /// <returns>The id to refer to the state.</returns>
        public void AddState(params string[] names) {
            foreach (var name in names) {
                if (Entity == null) throw new ArgumentException("State Machine must be Added to an Entity first!");

                var state = new State();

                //Using reflection to find all the appropriate functions!
                MethodInfo mi;
                mi = Entity.GetType().GetMethod("Enter" + name, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy);
                if (mi != null) {
                    state.OnEnter = (Action)Delegate.CreateDelegate(typeof(Action), Entity, mi);
                }

                mi = Entity.GetType().GetMethod("Update" + name, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy);
                if (mi != null) {
                    state.OnUpdate = (Action)Delegate.CreateDelegate(typeof(Action), Entity, mi);
                }

                mi = Entity.GetType().GetMethod("Exit" + name, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy);
                if (mi != null) {
                    state.OnExit = (Action)Delegate.CreateDelegate(typeof(Action), Entity, mi);
                }

                //Using reflection to assign the id to the right property
                FieldInfo fi;
                fi = Entity.GetType().GetField("state" + name, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy);
                fi.SetValue(Entity, nextStateId);

                AddState(state);
            }
        }

        public int AddState() {
            return AddState(new State());
        }

        public int AddState(Action onEnter, Action onUpdate, Action onExit) {
            return AddState(new State(onEnter, onUpdate, onExit));
        }

        public int AddState(Action onUpdate) {
            return AddState(new State(onUpdate));
        }

        public int AddState(Action onEnter, Action onUpdate, Action onExit, int id) {
            return AddState(new State(onEnter, onUpdate, onExit), id);
        }

        public int AddState(Action onUpdate, int id) {
            return AddState(new State(onUpdate), id);
        }

        public override void Update() {
            base.Update();
            if (CurrentState == -1) return;

            if (states.ContainsKey(CurrentState)) {
                s.Update();
            }
        }

        public void ChangeState(int state) {
            if (CurrentState == state) return;

            Timer = 0;

            if (states.ContainsKey(state)) {
                if (s != null) {
                    s.Exit();
                }
                CurrentState = state;
                if (s == null) throw new NullReferenceException("Next state is null.");
                s.Enter();

                if (transitions.ContainsKey(state)) {
                    if (transitions[state].ContainsKey(s.Id)) {
                        transitions[state][s.Id]();
                    }
                }
            }
        }

        public int State {
            get { return CurrentState; }
            set { ChangeState(value); }
        }

        public void ChangeState(State state) {
            if (invertedstates.ContainsKey(state)) {
                ChangeState(invertedstates[state]);
            }
        }

        State s {
            get {
                if (CurrentState == -1) {
                    return null;
                }
                return states[CurrentState];
            }
        }
    }

    /// <summary>
    /// State machine that uses a specific type.  This is really meant for using an enum as your list of states.
    /// If an enum is used, the state machine will automatically populate the states using methods in the parent
    /// Entity that match the name of the enum values.
    /// </summary>
    /// <example>
    /// Say you have an enum named State, and it has the value "Walking"
    /// When the state machine is added to the Entity, it will match any methods named:
    /// EnterWalking
    /// UpdateWalking
    /// ExitWalking
    /// And use those to build the states.  This saves a lot of boilerplate set up code.
    /// </example>
    /// <typeparam name="TState">An enum of states.</typeparam>
    public class StateMachine<TState> : Component {
        Dictionary<TState, State> states = new Dictionary<TState, State>();

        Dictionary<TState, Dictionary<TState, Action>> transitions = new Dictionary<TState, Dictionary<TState, Action>>();

        public bool AutoPopulate = true;

        public TState CurrentState;

        bool firstChange = true;

        public StateMachine() {
        }

        public override void Added() {
            base.Added();

            if (AutoPopulate) {
                if (typeof(TState).IsEnum) {
                    foreach (TState value in Enum.GetValues(typeof(TState))) {
                        AddState(value);
                    }
                }
            }
        }

        public void ChangeState(TState state) {
            if (!firstChange) {
                if (states.ContainsKey(CurrentState)) {
                    if (states[CurrentState] == states[state]) return;
                }
            }

            Timer = 0;

            var fromState = CurrentState;

            if (states.ContainsKey(state)) {
                if (s != null && !firstChange) {
                    s.Exit();
                }
                CurrentState = state;
                if (s == null) throw new NullReferenceException("Next state is null.");
                s.Enter();

                if (transitions.ContainsKey(fromState)) {
                    if (transitions[fromState].ContainsKey(state)) {
                        transitions[fromState][state]();
                    }
                }
            }

            if (firstChange) {
                firstChange = false;
            }
        }

        public override void Update() {
            base.Update();
            if (states.ContainsKey(CurrentState)) {
                s.Update();
            }
        }

        State s {
            get {
                if (!states.ContainsKey(CurrentState)) {
                    return null;
                }
                return states[CurrentState];
            }
        }

        public void AddTransition(TState fromState, TState toState, Action function) {
            if (transitions[fromState] == null) {
                transitions.Add(fromState, new Dictionary<TState, Action>());
            }
            transitions[fromState].Add(toState, function);
        }

        public void AddState(TState key, Action onEnter, Action onUpdate, Action onExit) {
            states.Add(key, new State(onEnter, onUpdate, onExit));
        }

        public void AddState(TState key, Action onUpdate) {
            states.Add(key, new State(onUpdate));
        }

        public void AddState(TState key, State value) {
            states.Add(key, value);
        }

        public void AddState(TState key) {
            var state = new State();
            var name = key.ToString();
            //Using reflection to find all the appropriate functions!
            MethodInfo mi;
            mi = Entity.GetType().GetMethod("Enter" + name, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy);
            if (mi == null) Entity.GetType().GetMethod("Enter" + name, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
            if (mi != null) {
                state.OnEnter = (Action)Delegate.CreateDelegate(typeof(Action), Entity, mi);
            }

            mi = Entity.GetType().GetMethod("Update" + name, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy);
            if (mi == null) Entity.GetType().GetMethod("Update" + name, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
            if (mi != null) {
                state.OnUpdate = (Action)Delegate.CreateDelegate(typeof(Action), Entity, mi);
            }

            mi = Entity.GetType().GetMethod("Exit" + name, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy);
            if (mi == null) Entity.GetType().GetMethod("Exit" + name, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
            if (mi != null) {
                state.OnExit = (Action)Delegate.CreateDelegate(typeof(Action), Entity, mi);
            }

            states.Add(key, state);
        }

    }

    /// <summary>
    /// Used in StateMachine. Contains functions for enter, update, and exit.
    /// </summary>
    public class State {
        public Action OnEnter;
        public Action OnUpdate;
        public Action OnExit;

        public int Id;
        public string Name;

        public StateMachine Machine;

        public State(Action onEnter = null, Action onUpdate = null, Action onExit = null) {
            Functions(onEnter, onUpdate, onExit);
        }

        public State(Action onUpdate) : this(null, onUpdate) { }

        public void Functions(Action onEnter, Action onUpdate, Action onExit) {
            OnEnter = onEnter;
            OnUpdate = onUpdate;
            OnExit = onExit;
        }

        public void Update() {
            if (OnUpdate != null) {
                OnUpdate();
            }
        }

        public void Enter() {
            if (OnEnter != null) {
                OnEnter();
            }
        }

        public void Exit() {
            if (OnExit != null) {
                OnExit();
            }
        }
    }
}
