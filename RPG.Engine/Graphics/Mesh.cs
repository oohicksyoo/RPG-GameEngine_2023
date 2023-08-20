namespace RPG.Engine.Graphics {
	
	//TODO: Mesh cleanup
	public class Mesh {


		#region Constructor

		public Mesh(List<Vertex> vertices, List<int> indices) {
			this.Vertices = vertices;
			this.Indices = indices;
		}

		#endregion
		

		#region Property

		public int IndiceCount => this.Indices.Count;

		public List<Vertex> Vertices {
			get;
			private set;
		}
		
		private List<int> Indices {
			get;
			set;
		}

		#endregion

	}
}