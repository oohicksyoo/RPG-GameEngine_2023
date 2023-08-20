namespace RPG.Engine.Graphics {
	using System.Numerics;

	public class Vertex {


		#region Property

		public Vector3 Position {
			get;
			private set;
		}

		public Vector2 TextureCoordinate {
			get;
			private set;
		}

		public Vector4 Color {
			get;
			private set;
		}

		#endregion


		#region Constructor

		public Vertex(Vector3 position, Vector2 textureCoordinate, Vector4 color) {
			this.Position = position;
			this.TextureCoordinate = textureCoordinate;
			this.Color = color;
		}

		#endregion

	}
}