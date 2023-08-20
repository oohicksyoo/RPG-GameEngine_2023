namespace Module.OpenGL {
	using System.Runtime.InteropServices;
	using RPG.Engine.Graphics.Interfaces;
	using RPG.Engine.Utility;

	public class TriangleDrawer : ITriangleDrawer {

		private uint VertexArrayObject {
			get;
			set;
		}
		
		public TriangleDrawer() {

			float[] vertices = new float[] {
				-0.5f, -0.5f, 0.0f, 0.0f, 1.0f, 0.0f, 1.0f,
				0.5f, -0.5f, 0.0f, 0.0f, 0.0f, 1.0f, 1.0f,
				-0.5f, 0.5f, 0.0f, 1.0f, 1.0f, 1.0f, 1.0f,
				0.5f, 0.5f, 0.0f, 1.0f, 0.0f, 0.0f, 1.0f,
			};

			int[] indices = new int[] {
				0, 1, 2,
				2, 3, 1
			};

			this.VertexArrayObject = GL.GenVertexArray();
			uint vertexBufferObject = GL.GenBuffer();
			uint elementArrayObjet = GL.GenBuffer();
			
			GL.BindVertexArray(this.VertexArrayObject);
			
			GL.BindBuffer(GLEnum.ARRAY_BUFFER, vertexBufferObject);
			GL.BufferData(GLEnum.ARRAY_BUFFER, sizeof(float) * vertices.Length, vertices.ArrayToIntPtr(), GLEnum.STATIC_DRAW);
			
			GL.BindBuffer(GLEnum.ELEMENT_ARRAY_BUFFER, elementArrayObjet);
			GL.BufferData(GLEnum.ELEMENT_ARRAY_BUFFER, sizeof(int) * indices.Length, indices.ArrayToIntPtr(), GLEnum.STATIC_DRAW);
			
			GL.EnableVertexAttribArray(0);
			GL.EnableVertexAttribArray(1);

			int stride = 7 * sizeof(float);
			GL.VertexAttribPointer(0, 3, GLEnum.FLOAT, false, stride, IntPtr.Zero);
			GL.VertexAttribPointer(1, 4, GLEnum.FLOAT, false, stride, 3 * sizeof(float));

			GL.BindBuffer(GLEnum.ARRAY_BUFFER, 0);
			GL.BindVertexArray(0);

		}

		public void Draw() {
			GL.BindVertexArray(this.VertexArrayObject);
			GL.DrawElements(GLEnum.TRIANGLES, 6, GLEnum.UNSIGNED_INT, IntPtr.Zero);
			GL.BindVertexArray(0);
		}
	}
}