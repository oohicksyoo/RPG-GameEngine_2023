namespace RPG.Engine.Modules.Interfaces {
	using System.Drawing;
	using System.Numerics;
	using FreeTypeSharp;
	using Graphics;
	using Graphics.Interfaces;

	public interface IGraphicsModule : IApplicationModule {

		public string Renderer {
			get;
		}

		public IBatcher Batcher {
			get;
		}
		
		public void PreRender(uint framebufferId, Color clearColor);
		
		public void PostRender();

		public Framebuffer CreateFramebuffer(Vector2 size);

		public void ResizeFramebuffer(Framebuffer framebuffer, Vector2 size);

		public void DeleteFramebuffer(Framebuffer framebuffer);

		public uint CompileShader(string vertexData, string fragmentData);

		public void UseShader(uint shaderId);

		public void SetShaderBool(uint shaderId, string propertyName, bool value);
		
		public void SetShaderInt(uint shaderId, string propertyName, int value);
		
		public void SetShaderIntArray(uint shaderId, string propertyName, int count, IntPtr value);
		
		public void SetShaderFloat(uint shaderId, string propertyName, float value);
		
		public void SetShaderVector2(uint shaderId, string propertyName, Vector2 value);
		
		public void SetShaderVector3(uint shaderId, string propertyName, Vector3 value);
		
		public void SetShaderVector4(uint shaderId, string propertyName, Vector4 value);
		
		public void SetShaderMatrix4x4(uint shaderId, string propertyName, Matrix4x4 value);
		
		public uint CompileTexture(byte[] data, uint width, uint height, ColorType colorType, WrapModeType wrapModeType);
		
		public void BindTexture(uint textureID, int textureUnitOffset);
		
		public void DeleteTexture(uint textureID);

		public void LoadFreeTypeFont(FreeTypeFaceFacade face);
	}
}