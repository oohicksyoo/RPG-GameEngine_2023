namespace RPG.Engine.Inspector {
	public abstract class AbstractDropdown : IDropdown {

		public string[] List {
			get;
			set;
		}

		public int SelectedIndex {
			get;
			set;
		}

		public string SelectedValue {
			get {
				if (this.SelectedIndex < this.ListCount) {
					return this.List[this.SelectedIndex];
				}
				return String.Empty;
			}
		}

		public int ListCount => this.List.Length;


		public AbstractDropdown(string[] list) {
			this.List = list;
		}

		public void PresetValue(string value) {
			for (int i = 0; i < this.ListCount; i++) {
				if (this.List[i] == value) {
					this.SelectedIndex = i;
					return;
				}
			}
		}

	}
}