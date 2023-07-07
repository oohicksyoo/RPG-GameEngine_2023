namespace RPG.DearImGUI {
	using Engine.Modules.Interfaces;

	public class EditorModule : IModule {


		#region IModule

		public string ModuleName => GetType().Name;

		public string Name => "Editor";

		public Version Version => new Version(0, 1, 0);

		public int Priority => int.MaxValue - 4;

		public void Awake() {
			ImGUISystem.Initialize();
		}

		public void Start() {
			
		}

		public void Update() {
			ImGUISystem.Update();
		}

		public void Shutdown() {
			
		}

		#endregion
		
	}
}