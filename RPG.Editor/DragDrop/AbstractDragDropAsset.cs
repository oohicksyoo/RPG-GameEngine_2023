namespace RPG.DearImGUI.DragDrop {
	using System.Runtime.InteropServices;
	using Interfaces;

	public abstract class AbstractDragDropAsset : IDragDropAsset {
		

		#region IDragDropAsset
		
		public abstract string Type {
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

		#endregion

	}
}