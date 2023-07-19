namespace RPG.Engine.Components {
	using System.Numerics;
	using Attributes;
	using Core;
	using Interfaces;

	[Singular]
	public class Transform : AbstractComponent {


		#region Constructor

		public Transform() : base() {
			
		}

		#endregion


		#region Properties

		[Inspector]
		public Vector2 Position {
			get;
			set;
		} = Vector2.Zero;

		[Inspector]
		public float Rotation {
			get;
			set;
		} = 0;
		
		[Inspector]
		public Vector2 Scale {
			get;
			set;
		} = Vector2.One;

		#endregion
		
	}
}