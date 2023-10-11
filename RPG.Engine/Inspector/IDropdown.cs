namespace RPG.Engine.Inspector {
	public interface IDropdown {

		public int SelectedIndex {
			get;
			set;
		}

		public string[] List {
			get;
		}

		public int ListCount {
			get;
		}

	}
}