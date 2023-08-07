namespace RPG.DearImGUI.Popups {
	using Engine.Serialization;
	using Engine.Settings;
	using ImGuiNET;

	public class ProjectSettingsPopup : AbstractPopup {


		#region Constructor

		public ProjectSettingsPopup() {
			
		}

		#endregion
		

		#region Overrides

		public override string Name => "Project Settings";

		protected override void OnRenderGui() {
			//Name
			string name = ProjectSettings.Instance.Name;
			if (ImGui.InputText($"Name", ref name, 128)) {
				ProjectSettings.Instance.Name = name;
			}
			
			//Starting Node
			string startingNode = ProjectSettings.Instance.StartingNode;
			if (ImGui.InputText($"Starting Node", ref startingNode, 128)) {
				ProjectSettings.Instance.StartingNode = startingNode;
			}
		}
		
		public override void Close() {
			ProjectSettings.Instance.Save();
		}

		#endregion

	}
}