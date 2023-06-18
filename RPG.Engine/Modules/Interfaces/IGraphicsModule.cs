namespace RPG.Engine.Modules.Interfaces {
	public interface IGraphicsModule : IApplicationModule {

		public string Renderer {
			get;
		}
		
		public void PreRender();
		
		public void Render();
		
		public void PostRender();

	}
}