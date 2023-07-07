using RPG.Engine.Core;
using RPG.Engine.Core.Interfaces;
using RPG.Engine.Modules.Interfaces;
using RPG.Engine.Utility;

namespace Module.OpenGL {
	using System.Drawing;
	using RPG.Engine.Modules;

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
			Color color = Color.White;
			IGraphicsClear graphicsClear = Application.Instance.Get<IGraphicsClear>();
			if (graphicsClear != null) {
				color = graphicsClear.ClearColor;
			}
			
			GL.ClearColor(color.R / 255,color.G / 255,color.B / 255,color.A / 255);
			GL.Clear(GLEnum.COLOR_BUFFER_BIT);
			
			//Enables
		}

		public void Render() {
			//Not Needed
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