namespace RPG.Engine.Modules.Interfaces {
	
	/// <summary>
	/// Critical modules that are needed to run the application
	/// </summary>
	public interface IApplicationModule {
		
		/// <summary>
		/// Initialization of the critical application module
		/// </summary>
		public void Initialize();
		
	}
}