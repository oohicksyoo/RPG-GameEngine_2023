namespace RPG.Engine.Graphics {
	public static class ShaderLibrary {


		#region ImGui - Default

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
		
	}
}