namespace RPG.DearImGUI.Windows {
	using System.Runtime.InteropServices;
	using DragDrop;
	using DragDrop.Interfaces;
	using Engine.Aseprite;
	using ImGuiNET;
	using Utility;

	public class AssetWindow : AbstractWindow {
		
		public AssetWindow(bool isOpen = true) : base(isOpen) {
			
		}

		public override string Name => "Assets";

		protected override void OnRenderGui() {
			if (ImGui.TreeNodeEx("Graphics", ImGuiTreeNodeFlags.OpenOnArrow, "Graphics")) {
				RenderDirectory<AsepriteDragDropAsset>(Directory.GetCurrentDirectory(),"/Assets/Graphics", "_Aseprite");
				ImGui.TreePop();
			}
			
			if (ImGui.TreeNodeEx("Nodes", ImGuiTreeNodeFlags.OpenOnArrow, "Nodes")) {
				RenderDirectory<NodeDragDropAsset>(Directory.GetCurrentDirectory(), "/Assets/Nodes", "_Node");
				ImGui.TreePop();
			}
			
		}

		private void RenderDirectory<T>(string directory, string path, string payloadType) where T : IDragDropAsset {
			string[] files = Directory.GetFiles(directory + path);

			foreach (var filePath in files) {
				FileInfo fi = new FileInfo(filePath);
				string[] split = fi.Name.Split('.');
				string name = split[0];
				string extension = split[1];
				
				ImGui.Selectable(name);


				IDragDropAsset dragDropAsset = Activator.CreateInstance<T>();
				dragDropAsset.FilePath = $"{path}/{fi.Name}";
				dragDropAsset.Name = $"{name}";
				dragDropAsset.Extension = $"{extension}";
				
				ImGuiHelpers.DragSource(dragDropAsset);
			}
		}
	}
}