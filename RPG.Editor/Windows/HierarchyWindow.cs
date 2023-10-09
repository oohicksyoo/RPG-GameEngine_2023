namespace RPG.DearImGUI.Windows {
	
	using System.Runtime.InteropServices;
	using DragDrop;
	using DragDrop.Interfaces;
	using Engine.Core;
	using Engine.Modules;
	using Engine.Serialization;
	using Engine.Serialization.Interfaces;
	using Engine.Utility;
	using ImGuiNET;
	using Popups;
	using Utility;

	/// <summary>
	/// Shows the Hierarchy starting at a specific Node, most times is the SceneGraphModule root Node but could be others if editing a Copy
	/// </summary>
	public class HierarchyWindow : AbstractWindow {
		
		public HierarchyWindow(bool isOpen = true) : base(isOpen) {
			
		}

		public override string Name => "Hierarchy";

		private bool CollectionModified {
			get;
			set;
		}

		protected override void OnRenderGui() {
			SceneGraphModule sceneGraphModule = Application.Instance.Get<SceneGraphModule>();
			
			if (sceneGraphModule == null) {
				return;
			}

			this.CollectionModified = false;

			bool isOpen = ImGui.CollapsingHeader($"{sceneGraphModule.RootNode.Name}##Header", ImGuiTreeNodeFlags.DefaultOpen);

			//Drop Target
			DropTarget dropTarget = ImGuiHelpers.DropTarget<NodeDragDropAsset>();
			if (dropTarget.HasDragDropAsset) {
				//Clear out root node to avoid filling cache with GUIDs that may exist
				sceneGraphModule.SetRootNode(null);
				IDragDropAsset dragDropAsset = (IDragDropAsset)dropTarget.DragDropAsset;
				Node node = new Node(dragDropAsset.Name);
				Serializer.Instance.Deserialize(node);
				sceneGraphModule.SetRootNode(node);
			}

			ContextMenu(sceneGraphModule.RootNode, true);

			if (isOpen) {
				foreach (Node nodeChild in sceneGraphModule.RootNode.Children) {
					RenderSingleNode(nodeChild);
					if (this.CollectionModified) {
						return;
					}
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

			ContextMenu(node);
			
			//Dragging node from Heirarchy somewhere
			IDragDropGuid dragDropGuid = Activator.CreateInstance<NodeDragDropGuid>();
			dragDropGuid.Guid = node.Guid.ToString();
			ImGuiHelpers.DragSource(dragDropGuid);
			
			if (isOpen) {
				foreach (Node nodeChild in node.Children) {
					RenderSingleNode(nodeChild);
					if (this.CollectionModified) {
						return;
					}
				}
				ImGui.TreePop();
			}
			
			ImGui.Unindent();
		}

		private void ContextMenu(Node node, bool isRoot = false) {
			if (ImGui.IsItemClicked(ImGuiMouseButton.Right)) {
				ImGui.OpenPopup("Hierarchy Context");
			}

			if (ImGui.BeginPopup("Hierarchy Context")) {

				if (isRoot) {
					if (ImGui.Button("Root Node Settings")) {
						Application.Instance.EditorModule?.OpenPopup(new BasicPopup());
					}
					ImGui.Separator();
				}
				
				if (ImGui.Button("Create Node")) {
					node.Add(new Node());
					//TODO: Mark sceneGraph as dirty
				}

				if (!isRoot && ImGui.Button("Delete")) {
					//Since it is not the root node Parent should exist
					if (node.Parent != null) {
						node.Parent.Remove(node);
						
						//Clean Up
						foreach (Node nodeChild in node.Children) {
							nodeChild.RemoveFromGuidDatabase();
						}
						node.RemoveFromGuidDatabase();
						
						//Collection was modified and we can no longer render the rest of the list this frame
						this.CollectionModified = true;
					}
				}
				
				ImGui.EndPopup();
			}

			
		}
	}
}