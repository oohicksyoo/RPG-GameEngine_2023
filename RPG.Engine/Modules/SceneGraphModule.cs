namespace RPG.Engine.Modules {

	using System.Drawing;
	using System.Text;
	using Components.Interfaces;
	using Core;
	using Graphics;
	using Interfaces;
	using Utility;

	public class SceneGraphModule : IModule, IGraphicsClear, IRender {


		#region Property

		public Node RootNode {
			get;
			private set;
		}

		#endregion
		
		
		#region IModule

		public string ModuleName => GetType().Name;

		public string Name => "SceneGraph";

		public Version Version => new Version(0, 1, 0);
		
		public int Priority => int.MaxValue - 5;

		public void Awake() {
			this.RootNode = new Node("Root");
			this.RootNode.Add(new Node("Sample 1"));
			this.RootNode.Add(new Node("Sample 2"));
			this.RootNode.Add(new Node("Sample 3"));
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