namespace RPG.DearImGUI.Popups {
	using Engine.Serialization;
	using Engine.Settings;
	using ImGuiNET;

	public class ProjectSettingsPopup : AbstractPopup {


		#region Constructor

		public ProjectSettingsPopup() {
			Serializer.Instance.Deserialize(ProjectSettings.Instance);
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
		}
		
		public override void Close() {
			Serializer.Instance.Serialize(ProjectSettings.Instance);
		}

		#endregion

	}
}