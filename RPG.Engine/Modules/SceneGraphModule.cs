namespace RPG.Engine.Modules {

	using System.Drawing;
	using System.Numerics;
	using System.Text;
	using Components.Interfaces;
	using Core;
	using Core.Interfaces;
	using Graphics;
	using Graphics.Interfaces;
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

		private Shader Shader {
			get;
			set;
		}

		private IntPtr Samplers {
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
			this.Shader = Shader.DefaultAseprite;
			int[] samplers = new int[32];
			for (int i = 0; i < 32; i++) {
				samplers[i] = i;
			}

			this.Samplers = samplers.ArrayToIntPtr();
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
			this.Samplers.FreeArrayIntPtr();
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
			
			//TODO: Maybe these can move to the Module Pre and Post render?
			IBatcher batcher = Application.Instance.GraphicsModule.Batcher;

			//Start Shader
			this.Shader.Use();
			
			//Setup multi textures
			this.Shader.SetIntArray("textures", 32, this.Samplers);
			//TODO: Issue: Intptr are causing lost memory issues, we need to create a nicer class wrapper which can take the type, return the IntPtr and handle its life cycle to free memory

			//Set Properties
			this.Shader.SetFloat("wireframe", 0);
			this.Shader.SetMatrix4x4("view", GetViewMatrix());
			this.Shader.SetMatrix4x4("projection", GetProjectionMatrix());

			//Run Batcher //TODO: Memory leak here while using batcher
			batcher.Begin();
			((IRender)this.RootNode).Render();
			batcher.End();

			//End Shader
			this.Shader.EndUse();
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

		private Matrix4x4 GetViewMatrix() {
			Matrix4x4 view = Matrix4x4.Identity;
			float zoom = 1;
			Vector2 windowSize = Application.Instance.Project.WindowSize;
			view = Matrix4x4.CreateTranslation(0, 0, -5);
			
			return view;
		}

		private Matrix4x4 GetProjectionMatrix() {
			Matrix4x4 projection = Matrix4x4.Identity;
			float zoom = 1;
			Vector2 windowSize = Application.Instance.Project.WindowSize;

			/*projection = Matrix4x4.CreateOrthographicOffCenter(
				0.0f,
				windowSize.X * zoom,
				windowSize.Y * zoom,
				0.0f,
				0.01f,
				1000.0f
			);*/

			projection = Matrix4x4.CreatePerspectiveFieldOfView(
				MathHelper.ToRadians(90),
				windowSize.X / windowSize.Y,
				0.1f,
				100.0f
			);
			
			return projection;
		}

		#endregion

	}
}