namespace RPG.Engine.Graphics.Interfaces {
	using Components;
	using Components.Interfaces;

	public interface IBatcher {
		
		public int DrawCount {
			get;
		}

		public int QuadCount {
			get;
		}
		
		public int IndiceCount {
			get;
		}

		public ITriangleDrawer TriangleDrawer {
			get;
		}

		public void Initialize();
		public void Shutdown();
		public void Begin();
		public void End();
		public void Draw(Transform transform, IComponentRenderable renderable);
	}
}