using System.Runtime.InteropServices;
using RPG.Engine.Core.Interfaces;
using RPG.Engine.Modules;
using RPG.Engine.Modules.Interfaces;
using RPG.Engine.Utility;

namespace RPG.Engine.Core {
	public class Application : Singleton<Application> {


		#region Private Variables

		private ModuleList moduleList;

		#endregion


		#region Properties

		public ISystemModule? SystemModule {
			get;
			private set;
		}
		
		public IInputModule? InputModule {
			get;
			private set;
		}
		
		public IGraphicsModule? GraphicsModule {
			get;
			private set;
		}
		
		private ModuleList ModuleList {
			get {
				return moduleList ??= new ModuleList();
			}
		}

		public IProject? Project {
			get;
			private set;
		}

		#endregion
		
		
		#region Public Methods

		public void Start(IProject project) {
			if (this.SystemModule == null) {
				throw new Exception("Missing SystemModule which is needed to run");
			}
			
			if (this.InputModule == null) {
				throw new Exception("Missing InputModule which is needed to run");
			}
			
			if (this.GraphicsModule == null) {
				throw new Exception("Missing GraphicsModule which is needed to run");
			}
			
			this.Project = project;
			
			//Platform Starting Information
			Debug.Log(this.Project.Name, $"Version {this.Project.Version}");
			Debug.Log(this.Project.Name, $"Platform: {RuntimeInformation.OSDescription} ({RuntimeInformation.OSArchitecture})");
			Debug.Log(this.Project.Name, $"Framework: {RuntimeInformation.FrameworkDescription}");
			Debug.Log(this.Project.Name, $"Window Size: ({this.Project.WindowWidth}x{this.Project.WindowHeight})");
			Debug.Log(this.Project.Name, $"Modules: {this.ModuleList.Count}");
			
			//Startup of critical IApplicationModule
			this.SystemModule.Initialize();
			this.InputModule.Initialize();
			this.GraphicsModule.Initialize();
			
			//General IModule Startup
			ModulesStartup();

			RunGameLoop();
		}
		
		public void Register<T>() where T : IModule {
			if (!ModuleList.Has<T>()) {
				IModule module = ModuleList.Register<T>();
				switch (module) {
					case ISystemModule systemModule:
						if (this.SystemModule == null) {
							this.SystemModule = systemModule;
						} else {
							Debug.Warning(GetType().Name, $"{typeof(T).Name} module with ISystemModule already exists and will not be added.");
						}
						break;
					case IInputModule inputModule:
						if (this.InputModule == null) {
							this.InputModule = inputModule;
						} else {
							Debug.Warning(GetType().Name, $"{typeof(T).Name} module with IInputModule already exists and will not be added.");
						}
						break;
					case IGraphicsModule graphicsModule:
						if (this.GraphicsModule == null) {
							this.GraphicsModule = graphicsModule;
						} else {
							Debug.Warning(GetType().Name, $"{typeof(T).Name} module with IGraphicsModule already exists and will not be added.");
						}
						break;
				}
			} else {
				Debug.Warning(GetType().Name, $"{typeof(T).Name} module already exists and will not be added.");
			}
		}

		#endregion


		#region Private Methods

		private void RunGameLoop() {
			bool isRunning = !(this.SystemModule == null || this.GraphicsModule == null || this.InputModule == null);

			while (isRunning) {
				this.SystemModule.TimeStep();
				
				if (this.InputModule.Poll()) {
					this.InputModule.BeginFrame();
					
					((IModule)this.SystemModule!).Update();
					
					this.ModuleList.Update();

					//Rendering Phase
					this.SystemModule.BeginPresent();
					this.GraphicsModule.PreRender();
					this.ModuleList.PreRender();
					this.GraphicsModule.Render();
					this.ModuleList.PostRender();
					this.GraphicsModule.PostRender();
					this.SystemModule.Present();
				} else {
					isRunning = false;
				}
			}
			
			ModulesShutdown();
		}

		private void ModulesStartup() {
			this.ModuleList.Awake();
			this.ModuleList.Start();
		}

		private void ModulesShutdown() {
			this.ModuleList.Shutdown();
			
			//SystemModule needs to offload some tasks to the GraphicsModule for shutdown
			this.SystemModule?.PreShutdown();

			//IApplicationModule Shutdowns: Ordering is specific
			((IModule)this.GraphicsModule!).Shutdown();
			((IModule)this.InputModule!).Shutdown();
			((IModule)this.SystemModule!).Shutdown();
		}

		#endregion

	}
}