namespace RPG.Engine.Graphics {
	using System.Numerics;
	using Core;
	using Utility;

	public class Shader {

		#region Presets

		public static Shader DefaultAseprite => new Shader(ShaderLibrary.Aseprite);
		
		public static Shader DefaultImGui => new Shader(ShaderLibrary.ImGui);

		public static Shader DefaultSample => new Shader(ShaderLibrary.DefaultSample);
		
		public uint Id {
			get;
			private set;
		}

		#endregion


		#region Constructor

		public Shader(UncompiledShader uncompiledShader) {
			Compile(uncompiledShader.vertexShaderData, uncompiledShader.fragmentShaderData);
		}

		#endregion


		#region Public Methods

		public void Compile(string vertexData, string fragmentData) {
			this.Id = Application.Instance.GraphicsModule.CompileShader(vertexData, fragmentData);
		}
		
		public void Use() {
			Application.Instance.GraphicsModule.UseShader(this.Id);
		}

		public void EndUse() {
			Application.Instance.GraphicsModule.UseShader(0);
		}
		
		public void SetBool(string propertyName, bool value) {
			Application.Instance.GraphicsModule.SetShaderBool(this.Id, propertyName, value);
		}

		public void SetInt(string propertyName, int value) {
			Application.Instance.GraphicsModule.SetShaderInt(this.Id, propertyName, value);
		}

		public void SetIntArray(string propertyName, int count, int[] value) {
			Application.Instance.GraphicsModule.SetShaderIntArray(this.Id, propertyName, count, value);
		}
		
		public void SetFloat(string propertyName, float value) {
			Application.Instance.GraphicsModule.SetShaderFloat(this.Id, propertyName, value);
		}

		public void SetVector2(string propertyName, Vector2 value) {
			Application.Instance.GraphicsModule.SetShaderVector2(this.Id, propertyName, value);
		}
		
		public void SetVector3(string propertyName, Vector3 value) {
			Application.Instance.GraphicsModule.SetShaderVector3(this.Id, propertyName, value);
		}
		
		public void SetVector4(string propertyName, Vector4 value) {
			Application.Instance.GraphicsModule.SetShaderVector4(this.Id, propertyName, value);
		}

		public void SetMatrix4x4(string propertyName, Matrix4x4 value) {
			Application.Instance.GraphicsModule.SetShaderMatrix4x4(this.Id, propertyName, value);
		}

		#endregion

	}
}