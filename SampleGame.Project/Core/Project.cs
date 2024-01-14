using RPG.Engine.Core.Interfaces;
using RPG.Engine.Utility;

namespace SampleGame.Core {
	using System.Numerics;
	using Module.OpenGL;
	using Module.SDL2;
	using RPG.Engine.Core;
	using RPG.Engine.Modules;

	public class Project : Singleton<Project>, IProject {

		#region IProject

		public string Name => "Sample Game";

		//TODO: Build number should be grabbed from somewhere and increased or something when we make builds
		public Version Version => new Version(0, 1, 0);

		public int WindowWidth => 1920;

		public int WindowHeight => 1080;
		
		public Vector2 WindowSize => new Vector2(this.WindowWidth, this.WindowHeight);
		
		public int PixelsPerMetre => 32;

		public void InitializeBasicModules() {
			//System Module
			Application.Instance.Register<SDL2Module>();
			
			//Input Module
			Application.Instance.Register<SDL2InputModule>();
			
			//Graphics Module
			Application.Instance.Register<OpenGLModule>();
			
			
			Application.Instance.Register<SceneGraphModule>();
		}
		
		public void ResizeWindowEvent(int width, int height) {
			
		}

		#endregion

	}
}