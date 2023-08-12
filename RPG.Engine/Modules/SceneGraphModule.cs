namespace RPG.Engine.Modules {

	using System.Drawing;
	using System.Text;
	using Components.Interfaces;
	using Core;
	using Core.Interfaces;
	using Graphics;
	using Interfaces;
	using Serialization;
	using Settings;
	using Utility;

	public class SceneGraphModule : IModule, IGraphicsClear, IRender {


		#region Property

		public Node RootNode {
			get;
			set;
		}

		private bool HasInitializedRootNode {
			get;
			set;
		}

		#endregion
		
		
		#region IModule

		public string ModuleName => GetType().Name;

		public string Name => "SceneGraph";

		public Version Version => new Version(0, 1, 0);
		
		public int Priority => int.MaxValue - 5;

		public void Awake() {
			string startingNode = ProjectSettings.Instance.StartingNode;
			if (string.IsNullOrEmpty(startingNode)) {
				startingNode = "Root";
				ProjectSettings.Instance.StartingNode = startingNode;
				ProjectSettings.Instance.Save();
			}
			
			//Load in this node data for us
			this.RootNode = new Node(startingNode);
			Serializer.Instance.Deserialize(this.RootNode);
		}

		public void Start() {
			InitializeRootNode();
		}

		public void Update() {
			if (Application.Instance.IsGameRunning) {
				InitializeRootNode();
				((IRunnable)this.RootNode).Update();
			}
		}

		public void Shutdown() {
			
		}

		#endregion


		#region IGraphicsClear

		public Color ClearColor => Color.Black;

		#endregion


		#region IRender

		public void Render() {
			
		}

		#endregion


		#region Private Methods

		private void InitializeRootNode() {
			if (this.HasInitializedRootNode) {
				return;
			}
			
			if (Application.Instance.IsGameRunning) {
				((IRunnable)this.RootNode).Awake();
				((IRunnable)this.RootNode).Start();
				this.HasInitializedRootNode = true;
			}
		}

		#endregion

	}
}