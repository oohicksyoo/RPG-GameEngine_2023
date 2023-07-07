using RPG.Engine.Modules.Interfaces;
using RPG.Engine.Utility;

namespace Module.SDL2 {
	
	public class SDL2InputModule : IModule, IInputModule {
		
		
		#region IInputModule

		public void Initialize() {
			this.Version = new Version(SDL.SDL_MAJOR_VERSION, SDL.SDL_MINOR_VERSION);
			Debug.Log(this.ModuleName, $"{this.Name} {this.Version}");
		}

		public bool Poll() {
			while (SDL.SDL_PollEvent(out SDL.SDL_Event e) != 0) {
				switch (e.type) {
					case SDL.SDL_EventType.SDL_QUIT:
						return false;
				}
			}
			
			return true;
		}

		public void BeginFrame() {
			//TODO: Setup input data
		}

		#endregion
		
		
		#region IModule
		
		public string ModuleName => GetType().Name;
		
		public string Name => $"SDL2";

		public Version Version {
			get;
			private set;
		} = new Version();
		
		public int Priority => int.MaxValue - 2;

		public void Awake() {
			
		}

		public void Start() {
			
		}

		public void Update() {
			
		}

		public void Shutdown() {
			Debug.Log(this.ModuleName, $"Shutdown");
		}

		#endregion
		
	}
}