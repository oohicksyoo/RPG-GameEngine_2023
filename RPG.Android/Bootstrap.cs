using System.Runtime.InteropServices;
using RPG.Engine.Core;
using SampleGame.Core;
using Module.SDL2;
using Module.OpenGL;
using Application = RPG.Engine.Core.Application;

namespace RPG.Android {
	public static class Bootstrap {

		delegate void Main();

		public static void SDL_Main() {
			//System Module
			Application.Instance.Register<SDL2Module>();
			
			//Input Module
			Application.Instance.Register<SDL2InputModule>();
			
			//Graphics Module
			Application.Instance.Register<OpenGLModule>();
			
			//TODO: Find a way to make this project agnostic
			Application.Instance.Start(Project.Instance);
		}

		public static void SetupMain() {
			SetMain(SDL_Main);
		}

		[DllImport("main")]
		static extern void SetMain(Main main);

	}
}