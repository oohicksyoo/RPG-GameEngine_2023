namespace RPG.DearImGUI.Windows {
	using Engine.Core;
	using Engine.Graphics.Interfaces;
	using ImGuiNET;

	public class StatisticsWindows : AbstractWindow {

		public StatisticsWindows(bool isOpen = true) : base(isOpen) {
		}

		public override string Name => "Statistics";

		protected override void OnRenderGui() {
			IBatcher batcher = Application.Instance.GraphicsModule.Batcher;
			ImGui.Text($"Draw Count: {batcher.DrawCount}");
			ImGui.Text($"Quad Count: {batcher.QuadCount}");
			ImGui.Text($"Indice Count: {batcher.IndiceCount}");
		}
	}
}