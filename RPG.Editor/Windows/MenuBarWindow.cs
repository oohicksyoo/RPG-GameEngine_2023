namespace RPG.DearImGUI.Windows {
	using ImGuiNET;

	public class MenuBarWindow : AbstractWindow {

		public MenuBarWindow() : base(true) {
		}

		public override string Name => "Menu Bar";

		public override void Render(uint dockId) {
			OnRenderGui();
		}

		protected override void OnRenderGui() {
			if (ImGui.BeginMainMenuBar()) {
				
				//Render different headings
				
				ImGui.EndMainMenuBar();
			}
		}
	}
}