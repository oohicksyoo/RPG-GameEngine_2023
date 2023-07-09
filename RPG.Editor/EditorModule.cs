namespace RPG.DearImGUI {
	using System.Numerics;
	using Windows;
	using Engine.Core;
	using Engine.Graphics;
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

		#endregion
		
		
		#region IModule

		public string ModuleName => GetType().Name;

		public string Name => "Editor";

		public Version Version => new Version(0, 1, 0);

		public int Priority => int.MaxValue - 4;

		public void Awake() {
			ImGUISystem.Initialize();
			
			this.Windows.Add(new SimpleWindow("1"));
			this.Windows.Add(new SimpleWindow("2"));
			this.Windows.Add(new SimpleWindow("3"));
			this.Windows.Add(new RenderTargetWindow("Game", Application.Instance.GameFramebuffer.RenderTextureId));
		}

		public void Start() {
			
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