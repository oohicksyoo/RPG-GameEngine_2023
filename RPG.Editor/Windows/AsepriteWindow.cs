namespace RPG.DearImGUI.Windows {
	using System.Numerics;
	using Engine.Aseprite;
	using Engine.Attributes;
	using Engine.Core;
	using Engine.Graphics;
	using ImGuiNET;

	public class AsepriteWindow : AbstractWindow {

		[Inspector]
		private AsepriteFile AsepriteFile {
			get;
			set;
		}

		private Texture Texture {
			get;
			set;
		}
		

		public AsepriteWindow(bool isOpen) : base(isOpen) {
		}

		public override string Name => "Aseprite";

		protected override void OnRenderGui() {
			if (ImGui.Button("Load Sample File")) {
				this.AsepriteFile = new AsepriteFile("/Assets/Graphics/Sample.aseprite");
				this.Texture = new Texture(this.AsepriteFile.GetPixels(), (uint)this.AsepriteFile.TextureWidth, (uint)this.AsepriteFile.TextureHeight, ColorType.RGBA);
			}

			if (this.Texture != null && this.AsepriteFile != null) {
				ImGui.Text($"{this.AsepriteFile.TextureWidth}x{this.AsepriteFile.TextureHeight}");
				ImGui.Image((IntPtr)this.Texture.ID, new Vector2(this.Texture.Width * 5, this.Texture.Height * 5), new Vector2(0, 0), new Vector2(1, 1), Vector4.One, Vector4.One);
			}
		}
	}
}