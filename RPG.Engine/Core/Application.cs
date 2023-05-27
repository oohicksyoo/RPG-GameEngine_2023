using RPG.Engine.Modules.Interfaces;

namespace RPG.Engine.Core {
	public static class Application {

		
		#region Public Methods

		public static void Start(string windowTitle, int width = 1280, int height = 720) {
			//TODO: Start gameplay loop
		}
		
		public static void Register<T>() where T : IModule {
			
		}

		#endregion

	}
}