namespace RPG.Engine.Core.Interfaces {
	
	/// <summary>
	/// Information about the project
	/// </summary>
	public interface IProject {
		
		public string Name {
			get;
		}

		public int WindowWidth {
			get;
		}
		
		public int WindowHeight {
			get;
		}
	}
}