namespace RPG.Engine.Modules.Interfaces {
	public interface ISystemModule : IApplicationModule {
		
		/// <summary>
		/// Called before other shutdowns happen so we can off load specific actions to other IApplicationModules if needed
		/// </summary>
		public void PreShutdown();

		public IntPtr GetProcAddress(string name);

		public void TimeStep();

		public void BeginPresent();

		public void Present();
	}
}