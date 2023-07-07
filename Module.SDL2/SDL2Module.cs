using RPG.Engine.Core;
using RPG.Engine.Core.Interfaces;
using RPG.Engine.Modules.Interfaces;
using RPG.Engine.Utility;

namespace Module.SDL2 {

	//TODO: SDL2.dll and the libSDL2.dylib get built to Desktop everytime and only the specific library file should move depending on the platform
	//TODO: Abstract base with implementation based on the graphics we want to use
	public class SDL2Module : IModule, ISystemModule {


		#region Propeties

		private IntPtr Window {
			get;
			set;
		}
		
		private IntPtr Context {
			get;
			set;
		}

		private ulong PreviousTimeStep {
			get;
			set;
		}
		
		private ulong CurrentTimeStep {
			get;
			set;
		}

		private float CurrentDeltaTime {
			get;
			set;
		}

		#endregion
		
		
		#region ISystemModule

		public float Delta => this.CurrentDeltaTime;

		public void Initialize() {
			this.Version = new Version(SDL.SDL_MAJOR_VERSION, SDL.SDL_MINOR_VERSION);
			Debug.Log(this.ModuleName, $"{this.Name} {this.Version}");
			
			if (SDL.SDL_Init(SDL.SDL_INIT_VIDEO) < 0) {
				throw new Exception($"Unable to initialize SDL, Error: {SDL.SDL_GetError()}");
			}

			this.Window = IntPtr.Zero;
			IProject? project = Application.Instance.Project;
			if (project == null) {
				throw new Exception($"Application project file was not properly setup");
			}

			this.Window = SDL.SDL_CreateWindow(
				project.Name, 
				SDL.SDL_WINDOWPOS_CENTERED, SDL.SDL_WINDOWPOS_CENTERED, 
				project.WindowWidth, project.WindowHeight,
				SDL.SDL_WindowFlags.SDL_WINDOW_OPENGL | SDL.SDL_WindowFlags.SDL_WINDOW_RESIZABLE | SDL.SDL_WindowFlags.SDL_WINDOW_ALLOW_HIGHDPI
			);
			
			if (this.Window == IntPtr.Zero) {
				throw new Exception($"Unable to create a window, Error: {SDL.SDL_GetError()}");
			}

			//TODO: IGraphicsModule should be doing this
			this.Context = SDL.SDL_GL_CreateContext(this.Window);
			
			//Setup Attributes
			//TODO: IGraphicsModule should be doing this
			SDL.SDL_GL_SetAttribute(SDL.SDL_GLattr.SDL_GL_CONTEXT_PROFILE_MASK, SDL.SDL_GLprofile.SDL_GL_CONTEXT_PROFILE_CORE);
			SDL.SDL_GL_SetSwapInterval((int)SwapInterval.Immediate); //Disable 60fps lock
			
			#if RPG_ANDROID
				SDL.SDL_SetWindowFullscreen(this.Window, (uint)SDL.SDL_bool.SDL_TRUE);
			#endif
			
			this.CurrentTimeStep = SDL.SDL_GetPerformanceCounter();
			this.PreviousTimeStep = this.CurrentTimeStep;
			this.CurrentDeltaTime = 0;
		}

		public void PreShutdown() {
			Debug.Log(this.ModuleName, $"PreShutdown");
			//TODO: IGraphicsModule should be doing this
			//TODO: Think ordering of shutdown may cause the above to be in error
			SDL.SDL_GL_DeleteContext(this.Context);
		}

		public IntPtr GetProcAddress(string name) {
			//TODO: How do we know this is OpenGL running on the IGraphicsModule
			//TODO: IGraphicsModule should be doing this
			return SDL.SDL_GL_GetProcAddress(name);
		}

		public void TimeStep() {
			this.PreviousTimeStep = this.CurrentTimeStep;
			this.CurrentTimeStep = SDL.SDL_GetPerformanceCounter();
			float elapsed = (this.CurrentTimeStep - this.PreviousTimeStep) * 1000.0f;
			this.CurrentDeltaTime = (elapsed / SDL.SDL_GetPerformanceFrequency()) * 0.001f;
		}

		public void BeginPresent() {
			//TODO: This is OpenGL specific
			SDL.SDL_GL_MakeCurrent(this.Window, this.Context);
		}

		public void Present() {
			//Present the swap chain
			//TODO: This is OpenGL specific
			SDL.SDL_GL_SwapWindow(this.Window);
		}

		#endregion
		
		
		#region IModule
		
		public string ModuleName => GetType().Name;
		
		public string Name => $"SDL2";

		public Version Version {
			get;
			private set;
		} = new Version();
		
		public int Priority => int.MaxValue;

		public void Awake() {
			
		}

		public void Start() {
			
		}

		public void Update() {
			SDL.SDL_SetWindowTitle(this.Window, $"RPG Engine FPS: {(1.0f / this.CurrentDeltaTime):F2}");
		}

		public void Shutdown() {
			Debug.Log(this.ModuleName, $"Shutdown");
			SDL.SDL_DestroyWindow(this.Window);
			SDL.SDL_Quit();
		}

		#endregion
		
	}
}

