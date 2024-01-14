namespace RPG.Engine.Components.Interfaces {
	using Graphics;

	public interface IComponentRenderable {

		public bool CanRender {
			get;
		}

		public Transform Transform {
			get;
		}

		public Mesh Mesh {
			get;
		}

		public uint TextureId {
			get;
		}

		public int CurrentFrame {
			get;
		}

		public int SingleFrameWidth {
			get;
		}

		public int TotalFrameCount {
			get;
		}

		public int ZIndex {
			get;
		}
		
	}
}