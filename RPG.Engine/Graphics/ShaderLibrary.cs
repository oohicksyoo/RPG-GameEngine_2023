namespace RPG.Engine.Graphics {
	public static class ShaderLibrary {


		#region ImGui - DefaultAseprite

		private readonly static string VertexShaderImGui = @"#version 330 core
layout (location = 0) in vec2 vertex_position;
layout (location = 1) in vec2 vertex_texCoords;
layout (location = 2) in vec4 vertex_color;

out vec2 Vertex_TexCoords;
out vec4 Vertex_Color;

uniform mat4 projection;

void main()
{
    gl_Position = projection * vec4(vertex_position, 0.0, 1.0);
    Vertex_TexCoords = vertex_texCoords;
    Vertex_Color = vertex_color;
}";

		private readonly static string FragmentShaderImGui = @"#version 330 core
in vec2 Vertex_TexCoords;
in vec4 Vertex_Color;

out vec4 FragmentColor;

uniform sampler2D fontTexture;

void main() {
    FragmentColor = Vertex_Color * texture(fontTexture, Vertex_TexCoords);
}";
		
		public readonly static UncompiledShader ImGui = new UncompiledShader() {
			vertexShaderData = VertexShaderImGui,
			fragmentShaderData = FragmentShaderImGui
		};

		#endregion
		
		
		#region Aseprite - DefaultAseprite

		private readonly static string VertexShaderAseprite = @"#version 330 core
		layout (location = 0) in vec3 position;
		layout (location = 1) in vec2 textureCoordinate;
		layout (location = 2) in vec4 color;
		layout (location = 3) in float textureID;
		layout (location = 4) in float frame;
		layout (location = 5) in float width;
		layout (location = 6) in float frameCount;

			out vec2 TextureCoordinate;
			out vec4 Color;
			out float TextureID;
			out float Frame;
			out float Width;
			out float FrameCount;

		uniform mat4 view;
		uniform mat4 projection;

		void main() {
			//model
			gl_Position = vec4(position.xyz, 1.0);//projection * view * vec4(position.xyz, 1.0);
			TextureCoordinate = textureCoordinate;
			Color = color;
			TextureID = textureID;
			Frame = frame;
			Width = width;
			FrameCount = frameCount;
		}";

		private readonly static string FragmentShaderAseprite = @"#version 330 core
			in vec2 TextureCoordinate;
			in vec4 Color;
			in float TextureID;
			in float Frame;
			in float Width;
			in float FrameCount;

			out vec4 FragColor;

		uniform float wireframe;
		uniform sampler2D textures[32];

		void main() {
			//UV Calculation
			float textureWidth = FrameCount * Width;
			float x = ((Frame + TextureCoordinate.x) * Width) / textureWidth;
			vec2 finalUV = vec2(x, TextureCoordinate.y);

			if (wireframe < 1) {
				FragColor = texture2D(textures[int(TextureID)], finalUV) * Color;
			} else {
				FragColor = Color;
			}

			//Gamma Correction
			float gamma = 2.2;
			FragColor.rgb = pow(FragColor.rgb, vec3(1.0/gamma));
		}";
		
		public readonly static UncompiledShader Aseprite = new UncompiledShader() {
			vertexShaderData = VertexShaderAseprite,
			fragmentShaderData = FragmentShaderAseprite
		};

		#endregion
		
	}
}