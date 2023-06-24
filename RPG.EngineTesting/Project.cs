using RPG.Engine.Core.Interfaces;
using RPG.Engine.Utility;

namespace RPG.EngineTesting {
	public class Project : Singleton<Project>, IProject {

		#region IProject

		public string Name => "Engine Testing";
		
		public Version Version => new Version(0, 1, 0);

		public int WindowWidth => 1280;

		public int WindowHeight => 720;

		#endregion

	}
}