namespace RPG.DearImGUI {
	using System.Numerics;
	using Windows;
	using Engine.Core;
	using Engine.Graphics;
	using Engine.Modules;
	using Engine.Modules.Interfaces;
	using Engine.Utility;
	using ImGuiNET;

	public class EditorModule : IModule, IRender {


		#region NonSerialized Fields

		[NonSerialized]
		private List<AbstractWindow> windows;

		#endregion


		#region Properties

		private List<AbstractWindow> Windows {
			get {
				return windows ??= new List<AbstractWindow>();
			}
		}

		public Node SelectedNode {
			get;
			set;
		}

		#endregion
		
		
		#region IModule

		public string ModuleName => GetType().Name;

		public string Name => "Editor";

		public Version Version => new Version(0, 1, 0);

		public int Priority => int.MaxValue - 4;

		public void Awake() {
			ImGUISystem.Initialize();
		}

		public void Start() {
			//Configure main node for HierarchyWindow, double check if SceneGraphModule is in fact being used
			SceneGraphModule sceneGraphModule = Application.Instance.Get<SceneGraphModule>();
			Node node = new Node("Empty");
			if (sceneGraphModule != null) {
				node = sceneGraphModule.RootNode;
			}
			
			this.Windows.Add(new MenuBarWindow());
			this.Windows.Add(new HierarchyWindow(node));
			this.Windows.Add(new ConsoleWindow());
			this.Windows.Add(new RenderTargetWindow("Game", Application.Instance.GameFramebuffer.RenderTextureId));
			this.Windows.Add(new RenderTargetWindow("Scene", Application.Instance.SceneFramebuffer.RenderTextureId));
			this.Windows.Add(new InspectorWindow());
			this.Windows.Add(new AsepriteWindow(true));
		}

		public void Update() {
			ImGUISystem.Update();
		}

		public void Shutdown() {
			
		}

		#endregion


		#region IRender

		public void Render() {
			uint dockSpaceId = ImGui.DockSpaceOverViewport();
			foreach (AbstractWindow window in this.Windows) {
				window.Render(dockSpaceId);
			}
			
			ImGUISystem.Render();
		}

		#endregion
		
	}
}