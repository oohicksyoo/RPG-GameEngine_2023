namespace RPG.DearImGUI.DragDrop {
	using Interfaces;

	public abstract class AbstractDragDropGuid : IDragDropGuid {
		
		
		#region IDragDropGuid

		public abstract string Type {
			get;
		}
		
		public string Guid {
			get;
			set;
		}

		#endregion
		
	}
}