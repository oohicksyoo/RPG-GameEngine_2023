namespace RPG.Engine.Modules.Interfaces {
	using Graphics;

	public interface ISystemModule : IApplicationModule {

		public float Delta {
			get;
		}
		
		public ulong ElapsedDuration {
			get;
		}
		
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