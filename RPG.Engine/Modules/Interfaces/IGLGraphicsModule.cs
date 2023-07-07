namespace RPG.Engine.Modules.Interfaces {
	using Graphics;

	public interface IGLGraphicsModule : IGraphicsModule {
		
		public uint CompileShader(string vertexData, string fragmentData);
		public uint CompileTexture(byte[] data, uint width, uint height, ColorType colorType, WrapModeType wrapModeType);

		public void BindTexture(uint textureID, int textureUnitOffset);
		public void DeleteTexture(uint textureID);
		
	}
}