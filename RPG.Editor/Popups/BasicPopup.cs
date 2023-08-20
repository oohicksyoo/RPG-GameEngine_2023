namespace RPG.DearImGUI.Popups {
	using ImGuiNET;

	public class BasicPopup : AbstractPopup {


		#region Overrides

		public override string Name => "Basic";

		protected override void OnRenderGui() {
			ImGui.Text($"Hello World - {this.Name} Popup");
		}
		
		public override void Close() {
			
		}

		#endregion

	}
}