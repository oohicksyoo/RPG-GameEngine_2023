namespace RPG.DearImGUI.Windows {
	using Engine.Core;
	using Engine.Input;
	using ImGuiNET;

	public class SimpleWindow : AbstractWindow {


		#region Constructor

		public SimpleWindow(string uniqueWindowName, bool isOpen = true) : base(isOpen) {
			this.Name = uniqueWindowName;
		}

		#endregion
		
		
		#region AbstractWindow

		public override string Name {
			get;
		}

		protected override void OnRenderGui() {
			ImGui.Text($"{this.Name}");
		}

		#endregion

	}
}