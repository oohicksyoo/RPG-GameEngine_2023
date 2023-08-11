namespace RPG.DearImGUI.DragDrop.Interfaces {
	public interface IDragDropAsset {
		
		public string Type {
			get;
		}
		
		public string FilePath {
			get;
			set;
		}

		public string Name {
			get;
			set;
		}

		public string Extension {
			get;
			set;
		}
		
	}
}