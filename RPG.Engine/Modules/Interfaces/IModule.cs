namespace RPG.Engine.Modules.Interfaces {
	public interface IModule {

		#region Properties

		/// <summary>
		/// Class name
		/// </summary>
		public string ModuleName {
			get;
		}
		
		/// <summary>
		/// Readable english name
		/// </summary>
		public string Name {
			get;
		}

		public Version Version {
			get;
		}

		#endregion


		#region Methods

		public void Awake();

		public void Start();

		public void Update();

		public void Shutdown();

		#endregion
		
		

	}
}