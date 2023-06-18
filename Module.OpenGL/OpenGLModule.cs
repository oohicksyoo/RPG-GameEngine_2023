using RPG.Engine.Core;
using RPG.Engine.Core.Interfaces;
using RPG.Engine.Modules.Interfaces;
using RPG.Engine.Utility;

namespace Module.OpenGL {

	public class OpenGLModule : IModule, IGraphicsModule {


		#region IGraphicsModule
		
		public string Renderer {
			get;
			private set;
		}

		public void Initialize() {
			ISystemModule? systemModule = Application.Instance.SystemModule;
			if (systemModule == null) {
				throw new Exception($"Unable to initialize IGraphicsModule without an ISystemModule already registered");
			}
			
			GL.Initialize(systemModule.GetProcAddress);
			GL.Enable(GLEnum.DEPTH_TEST);

			this.Version = new Version(GL.MajorVersion, GL.MinorVersion);
			this.Renderer = GL.GetString(GLEnum.RENDERER);
			string version = GL.GetString(GLEnum.VERSION);
			Debug.Log(this.ModuleName, $"{this.Name} {this.Version} ({version}) ({this.Renderer})");

			IProject? project = Application.Instance.Project;
			if (project == null) {
				throw new Exception($"IProject is not initialized");
			}
			GL.Viewport(0,0, project.WindowWidth, project.WindowHeight);
			
			//TODO: Setup window resizing
		}

		public void PreRender() {
			GL.ClearColor(1,0,0,1);
			GL.Clear(GLEnum.COLOR_BUFFER_BIT);
			
			//Enables
		}

		public void Render() {
			//TODO: Find everyone that wants to use like IRender and have them render now
		}

		public void PostRender() {
			//Disables
		}

		#endregion


		#region IModule

		public string ModuleName => GetType().Name;
		
		public string Name => $"OpenGL";

		public Version Version {
			get;
			private set;
		} = new Version();

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