namespace RPG.Engine.Input {
	
	using System;
	using System.Numerics;
	
	public class Keyboard {
		
		#region Constants

		public const int MAX_BUTTONS = 512;

		#endregion
		
		
		#region Properties
		
		public bool[] Pressed {
			get;
			private set;
		} = new bool[MAX_BUTTONS];
		
		public bool[] Down {
			get;
			private set;
		} = new bool[MAX_BUTTONS];
		
		public bool[] Released {
			get;
			private set;
		} = new bool[MAX_BUTTONS];
		
		public ulong[] Timestamp {
			get;
			private set;
		} = new ulong[MAX_BUTTONS];
		
		#endregion
		
		
		#region Public Functions

		public bool IsPressed(KeyboardKeys key) {
			return this.Pressed[(int) key];
		}
		
		public bool IsDown(KeyboardKeys key) {
			return this.Down[(int) key];
		}
		
		public bool IsReleased(KeyboardKeys key) {
			return this.Released[(int) key];
		}
		
		//TODO: Add extra helpers for the specific buttons themselves

		public void Copy(Keyboard other) {
			Array.Copy(other.Pressed, 0, this.Pressed, 0, MAX_BUTTONS);
			Array.Copy(other.Down, 0, this.Down, 0, MAX_BUTTONS);
			Array.Copy(other.Released, 0, this.Released, 0, MAX_BUTTONS);
			Array.Copy(other.Timestamp, 0, this.Timestamp, 0, MAX_BUTTONS);
		}

		public void BeginFrame() {
			Array.Fill(this.Pressed, false);
			Array.Fill(this.Released, false);
		}

		#endregion
	}
}