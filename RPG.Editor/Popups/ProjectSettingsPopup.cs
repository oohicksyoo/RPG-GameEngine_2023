namespace RPG.DearImGUI.Popups {
	using System.Drawing;
	using System.Numerics;
	using Engine.Serialization;
	using Engine.Settings;
	using Engine.Utility;
	using ImGuiNET;

	public class ProjectSettingsPopup : AbstractPopup {
		

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
			
			//Background Color
			Color backgroundColor = ProjectSettings.Instance.BackgroundColor;
			Vector3 backgroundColorVector = new Vector3((float)backgroundColor.R/255f, (float)backgroundColor.G/255f, (float)backgroundColor.B/255f);
			if (ImGui.ColorPicker3($"Background Color", ref backgroundColorVector)) {
				ProjectSettings.Instance.BackgroundColor =
					Color.FromArgb((int)(backgroundColorVector.X * 255), (int)(backgroundColorVector.Y * 255), (int)(backgroundColorVector.Z * 255));
			}
		}
		
		public override void Close() {
			ProjectSettings.Instance.Save();
		}

		#endregion

	}
}