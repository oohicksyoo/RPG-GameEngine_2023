namespace RPG.DearImGUI.Windows {
	using Engine.Core;
	using ImGuiNET;

	public class InspectorWindow : AbstractWindow {

		private EditorModule EditorModule {
			get;
			set;
		}
		
		public InspectorWindow(bool isOpen = true) : base(isOpen) {
			this.EditorModule = Application.Instance.Get<EditorModule>();
		}

		public override string Name => "Inspector";

		protected override void OnRenderGui() {
			if (this.EditorModule == null) {
				return;
			}

			Node node = this.EditorModule.SelectedNode;
			if (node == null) {
				ImGui.Text($"Please select a Node");
				return;
			}
			
			//Enabled
			bool isEnabled = node.IsEnabled;
			if (ImGui.Checkbox("##IsEnabled", ref isEnabled)) {
				node.IsEnabled = isEnabled;
			}

			ImGui.SameLine();

			//Name
			string name = node.Name;
			ImGui.InputText("Name", ref name, 32);
			node.Name = name;
			
			//Tag
			ImGui.SetNextItemWidth(ImGui.GetItemRectSize().X / 1.5f);
			string tag = node.Tag;
			if (ImGui.InputText("Tag", ref tag, 24)) {
				node.Tag = tag;
			}
			
			//Guid
			ImGui.TextDisabled(node.Guid.ToString());
			ImGui.SameLine();
			ImGui.Text("Guid");

		}
	}
}