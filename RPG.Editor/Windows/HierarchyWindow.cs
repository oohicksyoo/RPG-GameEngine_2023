namespace RPG.DearImGUI.Windows {
	using System.Runtime.InteropServices;
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
		public Node RootNode {
			private get;
			set;
		}
		
		public HierarchyWindow(bool isOpen = true) : base(isOpen) {
		}

		public override string Name => "Hierarchy";

		protected override void OnRenderGui() {
			if (this.RootNode == null) {
				return;
			}
			
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
			
			if (ImGui.BeginDragDropSource()) {
				int sizeOfChar = Marshal.SizeOf<char>();
				string dataString = $"{node.Guid}";
				IntPtr data = Marshal.StringToHGlobalAnsi(dataString);
					
				ImGui.SetDragDropPayload("_Node", data, (uint)(sizeOfChar * dataString.Length));
				ImGui.Text($"Node");
				ImGui.EndDragDropSource();
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