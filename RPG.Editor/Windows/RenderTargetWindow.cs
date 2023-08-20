namespace RPG.DearImGUI.Windows {
	using System.Numerics;
	using Engine.Core;
	using ImGuiNET;

	public class RenderTargetWindow : AbstractWindow {

		private uint RenderTargetId {
			get;
			set;
		}
		
		public override string Name {
			get;
		}
		
		public RenderTargetWindow(string windowName, uint renderTargetId, bool isOpen = true) : base(isOpen) {
			this.Name = windowName;
			this.RenderTargetId = renderTargetId;
		}

		protected override void OnRenderGui() {
			Vector2 windowSize = CalculateContentSize(ImGui.GetContentRegionAvail());
			ImGui.Image((IntPtr)this.RenderTargetId, windowSize, new Vector2(0, 1), new Vector2(1, 0), Vector4.One, Vector4.One);
		}

		private Vector2 CalculateContentSize(Vector2 availableSize) {
			float width = (float)Application.Instance.Project.WindowWidth;
			float height = (float)Application.Instance.Project.WindowHeight;
			float ratio = width / height;
			float reciprocal = 1 / ratio;

			height = availableSize.X * reciprocal;

			if (height > availableSize.Y) {
				height = availableSize.Y;
			}

			width = height * ratio;

			return new Vector2(width, height);
		}
	}
}