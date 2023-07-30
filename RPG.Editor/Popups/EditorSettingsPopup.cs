namespace RPG.DearImGUI.Popups {
	
	using ImGuiNET;
	
	public class EditorSettingsPopup : AbstractPopup {


		#region Overrides

		public override string Name => "Editor Settings";

		protected override void OnRenderGui() {
			ImGui.Text($"Hello World - {this.Name} Popup");
		}

		public override void Close() {
			
		}

		#endregion

	}
}