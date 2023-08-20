namespace RPG.Engine.Aseprite {
	using System.Drawing;

	public class Tag {

		public string Name {
			get;
			set;
		}

		public LoopDirection LoopDirection {
			get;
			set;
		}

		public int From {
			get;
			set;
		}

		public int To {
			get;
			set;
		}

		public Color Color {
			get;
			set;
		}
		
	}
}