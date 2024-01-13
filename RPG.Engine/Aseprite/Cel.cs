namespace RPG.Engine.Aseprite {
	using System.Drawing;

	public class Cel : IUserData {

		public Layer Layer {
			get;
			set;
		}

		/// <summary>
		/// Will need to initialize this array before use
		/// </summary>
		public Color[] Pixels {
			get;
			set;
		}

		public int X {
			get;
			set;
		}
		
		public int Y {
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
		
		public float Opactiy {
			get;
			set;
		}

		public int ZIndex {
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