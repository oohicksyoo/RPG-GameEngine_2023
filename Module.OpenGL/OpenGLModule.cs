using RPG.Engine.Core;
using RPG.Engine.Core.Interfaces;
using RPG.Engine.Modules.Interfaces;
using RPG.Engine.Utility;

namespace Module.OpenGL {
	using System.Drawing;
	using System.Numerics;
	using System.Runtime.InteropServices;
	using RPG.Engine.Graphics;
	using RPG.Engine.Graphics.Interfaces;
	using RPG.Engine.Modules;

	public class OpenGLModule : IModule, IGraphicsModule {


		#region NonSerialized Fields

		[NonSerialized]
		private List<Framebuffer> framebuffers;
		
		[NonSerialized]
		private List<uint> textures;

		#endregion


		#region Properties

		private List<Framebuffer> Framebuffers {
			get {
				return framebuffers ??= new List<Framebuffer>();
			}
		}
		
		private List<uint> Textures {
			get {
				return textures ??= new List<uint>();
			}
		}

		#endregion
		

		#region IGraphicsModule
		
		public string Renderer {
			get;
			private set;
		}
		
		public IBatcher Batcher {
			get;
			private set;
		}
		
		private Shader DefaultShader {
			get;
			set;
		}

		public void Initialize() {
			ISystemModule? systemModule = Application.Instance.SystemModule;
			if (systemModule == null) {
				throw new Exception($"Unable to initialize IGraphicsModule without an ISystemModule already registered");
			}
			
			GL.Initialize(systemModule.GetProcAddress);
			GL.Enable(GLEnum.DEPTH_TEST);

			this.Version = new Version(GL.MajorVersion, GL.MinorVersion);
			this.Renderer = GL.GetString(GLEnum.RENDERER);
			string version = GL.GetString(GLEnum.VERSION);
			Debug.Log(this.ModuleName, $"{this.Name} {this.Version} ({version}) ({this.Renderer})");

			IProject? project = Application.Instance.Project;
			if (project == null) {
				throw new Exception($"IProject is not initialized");
			}
			GL.Viewport(0,0, project.WindowWidth, project.WindowHeight);
			GL.Scissor(0, 0, project.WindowWidth, project.WindowHeight);

			this.Batcher = new OpenGLBatcher();
			this.Batcher.Initialize();
			
			this.DefaultShader = Shader.DefaultSample;

			//TODO: Setup window resizing
		}

		public void PreRender(uint framebufferId, Color clearColor) {
			GL.PolygonMode(GLEnum.FRONT_AND_BACK, GLEnum.FILL);
			
			Color color = clearColor;
			/*IGraphicsClear graphicsClear = Application.Instance.Get<IGraphicsClear>();
			if (graphicsClear != null) {
				color = Color.Aqua;// graphicsClear.ClearColor;
			}*/
			
			GL.BindFramebuffer(GLEnum.FRAMEBUFFER, framebufferId);
			
			GL.ClearColor(color.R / 255,color.G / 255,color.B / 255,color.A / 255);
			GL.Clear(GLEnum.COLOR_BUFFER_BIT | GLEnum.DEPTH_BUFFER_BIT);

			//Enables
			GL.BlendFunc(GLEnum.SRC_ALPHA, GLEnum.ONE_MINUS_SRC_ALPHA);
			GL.Enable(GLEnum.BLEND);
			GL.Enable(GLEnum.DEPTH_TEST);
		}

		public void PostRender() {
			//Unbind the Framebuffer used for rendering in PreRender
			GL.BindFramebuffer(GLEnum.FRAMEBUFFER, 0);
			
			//Disables
			GL.Disable(GLEnum.BLEND);
			GL.Disable(GLEnum.DEPTH_TEST);

			Vector2 size = Application.Instance.Project.WindowSize;
			GL.BindFramebuffer(GLEnum.READ_FRAMEBUFFER, Application.Instance.FinalFramebuffer.Id);
			GL.ReadBuffer(GLEnum.COLOR_ATTACHMENT0);
			GL.BindFramebuffer(GLEnum.DRAW_FRAMEBUFFER, 0);
			GL.Viewport(0, 0, (int)size.X, (int)size.Y);
			GL.BlitFramebuffer(0, 0, (int)size.X, (int)size.Y, 0, 0, (int)size.X, (int)size.Y, GLEnum.COLOR_BUFFER_BIT, GLEnum.NEAREST);
			
			GL.PolygonMode(GLEnum.FRONT_AND_BACK, GLEnum.FILL);
		}

		public Framebuffer CreateFramebuffer(Vector2 size) {

			//Setup uint references for the Framebuffer
			uint framebufferId = GL.GenFramebuffer();
			uint renderTextureId = GL.GenTexture();
			uint depthStencilBufferId = GL.GenRenderbuffer();
			
			//Setup Texture
			GL.BindTexture(GLEnum.TEXTURE_2D, renderTextureId);
			GL.TexParameteri(GLEnum.TEXTURE_2D, GLEnum.TEXTURE_MIN_FILTER, (int)GLEnum.NEAREST);
			GL.TexParameteri(GLEnum.TEXTURE_2D, GLEnum.TEXTURE_MAG_FILTER, (int)GLEnum.NEAREST);
			GL.BindTexture(GLEnum.TEXTURE_2D, 0);
			
			//Setup Framebuffer
			GL.BindFramebuffer(GLEnum.FRAMEBUFFER, framebufferId);
			GL.FramebufferTexture(GLEnum.FRAMEBUFFER, GLEnum.COLOR_ATTACHMENT0, renderTextureId, 0);
			
			//Create Framebuffer
			Framebuffer framebuffer = new Framebuffer(framebufferId, renderTextureId, depthStencilBufferId);
			ResizeFramebuffer(framebuffer, Application.Instance.Project.WindowSize);
			
			//Check over Framebuffer to make sure its all complete and setup
			/*if (GL.CheckFramebufferStatus(framebufferId) == GLEnum.FRAMEBUFFER_COMPLETE) {
				//TODO: Should set this up to double check
			}*/
			GL.BindFramebuffer(GLEnum.FRAMEBUFFER, 0);
			
			//Add Framebuffer to our list so it can be cleaned up on shutdown
			this.Framebuffers.Add(framebuffer);

			return framebuffer;
		}

		public void ResizeFramebuffer(Framebuffer framebuffer, Vector2 size) {
			//Resize Texture
			GL.BindTexture(GLEnum.TEXTURE_2D, framebuffer.RenderTextureId);
			GL.TexImage2D(GLEnum.TEXTURE_2D, 0, GLEnum.RGB, (int)size.X, (int)size.Y, 0, GLEnum.RGB, GLEnum.UNSIGNED_BYTE, 0);
			GL.BindTexture(GLEnum.TEXTURE_2D, 0);
			
			//Setup depth-stencil buffer
			GL.BindRenderbuffer(GLEnum.RENDERBUFFER, framebuffer.DepthStencilId);
			GL.RenderbufferStorage(GLEnum.RENDERBUFFER, GLEnum.DEPTH_STENCIL, (int)size.X, (int)size.Y);
			GL.BindRenderbuffer(GLEnum.RENDERBUFFER, 0);
			
			//Attach depth and stencil buffer to the framebuffer
			GL.BindFramebuffer(GLEnum.FRAMEBUFFER, framebuffer.Id);
			GL.FramebufferRenderbuffer(GLEnum.FRAMEBUFFER, GLEnum.DEPTH_ATTACHMENT, GLEnum.RENDERBUFFER, framebuffer.DepthStencilId);
			GL.FramebufferRenderbuffer(GLEnum.FRAMEBUFFER, GLEnum.STENCIL_ATTACHMENT, GLEnum.RENDERBUFFER, framebuffer.DepthStencilId);
			GL.BindFramebuffer(GLEnum.FRAMEBUFFER, 0);
			
		}

		public void DeleteFramebuffer(Framebuffer framebuffer) {
			GL.DeleteBuffer(framebuffer.Id);
			GL.DeleteTexture(framebuffer.RenderTextureId);
			GL.DeleteRenderbuffer(framebuffer.DepthStencilId);
		}

		public uint CompileShader(string vertexData, string fragmentData) {
			uint vertex, fragment, shaderProgram;
			int success;

			vertex = GL.CreateShader(GLEnum.VERTEX_SHADER);
			GL.ShaderSource(vertex, 1, new []{vertexData}, new int[] {vertexData.Length});
			GL.CompileShader(vertex);
			GL.GetShaderIV(vertex, GLEnum.COMPILE_STATUS, out success);
			if (success == 0) {
				string? value = GL.GetShaderInfoLog(vertex);
				if (value != null) {
					throw new Exception($"Failed to compile vertex shader: {value}");
				}
				
				throw new Exception($"Failed to compile vertex shader: No Log Available");
			}
			
			fragment = GL.CreateShader(GLEnum.FRAGMENT_SHADER);
			GL.ShaderSource(fragment, 1, new []{fragmentData}, new int[] {fragmentData.Length});
			GL.CompileShader(fragment);
			GL.GetShaderIV(fragment, GLEnum.COMPILE_STATUS, out success);
			if (success == 0) {
				string? value = GL.GetShaderInfoLog(fragment);
				if (value != null) {
					throw new Exception($"Failed to compile fragment shader: {value}");
				}
				
				throw new Exception($"Failed to compile fragment shader: No Log Available");
			}
			
			shaderProgram = GL.CreateProgram();
			GL.AttachShader(shaderProgram, vertex);
			GL.AttachShader(shaderProgram, fragment);
			GL.LinkProgram(shaderProgram);
			GL.GetProgramIV(shaderProgram, GLEnum.LINK_STATUS, out success);
			if (success == 0) {
				string? value = GL.GetProgramInfoLog(shaderProgram);
				if (value != null) {
					throw new Exception($"Failed to compile shader: {value}");
				}
				
				throw new Exception($"Failed to compile shader: No Log Available");
			}
			
			GL.DeleteShader(vertex);
			GL.DeleteShader(fragment);

			return shaderProgram;
		}

		public void UseShader(uint shaderId) {
			GL.UseProgram(shaderId);
		}

		public void SetShaderBool(uint shaderId, string propertyName, bool value) {
			int val = value ? 1 : 0;
			GL.Uniform1i(GL.GetUniformLocation(shaderId, propertyName), val);
		}
		
		public void SetShaderInt(uint shaderId, string propertyName, int value) {
			GL.Uniform1i(GL.GetUniformLocation(shaderId, propertyName), value);
		}

		public void SetShaderIntArray(uint shaderId, string propertyName, int count, IntPtr value) {
			GL.Uniform1iv(GL.GetUniformLocation(shaderId, propertyName), count, value);
		}
		
		public void SetShaderFloat(uint shaderId, string propertyName, float value) {
			GL.Uniform1f(GL.GetUniformLocation(shaderId, propertyName), value);
		}
		
		public void SetShaderVector2(uint shaderId, string propertyName, Vector2 value) {
			GL.Uniform2f(GL.GetUniformLocation(shaderId, propertyName), value.X, value.Y);
		}
		
		public void SetShaderVector3(uint shaderId, string propertyName, Vector3 value) {
			GL.Uniform3f(GL.GetUniformLocation(shaderId, propertyName), value.X, value.Y, value.Z);
		}
		
		public void SetShaderVector4(uint shaderId, string propertyName, Vector4 value) {
			GL.Uniform4f(GL.GetUniformLocation(shaderId, propertyName), value.X, value.Y, value.Z, value.W);
		}
		
		public unsafe void SetShaderMatrix4x4(uint shaderId, string propertyName, Matrix4x4 value) {
			float[] matrix4x4Array = value.ToFloatArray();
			fixed (float* ptr = matrix4x4Array) {
				GL.UniformMatrix4fv(GL.GetUniformLocation(shaderId, propertyName), 1, false, new IntPtr(ptr));
			}
		}

		public uint CompileTexture(byte[] data, uint width, uint height, ColorType colorType, WrapModeType wrapModeType) {
			uint t = GL.GenTexture();
			GL.BindTexture(GLEnum.TEXTURE_2D, t);

			int wrapMode = (wrapModeType == WrapModeType.ClampToEdge) ? (int) GLEnum.CLAMP_TO_EDGE : (int) GLEnum.REPEAT;
			
			GL.TexParameteri(GLEnum.TEXTURE_2D, GLEnum.TEXTURE_WRAP_S, wrapMode);
			GL.TexParameteri(GLEnum.TEXTURE_2D, GLEnum.TEXTURE_WRAP_T, wrapMode);
			GL.TexParameteri(GLEnum.TEXTURE_2D, GLEnum.TEXTURE_MIN_FILTER, (int)GLEnum.LINEAR);
			GL.TexParameteri(GLEnum.TEXTURE_2D, GLEnum.TEXTURE_MAG_FILTER, (int)GLEnum.LINEAR);
			
			int sizeOfColor = Marshal.SizeOf<byte>();
			IntPtr textureData = Marshal.AllocHGlobal(sizeOfColor * data.Length);
			Marshal.Copy(data, 0, textureData, data.Length);
			GLEnum format = (colorType == ColorType.RGB) ? GLEnum.RGB : GLEnum.RGBA;
			GL.TexImage2D(GLEnum.TEXTURE_2D, 0, format, (int)width, (int)height, 0, format, GLEnum.UNSIGNED_BYTE, textureData);
			
			this.Textures.Add(t);
			
			return t;
		}

		public void BindTexture(uint textureID, int textureUnitOffset) {
			GL.ActiveTexture((uint) GLEnum.TEXTURE0 + (uint)textureUnitOffset);
			GL.BindTexture(GLEnum.TEXTURE_2D, textureID);
		}

		public void DeleteTexture(uint textureID) {
			GL.DeleteTexture(textureID);
		}

		#endregion


		#region IModule

		public string ModuleName => GetType().Name;
		
		public string Name => $"OpenGL";

		public Version Version {
			get;
			private set;
		} = new Version();
		
		public int Priority => int.MaxValue - 2;

		public void Awake() {
			
		}

		public void Start() {
			
		}

		public void Update() {
			
		}

		public void Shutdown() {
			Debug.Log(this.ModuleName, $"Shutdown");
			this.Batcher.Shutdown();
			
			foreach (Framebuffer framebuffer in this.Framebuffers) {
				DeleteFramebuffer(framebuffer);
			}

			foreach (uint texture in this.Textures) {
				DeleteTexture(texture);
			}
		}

		#endregion
		
	}
}