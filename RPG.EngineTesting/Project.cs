using RPG.Engine.Core.Interfaces;
using RPG.Engine.Utility;

namespace RPG.EngineTesting {
	using System.Numerics;

	public class Project : Singleton<Project>, IProject {

		#region IProject

		public string Name => "Engine Testing";
		
		public Version Version => new Version(0, 1, 0);

		public int WindowWidth => 1280;

		public int WindowHeight => 720;

		public Vector2 WindowSize => new Vector2(this.WindowWidth, this.WindowHeight);
		
		public int PixelsPerMetre => 32;

		public void InitializeBasicModules() {
			
		}

		public void ResizeWindowEvent(int width, int height) {
			
		}
		
		public void LoadFonts() {
			
		}

		#endregion

	}
}