using RPG.Engine.Core.Interfaces;
using RPG.Engine.Utility;

namespace SampleGame.Core {
	public class Project : Singleton<Project>, IProject {

		#region IProject

		public string Name => "Sample Game";

		public int WindowWidth => 1280;

		public int WindowHeight => 720;

		#endregion

	}
}