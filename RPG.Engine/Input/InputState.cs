namespace RPG.Engine.Input {
	public class InputState {
		
		
		#region Properties

		public Mouse Mouse {
			get;
			private set;
		}

		public Keyboard Keyboard {
			get;
			private set;
		}

		#endregion
		
		
		#region Constructor

		public InputState() {
			this.Mouse = new Mouse();
			this.Keyboard = new Keyboard();
		}

		#endregion
		
		
		#region Public Functions

		public void Copy(InputState other) {
			this.Mouse.Copy(other.Mouse);
			this.Keyboard.Copy(other.Keyboard);
		}

		/// <summary>
		/// Called to clean up the state
		/// </summary>
		public void BeginFrame() {
			this.Mouse.BeingFrame();
			this.Keyboard.BeginFrame();
		}

		#endregion
	}
}