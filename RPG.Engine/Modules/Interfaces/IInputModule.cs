namespace RPG.Engine.Modules.Interfaces {
	public interface IInputModule : IApplicationModule {

		public bool Poll();

		public void BeginFrame();

	}
}