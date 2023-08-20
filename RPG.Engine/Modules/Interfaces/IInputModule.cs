namespace RPG.Engine.Modules.Interfaces {
	using Input;

	public interface IInputModule : IApplicationModule {

		public Keyboard Keyboard {
			get;
		}

		public Mouse Mouse {
			get;
		}
		
		public bool Poll();

		public void BeginFrame();

	}
}