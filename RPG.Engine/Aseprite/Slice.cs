namespace RPG.Engine.Aseprite {
	using System.Drawing;
	using System.Numerics;

	public class Slice : IUserData {

		public int Frame {
			get;
			set;
		}

		public string Name {
			get;
			set;
		}

		public int OriginX {
			get;
			set;
		}

		public int OriginY {
			get;
			set;
		}

		public int Width {
			get;
			set;
		}

		public int Height {
			get;
			set;
		}

		/// <summary>
		/// Note: Can be null if not set
		/// </summary>
		public Vector2 Pivot {
			get;
			set;
		}

		/// <summary>
		/// Note: Can be null if not set
		/// </summary>
		public Vector4 NineSlice {
			get;
			set;
		}
		
		public string UserDataText {
			get;
			set;
		}

		public Color UserDataColor {
			get;
			set;
		}
	}
}