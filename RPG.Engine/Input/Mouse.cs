namespace RPG.Engine.Input {
	
	using System;
	using System.Numerics;

	public class Mouse {

		#region Constants

		public const int MAX_BUTTONS = 5;

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

		public Vector2 Wheel {
			get;
			set;
		} = Vector2.Zero;

		public Vector2 Position {
			get;
			set;
		} = Vector2.Zero;

		public Vector2 Delta {
			get;
			set;
		} = Vector2.Zero;

		#endregion


		#region Public Functions

		public bool IsPressed(MouseButtons button) {
			return this.Pressed[(int) button];
		}
		
		public bool IsDown(MouseButtons button) {
			return this.Down[(int) button];
		}
		
		public bool IsReleased(MouseButtons button) {
			return this.Released[(int) button];
		}
		
		//TODO: Add extra helpers for the specific buttons themselves

		public void Copy(Mouse other) {
			Array.Copy(other.Pressed, 0, this.Pressed, 0, MAX_BUTTONS);
			Array.Copy(other.Down, 0, this.Down, 0, MAX_BUTTONS);
			Array.Copy(other.Released, 0, this.Released, 0, MAX_BUTTONS);
			Array.Copy(other.Timestamp, 0, this.Timestamp, 0, MAX_BUTTONS);

			this.Wheel = other.Wheel;
			this.Delta = this.Position - other.Position; //Delta movement of the mouse
			this.Position = other.Position;
		}

		public void BeingFrame() {
			Array.Fill(this.Pressed, false);
			Array.Fill(this.Released, false);
			this.Wheel = Vector2.Zero;
			this.Position = Vector2.Zero;
		}

		#endregion
	}
}