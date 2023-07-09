namespace RPG.Engine.Input {
	using System.Numerics;
	using Core;
	using Modules.Interfaces;

	public abstract class AbstractInputModule : IModule, IInputModule {


		#region Properties

		public Keyboard Keyboard => this.CurrentState.Keyboard;

		public Mouse Mouse => this.CurrentState.Mouse;

		/// <summary>
		/// Last frames state
		/// </summary>
		private InputState LastState {
			get;
			set;
		}
		
		/// <summary>
		/// Current frames state
		/// </summary>
		private InputState CurrentState {
			get;
			set;
		}
		
		/// <summary>
		/// Next frames state; empty state so it is easier to copy from
		/// </summary>
		private InputState NextState {
			get;
			set;
		}

		#endregion
		
		
		#region IModule

		public abstract string ModuleName {
			get;
		}

		public abstract string Name {
			get;
		}

		public abstract Version Version {
			get;
			protected set;
		}

		public abstract int Priority {
			get;
		}

		public abstract void Awake();

		public abstract void Start();

		public abstract void Update();

		public abstract void Shutdown();

		#endregion


		#region IInputModule

		public virtual void Initialize() {
			//Setup input state stuff
			this.LastState = new InputState();
			this.CurrentState = new InputState();
			this.NextState = new InputState();
		}

		public abstract bool Poll();

		public void BeginFrame() {
			//Get ready to record all teh states of input
			this.LastState.Copy(this.CurrentState);
			this.CurrentState.Copy(this.NextState);
			this.NextState.BeginFrame();
		}

		#endregion
		
		
		#region Protected Functions

		protected void OnMousePosition(Vector2 position) {
			this.NextState.Mouse.Position = position;
		}

		protected void OnMouseDown(MouseButtons button) {
			this.NextState.Mouse.Down[(int) button] = true;
			this.NextState.Mouse.Pressed[(int) button] = true;
			this.NextState.Mouse.Timestamp[(int) button] = Time.ElapsedDuration;
		}

		protected void OnMouseUp(MouseButtons button) {
			this.NextState.Mouse.Down[(int) button] = false;
			this.NextState.Mouse.Released[(int) button] = true;
		}

		protected void OnMouseWheel(float x, float y) {
			this.NextState.Mouse.Wheel = new Vector2(x, y);
		}

		protected void OnKeyDown(KeyboardKeys key) {
			this.NextState.Keyboard.Down[(int) key] = true;
			this.NextState.Keyboard.Pressed[(int) key] = true;
			this.NextState.Keyboard.Timestamp[(int) key] = Time.ElapsedDuration;
		}
		
		protected void OnKeyUp(KeyboardKeys key) {
			this.NextState.Keyboard.Down[(int) key] = false;
			this.NextState.Keyboard.Released[(int) key] = true;
		}

		#endregion

		
	}
}