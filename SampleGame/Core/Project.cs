using RPG.Engine.Core.Interfaces;
using RPG.Engine.Utility;

namespace SampleGame.Core {
	public class Project : Singleton<Project>, IProject {

		#region IProject

		public string Name => "Sample Game";

		//TODO: Build number should be grabbed from somewhere and increased or something when we make builds
		public Version Version => new Version(0, 1, 0);

		public int WindowWidth => 1280;

		public int WindowHeight => 720;

		#endregion

	}
}