namespace Module.OpenGL {

	using System.Numerics;
	using System.Runtime.InteropServices;
	using RPG.Engine.Components;
	using RPG.Engine.Components.Interfaces;
	using RPG.Engine.Core;
	using RPG.Engine.Graphics;
	using RPG.Engine.Graphics.Interfaces;
	using RPG.Engine.Utility;

	public class OpenGLBatcher : IBatcher {


		#region Constants

		private const int MAX_TEXTURE_COUNT = 32;

		private const int MAX_QUAD_COUNT = 1000;

		private const int MAX_VERTEX_COUNT = MAX_QUAD_COUNT * 4;

		private const int MAX_INDICE_COUNT = MAX_QUAD_COUNT * 6;

		private const int SHADER_PROPERTIES_COUNT = 13;

		private const int MAX_DATA_ARRAY = MAX_VERTEX_COUNT * SHADER_PROPERTIES_COUNT;

		#endregion
		

		#region Properties

		private uint[] TextureIds {
			get;
			set;
		}

		private uint WhiteTexture {
			get;
			set;
		}
		
		private uint VertexArrayObject {
			get;
			set;
		}
		
		private uint VertexBufferObject {
			get;
			set;
		}

		private int VertexStride {
			get;
			set;
		}

		private float[] DataArray {
			get;
			set;
		}

		/// <summary>
		/// Current index we are at in the VertexDataArray
		/// </summary>
		private int VertexDataArrayIndex {
			get;
			set;
		}

		private int VertexDataArrayInternalCount {
			get;
			set;
		}

		//Current index we are at in the TextureIds
		private int TextureSlotIndex {
			get;
			set;
		}

		#endregion
		

		#region IBatcher
		
		public int DrawCount {
			get;
			private set;
		}
		
		public int QuadCount {
			get;
			private set;
		}
		
		public int IndiceCount {
			get;
			private set;
		}

		public void Initialize() {
			this.DataArray = new float[MAX_DATA_ARRAY];
			this.TextureIds = new uint[MAX_TEXTURE_COUNT];
			this.VertexDataArrayIndex = 0;
			this.VertexDataArrayInternalCount = 0;
			this.TextureSlotIndex = 1;
			
			//Create Vertex Array
			this.VertexArrayObject = GL.GenVertexArray();
			this.VertexBufferObject = GL.GenBuffer();
			uint elementArrayObject = GL.GenBuffer();
			
			//Bind Vertex Array
			GL.BindVertexArray(this.VertexArrayObject);

			//Setup Vertex Buffer
			this.VertexStride = sizeof(float) * SHADER_PROPERTIES_COUNT;
			GL.BindBuffer(GLEnum.ARRAY_BUFFER, this.VertexBufferObject);
			GL.BufferData(GLEnum.ARRAY_BUFFER, this.VertexStride * MAX_VERTEX_COUNT, IntPtr.Zero, GLEnum.DYNAMIC_DRAW);
			
			//Setup Attributes
			GL.EnableVertexAttribArray(0);
			GL.EnableVertexAttribArray(1);
			GL.EnableVertexAttribArray(2);
			GL.EnableVertexAttribArray(3);
			GL.EnableVertexAttribArray(4);
			GL.EnableVertexAttribArray(5);
			GL.EnableVertexAttribArray(6);
			
			GL.VertexAttribPointer(0, 3, GLEnum.FLOAT, false, this.VertexStride, IntPtr.Zero);
			GL.VertexAttribPointer(1, 2, GLEnum.FLOAT, false, this.VertexStride, sizeof(float)*(3+0));
			GL.VertexAttribPointer(2, 4, GLEnum.FLOAT, false, this.VertexStride, sizeof(float)*(3+2));
			GL.VertexAttribPointer(3, 1, GLEnum.FLOAT, false, this.VertexStride, sizeof(float)*(3+2+4));
			GL.VertexAttribPointer(4, 1, GLEnum.FLOAT, false, this.VertexStride, sizeof(float)*(3+2+4+1));
			GL.VertexAttribPointer(5, 1, GLEnum.FLOAT, false, this.VertexStride, sizeof(float)*(3+2+4+1+1));
			GL.VertexAttribPointer(6, 1, GLEnum.FLOAT, false, this.VertexStride, sizeof(float)*(3+2+4+1+1+1));
			
			//Indice Buffer Setup
			int[] indices = new int[MAX_INDICE_COUNT];
			int offset = 0;
			for (int i = 0; i < MAX_INDICE_COUNT; i += 6) {
				indices[i + 0] = 0 + offset;
				indices[i + 1] = 1 + offset;
				indices[i + 2] = 2 + offset;

				indices[i + 3] = 2 + offset;
				indices[i + 4] = 3 + offset;
				indices[i + 5] = 1 + offset;
				
				//0,1,2
				//2,3,1

				offset += 4;
			}

			IntPtr indiceIntPtr = indices.ArrayToIntPtr();
			GL.BindBuffer(GLEnum.ELEMENT_ARRAY_BUFFER, elementArrayObject);
			GL.BufferData(GLEnum.ELEMENT_ARRAY_BUFFER, sizeof(int) * indices.Length, indiceIntPtr, GLEnum.STATIC_DRAW);
			indiceIntPtr.FreeArrayIntPtr();

			//Reset binds
			GL.BindBuffer(GLEnum.ARRAY_BUFFER, 0);
			GL.BindVertexArray(0);
			
			//Create White Texture
			this.WhiteTexture = Application.Instance.GraphicsModule.CompileTexture(new byte[] { 255, 255, 255, 255 }, 1, 1, ColorType.RGBA, WrapModeType.ClampToEdge);
			for (int i = 0; i < MAX_TEXTURE_COUNT; i++) {
				this.TextureIds[i] = this.WhiteTexture;
			}
		}

		public void Shutdown() {
			GL.DeleteVertexArray(this.VertexArrayObject);
			GL.DeleteBuffer(this.VertexBufferObject);
			
			//Remove white texture
			GL.DeleteTexture(this.WhiteTexture);
		}

		/// <summary>
		/// Initial call per frame to get the Batcher ready for... well batching
		/// </summary>
		public void Begin() {
			this.DrawCount = 0;
			this.QuadCount = 0;
			this.IndiceCount = 0;
			
			Reset();
		}

		/// <summary>
		/// Current batcher is full or rendering for the frame is complete
		/// Will flush to screen when complete
		/// </summary>
		public void End() {
			if (this.IndiceCount == 0) {
				Reset();
				return; //nothing to draw
			}

			IntPtr dataArrayIntPtr = this.DataArray.ArrayToIntPtr();
			GL.BindBuffer(GLEnum.ARRAY_BUFFER, this.VertexBufferObject);
			GL.BufferSubData(GLEnum.ARRAY_BUFFER, 0, this.VertexStride * this.VertexDataArrayIndex, dataArrayIntPtr);
			dataArrayIntPtr.FreeArrayIntPtr();

			//Always flush the end results
			Flush();

			//Reset Properties
			Reset();
		}

		/// <summary>
		/// Add renderable to the batcher for drawing to the screen
		/// </summary>
		public void Draw(IComponentRenderable renderable) {
			if (!renderable.CanRender || renderable.Mesh == null) {
				return;
			}

			uint textureId = renderable.TextureId;
			Mesh mesh = renderable.Mesh;
			
			//Check if we surpass anything for this batch
			MaxTextureCheck();
			MaxIndiceCheck(mesh.IndiceCount);
			
			//Add Texture to the texture list
			//TODO: FUTURE: Support multiple textures for a renderable
			float textureIndex = 0f;
			for (int i = 0; i < this.TextureSlotIndex; i++) {
				//Texture already exists in this batch
				if (this.TextureIds[i] == textureId) {
					textureIndex = (float)i;
					break;
				}
				
				//New texture not in the batch
				if (textureIndex == 0) {
					textureIndex = this.TextureSlotIndex;
					this.TextureIds[this.TextureSlotIndex] = textureId;
					this.TextureSlotIndex++;
				}
			}
			
			//Apply Shader Properties
			Vector3 position = Vector3.Zero;
			foreach (Vertex vertex in mesh.Vertices) {
				position = Vector3.Transform(vertex.Position, renderable.Transform.LocalToWorld());
				this.DataArray[this.VertexDataArrayInternalCount + 0] = position.X;
				this.DataArray[this.VertexDataArrayInternalCount + 1] = position.Y;
				this.DataArray[this.VertexDataArrayInternalCount + 2] = position.Z;
				this.DataArray[this.VertexDataArrayInternalCount + 3] = vertex.TextureCoordinate.X;
				this.DataArray[this.VertexDataArrayInternalCount + 4] = vertex.TextureCoordinate.Y;
				this.DataArray[this.VertexDataArrayInternalCount + 5] = vertex.Color.X;
				this.DataArray[this.VertexDataArrayInternalCount + 6] = vertex.Color.Y;
				this.DataArray[this.VertexDataArrayInternalCount + 7] = vertex.Color.Z;
				this.DataArray[this.VertexDataArrayInternalCount + 8] = vertex.Color.W;
				this.DataArray[this.VertexDataArrayInternalCount + 9] = textureIndex;
				this.DataArray[this.VertexDataArrayInternalCount + 10] = (float)renderable.CurrentFrame;
				this.DataArray[this.VertexDataArrayInternalCount + 11] = renderable.SingleFrameWidth;
				this.DataArray[this.VertexDataArrayInternalCount + 12] = renderable.TotalFrameCount;
				this.VertexDataArrayIndex++;
				this.VertexDataArrayInternalCount += SHADER_PROPERTIES_COUNT;
			}
			
			//Update properties
			this.IndiceCount += 6;
			this.QuadCount++;
		}

		#endregion


		#region Private Methods

		/// <summary>
		/// Push this batch to the screen
		/// </summary>
		private void Flush() {
			//Bind textures
			for (int i = 0; i < this.TextureSlotIndex; i++) {
				GL.BindTextureUnit((uint)i, this.TextureIds[i]);
			}
			
			//Draw Elements
			GL.BindVertexArray(this.VertexArrayObject);
			GL.DrawElements(GLEnum.TRIANGLES, this.IndiceCount, GLEnum.UNSIGNED_INT, IntPtr.Zero);
			GL.BindVertexArray(0);
			
			//Update Draw Count
			this.DrawCount++;
		}

		/// <summary>
		/// Reset things back to be ready
		/// </summary>
		private void Reset() {
			this.VertexDataArrayIndex = 0;
			this.VertexDataArrayInternalCount = 0;
			this.TextureSlotIndex = 1;
		}

		private void MaxTextureCheck() {
			//TODO: Support multi texture files?
			//TODO: Support looking up in the dictionary if our texture uint already exists in which case we could render
			
			if (this.TextureSlotIndex + 1 >= MAX_TEXTURE_COUNT) {
				End();
			}
		}

		private void MaxIndiceCheck(int indiceCount) {
			if (this.IndiceCount + indiceCount >= MAX_INDICE_COUNT) {
				End();
			}
		}

		#endregion
		
	}
}