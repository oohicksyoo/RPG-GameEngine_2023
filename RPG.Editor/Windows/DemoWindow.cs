namespace RPG.DearImGUI.Windows {
	using ImGuiNET;

	public class DemoWindow : AbstractWindow {

		public DemoWindow(bool isOpen) : base(isOpen) {
		}

		public override string Name => "Demo";

		protected override void OnRenderGui() {
			ImGui.ShowDemoWindow();
		}
	}
}