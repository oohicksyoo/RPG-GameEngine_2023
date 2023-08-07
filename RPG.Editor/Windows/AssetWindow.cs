namespace RPG.DearImGUI.Windows {
	using ImGuiNET;

	public class AssetWindow : AbstractWindow {
		
		public AssetWindow(bool isOpen = true) : base(isOpen) {
			
		}

		public override string Name => "Assets";

		protected override void OnRenderGui() {
			if (ImGui.TreeNodeEx("Graphics", ImGuiTreeNodeFlags.OpenOnArrow, "Graphics")) {
				RenderDirectory(Directory.GetCurrentDirectory() + "/Assets/Graphics");
				ImGui.TreePop();
			}
			
			if (ImGui.TreeNodeEx("Nodes", ImGuiTreeNodeFlags.OpenOnArrow, "Nodes")) {
				RenderDirectory(Directory.GetCurrentDirectory() + "/Assets/Nodes");
				ImGui.TreePop();
			}
			
		}

		private void RenderDirectory(string path) {
				
		}
	}
}