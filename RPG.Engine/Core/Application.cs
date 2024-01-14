using System.Runtime.InteropServices;
using RPG.Engine.Core.Interfaces;
using RPG.Engine.Modules;
using RPG.Engine.Modules.Interfaces;
using RPG.Engine.Utility;

namespace RPG.Engine.Core {
	using System.Drawing;
	using Graphics;
	using Platform;
	using Settings;

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
		
		public IEditorModule? EditorModule {
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

		public bool IsApplicationRunning {
			get;
			private set;
		}

		public bool IsGameRunning {
			get;
			set;
		}

		public PlatformType PlatformType {
			get;
			private set;
		}
		
		/// <summary>
		/// Framebuffer containing the game scene rendering
		/// </summary>
		public Framebuffer GameFramebuffer {
			get;
			private set;
		}

		public Framebuffer SceneFramebuffer {
			get;
			private set;
		}

		/// <summary>
		/// Framebuffer containing all the results from ImGui's rendered Framebuffer
		/// </summary>
		public Framebuffer EditorFramebuffer {
			get;
			set;
		}

		public Framebuffer FinalFramebuffer {
			get {
				if (this.IsEditor) {
					return this.EditorFramebuffer;
				}

				return this.GameFramebuffer;
			}
		}

		private bool IsEditor {
			get;
			set;
		}

		#endregion
		
		
		#region Public Methods

		public void Start(IProject project, PlatformType platformType, bool isEditor = false) {
			if (this.SystemModule == null) {
				throw new Exception("Missing SystemModule which is needed to run");
			}
			
			if (this.InputModule == null) {
				throw new Exception("Missing InputModule which is needed to run");
			}
			
			if (this.GraphicsModule == null) {
				throw new Exception("Missing GraphicsModule which is needed to run");
			}

			this.IsEditor = isEditor;
			this.Project = project;
			this.PlatformType = platformType;
			
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
			
			//Create Rendering Framebuffer
			this.GameFramebuffer = this.GraphicsModule.CreateFramebuffer(this.Project.WindowSize);
			this.SceneFramebuffer = this.GraphicsModule.CreateFramebuffer(this.Project.WindowSize);
			this.EditorFramebuffer = this.GraphicsModule.CreateFramebuffer(this.Project.WindowSize);
			
			//General IModule Startup
			ModulesStartup();

			RunGameLoop();
		}

		public void RequestShutdown() {
			this.IsApplicationRunning = false;
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
					case IEditorModule editorModule:
						if (this.EditorModule == null) {
							this.EditorModule = editorModule;
						} else {
							Debug.Warning(GetType().Name, $"{typeof(T).Name} module with IEditorModule already exists and will not be added.");
						}
						break;
				}
			} else {
				Debug.Warning(GetType().Name, $"{typeof(T).Name} module already exists and will not be added.");
			}
		}

		public T? Get<T>() {
			return this.ModuleList.Get<T>();
		}

		#endregion


		#region Private Methods

		private void RunGameLoop() {
			this.IsApplicationRunning = !(this.SystemModule == null || this.GraphicsModule == null || this.InputModule == null);
			this.IsGameRunning = !this.IsEditor;

			while (this.IsApplicationRunning) {
				this.SystemModule.TimeStep();
				this.InputModule.BeginFrame();
				
				if (this.InputModule.Poll()) {
					
					((IModule)this.SystemModule!).Update();
					
					this.EditorModule?.Update();
					this.ModuleList.Update();

					//Rendering Phase
					this.SystemModule.BeginPresent();
					
					//Editor Scene Rendering
					if (this.IsEditor) {
						this.GraphicsModule.PreRender(this.SceneFramebuffer.Id, Color.SlateGray);
						this.ModuleList.Render();
						this.ModuleList.PostProcess();
						this.GraphicsModule.PostRender();
					}

					//Game Scene Rendering
					{
						this.GraphicsModule.PreRender(this.GameFramebuffer.Id, ProjectSettings.Instance.BackgroundColor);
						this.ModuleList.Render();
						this.ModuleList.PostProcess();
						this.GraphicsModule.PostRender();
					}
					
					//Render Editor Module
					this.EditorModule?.Render();
					
					//Scene Presenting
					this.SystemModule.Present();
				} else {
					this.IsApplicationRunning = false;
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
			
			this.EditorModule?.Shutdown();
			
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