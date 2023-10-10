using RPG.Engine.Core;
using SampleGame.Core;
using Module.SDL2;
using Module.OpenGL;


namespace SampleGame.DesktopEditor {
	using RPG.DearImGUI;

	public class Entry {
		
		static void Main(string[] args) {
			Project.Instance.InitializeBasicModules();
			
			//Register Editor Modules
			Application.Instance.Register<EditorModule>();
			
			Application.Instance.Start(Project.Instance, true);
		}
	}
}