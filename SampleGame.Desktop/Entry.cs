using RPG.Engine.Core;
using SampleGame.Core;
using Module.SDL2;
using Module.OpenGL;


namespace SampleGame.Desktop {
	using RPG.Engine.Platform;

	public class Entry {
		
		static void Main(string[] args) {
			Project.Instance.InitializeBasicModules();
			
			Application.Instance.Start(Project.Instance, PlatformType.Windows);
		}
	}
}
