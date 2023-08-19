namespace RPG.Engine.Core.Interfaces {
	using System.Numerics;

	/// <summary>
	/// Information about the project
	/// </summary>
	public interface IProject {
		
		public string Name {
			get;
		}

		public Version Version {
			get;
		}

		public int WindowWidth {
			get;
		}
		
		public int WindowHeight {
			get;
		}

		public Vector2 WindowSize {
			get;
		}

		public int PixelsPerMetre {
			get;
		}

		public void InitializeBasicModules();
	}
}