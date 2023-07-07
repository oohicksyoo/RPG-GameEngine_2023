using System.Collections;
using RPG.Engine.Modules.Interfaces;

namespace RPG.Engine.Modules {
	using System.Diagnostics;
	using Attributes;
	using Debug = Utility.Debug;

	public class ModuleList : IEnumerable<IModule> {


		#region Private Variables

		private List<IModule> modules;

		#endregion


		#region Properties

		private List<IModule> Modules {
			get {
				return modules ??= new List<IModule>();
			}
			set {
				modules = value;
			}
		}

		public int Count => this.Modules.Count;

		#endregion


		#region IEnumerable

		public IEnumerator<IModule> GetEnumerator() {
			for (int i = 0; i < this.Modules.Count; i++) {
				IModule module = modules[i];
				yield return module;
			}
		}

		IEnumerator IEnumerable.GetEnumerator() {
			for (int i = 0; i < this.Modules.Count; i++) {
				IModule module = modules[i];
				yield return module;
			}
		}

		#endregion


		#region Public Methods

		public IModule Register<T>() where T : IModule {
			T module = Activator.CreateInstance<T>();
			this.Modules.Add(module);
			this.Modules = this.Modules.OrderByDescending(x => x.Priority).ToList();
			return module;
		}

		public void Unregister<T>() where T : IModule {
			IModule? module = Get<T>();
			if (module != null) {
				this.Modules.Remove(module);
			} else {
				throw new Exception($"Module ({typeof(T)}) does not exist");
			}
		}
		
		public bool Has<T>() where T : IModule {
			foreach (var module in this.Modules) {
				if (module is T) {
					return true;
				}
			}

			return false;
		}
		
		public T? Get<T>() {
			foreach (var module in this.Modules) {
				if (module is T returnModule) {
					return returnModule;
				}
			}

			return default;
		}

		public void Awake() {
			foreach (var module in this.Modules) {
				module.Awake();
			}
		}

		public void Start() {
			foreach (var module in this.Modules) {
				module.Start();
			}
		}

		public void Update() {
			foreach (var module in this.Modules) {
				if (!(module is IApplicationModule)) {
					module.Update();
				}
			}
		}

		public void PreRender() {
			foreach (var module in this.Modules) {
				if (!(module is IApplicationModule) && module is IRender renderModule) {
					renderModule.PreRender();
				}
			}
		}

		public void Render() {
			foreach (var module in this.Modules) {
				if (!(module is IApplicationModule) && module is IRender renderModule) {
					renderModule.Render();
				}
			}
		}

		/// <summary>
		/// See if any Modules using IRender want to PostRender anything
		/// Example: Clear Debug Data, Post Processing on the frame
		/// </summary>
		public void PostRender() {
			foreach (var module in this.Modules) {
				if (!(module is IApplicationModule) && module is IRender renderModule) {
					renderModule.PostRender();
				}
			}
		}

		public void Shutdown() {
			foreach (var module in this.Modules) {
				if (!(module is IApplicationModule)) {
					module.Shutdown();
				}
			}
		}

		#endregion
		
	}
}