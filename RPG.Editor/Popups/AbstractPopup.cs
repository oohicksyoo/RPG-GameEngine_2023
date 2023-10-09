namespace RPG.DearImGUI.Popups {
	using System.Numerics;
	using Engine.Core;
	using Engine.Modules.Interfaces;
	using ImGuiNET;

	public abstract class AbstractPopup : IEditorPopup {

		public abstract string Name {
			get;
		}

		public bool IsOpen {
			get;
			protected set;
		} = true;

		protected bool HasInitialized {
			get;
			private set;
		}

		protected abstract void OnRenderGui();

		public abstract void Close();

		protected virtual void OnOpen() {
			
		}

		public virtual void Render() {
			ImGui.SetNextWindowPos(ImGui.GetMainViewport().GetCenter(), ImGuiCond.Appearing, new Vector2(0.5f, 0.5f));
			ImGui.SetNextWindowSize(Application.Instance.Project.WindowSize * 0.5f);
			bool isOpen = this.IsOpen;
			if (ImGui.BeginPopupModal(this.Name, ref isOpen, ImGuiWindowFlags.AlwaysAutoResize | ImGuiWindowFlags.NoMove)) {

				if (!this.HasInitialized) {
					this.HasInitialized = true;
					OnOpen();
				}
				
				//Render just the popup UI
				OnRenderGui();
				
				ImGui.EndPopup();
			}
			this.IsOpen = isOpen;
		}

	}
}