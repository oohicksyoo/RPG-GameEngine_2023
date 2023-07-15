namespace RPG.Engine.Aseprite {
	
	using System.Drawing;

	public interface IUserData {
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