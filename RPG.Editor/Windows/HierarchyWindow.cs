namespace RPG.DearImGUI.Windows {
	using Engine.Core;
	using Engine.Modules;
	using ImGuiNET;

	/// <summary>
	/// Shows the Hierarchy starting at a specific Node, most times is the SceneGraphModule root Node but could be others if editing a Copy
	/// </summary>
	public class HierarchyWindow : AbstractWindow {

		/// <summary>
		/// Main node starting point
		/// </summary>
		private Node RootNode {
			get;
			set;
		}
		
		public HierarchyWindow(Node rootNode, bool isOpen = true) : base(isOpen) {
			this.RootNode = rootNode;
		}

		public override string Name => "Hierarchy";

		protected override void OnRenderGui() {
			if (ImGui.CollapsingHeader($"{this.RootNode.Name}##Header", ImGuiTreeNodeFlags.DefaultOpen)) {
				foreach (Node nodeChild in this.RootNode.Children) {
					RenderSingleNode(nodeChild);
				}
			}
		}

		private void RenderSingleNode(Node node) {
			EditorModule editorModule = Application.Instance.Get<EditorModule>();
			if (editorModule == null) {
				return;
			}
			
			ImGui.Indent();

			bool isSelected = editorModule.SelectedNode != null && node.Guid == editorModule.SelectedNode.Guid;
			ImGuiTreeNodeFlags flags = ImGuiTreeNodeFlags.None;
			if (node.Children.Count == 0) {
				flags |= ImGuiTreeNodeFlags.Leaf;
			} else {
				flags |= ImGuiTreeNodeFlags.OpenOnArrow;
			}

			if (isSelected) {
				flags |= ImGuiTreeNodeFlags.Selected;
			}

			bool isOpen = ImGui.TreeNodeEx(node.Guid.ToString(), flags, node.Name);
			
			if (ImGui.IsItemClicked(ImGuiMouseButton.Left) && (ImGui.GetMousePos().X - ImGui.GetItemRectMin().X) > ImGui.GetTreeNodeToLabelSpacing()) {
				editorModule.SelectedNode = (isSelected) ? null : node;
			}
			
			if (isOpen) {
				foreach (Node nodeChild in node.Children) {
					RenderSingleNode(nodeChild);
				}
				ImGui.TreePop();
			}
			
			ImGui.Unindent();
		}
	}
}