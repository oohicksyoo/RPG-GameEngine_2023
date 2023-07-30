namespace RPG.DearImGUI.Popups {
	using System.Numerics;
	using Engine.Core;
	using ImGuiNET;

	public abstract class AbstractPopup {

		public abstract string Name {
			get;
		}

		public bool IsOpen {
			get;
			protected set;
		} = true;

		protected abstract void OnRenderGui();

		public abstract void Close();

		public virtual void Render() {
			ImGui.SetNextWindowPos(ImGui.GetMainViewport().GetCenter(), ImGuiCond.Appearing, new Vector2(0.5f, 0.5f));
			ImGui.SetNextWindowSize(Application.Instance.Project.WindowSize * 0.5f);
			bool isOpen = this.IsOpen;
			if (ImGui.BeginPopupModal(this.Name, ref isOpen, ImGuiWindowFlags.AlwaysAutoResize | ImGuiWindowFlags.NoMove)) {
				
				//Render just the popup UI
				OnRenderGui();
				
				ImGui.EndPopup();
			}
			this.IsOpen = isOpen;
		}

	}
}