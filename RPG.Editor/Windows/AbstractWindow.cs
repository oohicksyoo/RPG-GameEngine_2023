namespace RPG.DearImGUI.Windows {
	using System.Numerics;
	using ImGuiNET;

	public abstract class AbstractWindow {

		#region Properties

		public bool IsOpen {
			get;
			protected set;
		}

		public abstract string Name {
			get;
		}

		public Vector2 WindowPosition {
			get;
			private set;
		}
		
		public Vector2 WindowSize {
			get;
			private set;
		}

		#endregion


		#region Constructor

		public AbstractWindow(bool isOpen) {
			this.IsOpen = isOpen;
		}

		#endregion


		#region Virtual Methods

		public virtual void Render(uint dockId) {
			if (!this.IsOpen) {
				return;
			}
			
			ImGui.SetNextWindowDockID(dockId, ImGuiCond.FirstUseEver);

			bool isOpen = this.IsOpen;
			ImGui.Begin(this.Name, ref isOpen);

			this.WindowPosition = ImGui.GetCursorScreenPos();
			this.WindowSize = ImGui.GetContentRegionAvail();

			OnRenderGui();
			
			ImGui.End();
			this.IsOpen = isOpen;
		}

		#endregion

		
		#region Abstract Methods

		protected abstract void OnRenderGui();

		#endregion

	}
}