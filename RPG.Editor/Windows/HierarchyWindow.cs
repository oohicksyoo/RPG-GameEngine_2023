namespace RPG.DearImGUI.Windows {
	using System.Runtime.InteropServices;
	using DragDrop;
	using Engine.Core;
	using Engine.Modules;
	using Engine.Serialization;
	using Engine.Serialization.Interfaces;
	using Engine.Utility;
	using ImGuiNET;
	using Utility;

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
		
		public HierarchyWindow(bool isOpen = true) : base(isOpen) {
			
		}

		public override string Name => "Hierarchy";

		protected override void OnRenderGui() {
			if (this.RootNode == null) {
				return;
			}

			bool isOpen = ImGui.CollapsingHeader($"{this.RootNode.Name}##Header", ImGuiTreeNodeFlags.DefaultOpen);

			//Drop Target
			DropTarget dropTarget = ImGuiHelpers.DropTarget<NodeDragDropAsset>();
			if (dropTarget.HasDragDropAsset) {
				Node node = new Node(dropTarget.DragDropAsset.Name);
				Serializer.Instance.Deserialize(node);
				SceneGraphModule sceneGraphModule = Application.Instance.Get<SceneGraphModule>();
				if (sceneGraphModule != null) { 
					sceneGraphModule.RootNode = node;
				}
				SetRootNode(node);
			}

			if (isOpen) {
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

		public void SetRootNode(Node node) {
			((IGuidDatabase)node).RemoveFromGuidDatabase();
			this.RootNode = node;
		}
	}
}