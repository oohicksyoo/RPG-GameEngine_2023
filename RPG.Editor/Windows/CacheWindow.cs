namespace RPG.DearImGUI.Windows {
	using System.Diagnostics;
	using Engine.Serialization;
	using ImGuiNET;

	public class CacheWindow : AbstractWindow {

		public CacheWindow(bool isOpen = true) : base(isOpen) {
			
		}

		public override string Name => "Cache";

		protected override void OnRenderGui() {
			var nodeMapGuids = GuidDatabase.Instance.NodeMap;
			if (ImGui.CollapsingHeader("Node Map", ImGuiTreeNodeFlags.DefaultOpen)) {
				foreach (var kvp in nodeMapGuids) {
					ImGui.Text($"{kvp.Key} - {kvp.Value.Name}");
				}
			}

			var componentMapGuids = GuidDatabase.Instance.ComponentMap;
			if (ImGui.CollapsingHeader("Component Map", ImGuiTreeNodeFlags.DefaultOpen)) {
				foreach (var kvp in componentMapGuids) {
					ImGui.Text($"{kvp.Key} - {kvp.Value.Node.Name}:{kvp.Value.GetType().Name}");
				}
			}
		}
	}
}