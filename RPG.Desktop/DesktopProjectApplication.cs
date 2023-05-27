using OpenGL;
using RPG.Engine.Core;
using RPG.Engine.Core.Interfaces;
using SampleGame.Core;
using SDL2;

namespace RPG.Desktop {
	public class DesktopProjectApplication {
		
		static void Main(string[] args) {
			//TODO: Find a way to make this project agnostic
			IProject project = Project.Instance;
			
			//System Module
			Application.Register<SDL2Module>();
			
			//Graphics Module
			Application.Register<OpenGLModule>();
			
			Application.Start(project.Name, project.WindowWidth, project.WindowHeight);
		}
	}
}
