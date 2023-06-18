using RPG.Engine.Core;
using SampleGame.Core;
using Module.SDL2;
using Module.OpenGL;


namespace RPG.DesktopEditor {
	public class Entry {
		
		static void Main(string[] args) {
			//System Module
			Application.Instance.Register<SDL2Module>();
			
			//Input Module
			Application.Instance.Register<SDL2InputModule>();
			
			//Graphics Module
			Application.Instance.Register<OpenGLModule>();
			
			//TODO: Find a way to make this project agnostic
			Application.Instance.Start(Project.Instance);
		}
	}
}