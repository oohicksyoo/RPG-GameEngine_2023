namespace RPG.Engine.Modules {

	using System.Drawing;
	using System.Text;
	using Components.Interfaces;
	using Core;
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
			
		}

		public void Update() {
			
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

		private void DebugSceneGraph() {
			string output =	DebugSceneGraphNode(this.RootNode);
			Debug.Log(this.ModuleName, $"{output}");
		}

		private string DebugSceneGraphNode(Node node, int indentLevel = 1) {
			StringBuilder stringBuilder = new StringBuilder(); 
			stringBuilder.Append(IndentLine(indentLevel));

			stringBuilder.Append($"Node ({node.Name})");
			foreach (IComponent component in node.Components) {
				stringBuilder.Append(IndentLine(indentLevel));
				stringBuilder.Append($"- {component.GetType().Name}");
			}
			
			foreach (Node childNode in node.Children) {
				string output = DebugSceneGraphNode(childNode, indentLevel + 1);
				stringBuilder.Append(output);
			}

			return stringBuilder.ToString();
		}

		private string IndentLine(int indentLevel) {
			string output = "\n";

			for (int i = 0; i < indentLevel; i++) {
				output += "\t";
			}
			
			return output;
		}

		#endregion

	}
}