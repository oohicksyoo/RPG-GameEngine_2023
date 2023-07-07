namespace RPG.Engine.Modules.Interfaces {
	public interface IGraphicsModule : IApplicationModule, IRender {

		public string Renderer {
			get;
		}

	}
}