namespace RPG.Engine.Aseprite {
	using System.Drawing;

	public class Layer : IUserData {

		#region Types

		[Flags]
		public enum Flags {
			Visible = 1,
			Editable = 2,
			LockMovement = 4,
			Background = 8,
			PreferLinkedCels = 16,
			Collapsed = 32,
			Reference = 64
		}

		public enum Types {
			Normal = 0,
			Group = 1
		}

		#endregion

		public Flags Flag {
			get;
			set;
		}

		public Types Type {
			get;
			set;
		}

		public string Name {
			get;
			set;
		}

		public int ChildLevel {
			get;
			set;
		}

		public int BlendMode {
			get;
			set;
		}

		public float Alpha {
			get;
			set;
		}

		public bool IsVisible => this.Flag.HasFlag(Flags.Visible);

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