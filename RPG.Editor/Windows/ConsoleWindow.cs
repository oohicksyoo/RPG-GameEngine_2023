namespace RPG.DearImGUI.Windows {
	using System.Drawing;
	using System.Numerics;
	using Engine.Utility;
	using ImGuiNET;

	public class ConsoleWindow : AbstractWindow {

		private float CurrentScrollY {
			get;
			set;
		}
		
		public ConsoleWindow(bool isOpen = true) : base(isOpen) {
			this.CurrentScrollY = 0.0f;
		}

		public override string Name => "Console";

		protected override void OnRenderGui() {
			foreach (Log log in Debug.Logs) {
				Color color = Color.White;
				switch (log.logType) {
					case LogType.Standard:
						color = Color.LawnGreen;
						break;
					case LogType.Warning:
						color = Color.Yellow;
						break;
					case LogType.Error:
						color = Color.Red;
						break;
				}

				Vector4 white = new Vector4(255, 255, 255, 255);
				Vector4 chosenColor = new Vector4(color.R, color.G, color.B, color.A);
				ImGui.PushStyleColor(ImGuiCol.Text, chosenColor);
				ImGui.Text($"[{log.caller}] ");
				ImGui.SameLine(150);
				ImGui.Text($"[{log.timeStamp.ToShortTimeString()}] ");
				ImGui.SameLine(225);
				ImGui.PopStyleColor();
				ImGui.PushStyleColor(ImGuiCol.Text, white);
				ImGui.Text($"{log.message}");
				ImGui.PopStyleColor();
			}
			//ImGui.SetScrollHereY(this.CurrentScrollY);
		}
	}
}