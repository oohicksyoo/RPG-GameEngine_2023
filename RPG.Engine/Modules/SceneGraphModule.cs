namespace RPG.Engine.Modules {

	using System.Drawing;
	using System.Text;
	using Components.Interfaces;
	using Core;
	using Core.Interfaces;
	using Graphics;
	using Interfaces;
	using Newtonsoft.Json.Linq;
	using Serialization;
	using Settings;
	using Utility;

	public class SceneGraphModule : IModule, IGraphicsClear, IRender {


		#region Property

		public Node RootNode {
			get;
			private set;
		}

		private JObject CloneNode {
			get;
			set;
		}

		private string RootNodeName {
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
			SetRootNode(new Node(startingNode));
			Serializer.Instance.Deserialize(this.RootNode);
		}

		public void Start() {
			InitializeRootNode();
		}

		public void Update() {
			if (Application.Instance.IsGameRunning) {
				InitializeRootNode();
				((IRunnable)this.RootNode).Update();
			} else if (this.HasInitializedRootNode) {
				//Scene Clean Up as the editor has stopped us
				ApplyClonedNode();
				this.HasInitializedRootNode = false;
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
			if (this.RootNode == null) {
				return;
			}
			
			((IRender)this.RootNode).Render();
		}

		#endregion


		#region Public Methods

		public void SetRootNode(Node node) {
			if (this.RootNode != null) {
				this.RootNode.RemoveFromGuidDatabase();
			}
			
			this.RootNode = node;

			IEditorModule editorModule = Application.Instance.EditorModule;
			if (editorModule != null) {
				editorModule.SelectedNode = null;
			}
		}

		#endregion


		#region Private Methods

		private void InitializeRootNode() {
			if (this.HasInitializedRootNode) {
				return;
			}
			
			//Store RootNode temp in case we turn IsGameRunning to false and need to revert back
			this.CloneNode = this.RootNode.Serialize();
			this.RootNodeName = this.RootNode.Name;
			
			if (Application.Instance.IsGameRunning) {
				((IRunnable)this.RootNode).Awake();
				((IRunnable)this.RootNode).Start();
				this.HasInitializedRootNode = true;
			}
		}

		private void ApplyClonedNode() {
			//Replace RootNote with what we had before initialization
			if (this.RootNode != null) {
				this.RootNode.RemoveFromGuidDatabase();
				this.RootNode = null;
			}
			
			Node node = new Node(this.RootNodeName);
			node.Deserialize(this.CloneNode);
			SetRootNode(node);
				
			this.RootNodeName = String.Empty;
			this.CloneNode = null;
		}

		#endregion

	}
}