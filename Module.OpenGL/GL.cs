using System.Runtime.InteropServices;

namespace Module.OpenGL {
	public static class GL {
		
		#region Private Static Variables

		private static GLBindings bindings;
		private static int majorVersion;
		private static int minorVersion;
		private static int maxVertexAttributes;

		#endregion
		

		#region Properties

		public static int MajorVersion {
			get {
				return majorVersion;
			}
		}
		
		public static int MinorVersion {
			get {
				return minorVersion;
			}
		}

		public static int MaxVertexAttributes {
			get {
				return maxVertexAttributes;
			}
		}

		#endregion


		#region Initialization

		public static void Initialize(Func<string, IntPtr> getProcAddressFunc) {
			bindings = new GLBindings(getProcAddressFunc);
			
			GetIntegerv((GLEnum)0x821B, out majorVersion);
			GetIntegerv((GLEnum)0x821C, out minorVersion);
			GetIntegerv(GLEnum.MAX_VERTEX_ATTRIBS, out maxVertexAttributes);
		}

		#endregion
		
		
		#region Bindings
		
		public static unsafe string GetString(GLEnum name) {
			return Marshal.PtrToStringAnsi(bindings.glGetString(name)) ?? "";
		}
		
		public static void GetIntegerv(GLEnum name, out int data) => bindings.glGetIntegerv(name, out data);
		
		public static void GetShaderIV(uint shader, GLEnum pname, out int result) => bindings.glGetShaderiv(shader, pname, out result);
		
		public static void Enable(GLEnum mode) => bindings.glEnable(mode);
		
		public static void Disable(GLEnum mode) => bindings.glDisable(mode);
		
		public static void BlendEquation(GLEnum eq) => bindings.glBlendEquation(eq);
		
		public static void BlendFunc(GLEnum sfactor, GLEnum dfactor) => bindings.glBlendFunc(sfactor, dfactor);
		
		public static void Scissor(int x, int y, int width, int height) => bindings.glScissor(x, y, width, height);
		
		public static unsafe string? GetShaderInfoLog(uint shader) {
			bindings.glGetShaderiv(shader, (GLEnum)0x8B84, out int len);

			char* bytes = stackalloc char[len];
			IntPtr ptr = new IntPtr(bytes);

			bindings.glGetShaderInfoLog(shader, len, out len, ptr);

			if (len <= 0) {
				return null;
			}

			return Marshal.PtrToStringAnsi(ptr, len);
		}
		
		public static void GetProgramIV(uint program, GLEnum pname, out int result) => bindings.glGetProgramiv(program, pname, out result);

		public static unsafe string? GetProgramInfoLog(uint program) {
			bindings.glGetProgramiv(program, (GLEnum)0x8B84, out int len);

			char* bytes = stackalloc char[len];
			IntPtr ptr = new IntPtr(bytes);

			bindings.glGetProgramInfoLog(program, len, out len, ptr);

			if (len <= 0) {
				return null;
			}

			return Marshal.PtrToStringAnsi(ptr, len);
		}
		
		public static void BindFramebuffer(GLEnum target, uint id) => bindings.glBindFramebuffer(target, id);
		
		public static unsafe uint GenFramebuffer() {
			uint id;
			bindings.glGenFramebuffers(1, new IntPtr(&id));
			return id;
		}
		
		public static void FramebufferTexture2D(GLEnum target, GLEnum attachment, GLEnum textarget, uint texture, int level) => bindings.glFramebufferTexture2D(target, attachment, textarget, texture, level);
		
		public static unsafe void DeleteFramebuffer(uint id) {
			bindings.glDeleteFramebuffers(1, &id);
		}
		
		public static unsafe uint GenRenderbuffer() {
			uint id;
			bindings.glGenRenderbuffers(1, new IntPtr(&id));
			return id;
		}
		
		public static void BindRenderbuffer(GLEnum target, uint id) => bindings.glBindRenderbuffer(target, id);
		
		public static void FramebufferRenderbuffer(GLEnum target, GLEnum attachment, GLEnum renderbuffertarget, uint renderbuffer) => bindings.glFramebufferRenderbuffer(target, attachment, renderbuffertarget, renderbuffer);
		
		public static void RenderbufferStorage(GLEnum target, GLEnum internalformat, int width, int height) => bindings.glRenderbufferStorage(target, internalformat, width, height);
		
		public static void Viewport(int x, int y, int width, int height) => bindings.glViewport(x, y, width, height);
		
		public static void Clear(GLEnum mask) => bindings.glClear(mask);
		
		public static void ClearColor(float red, float green, float blue, float alpha) => bindings.glClearColor(red, green, blue, alpha);
		
		public static void EnableVertexAttribArray(uint location) => bindings.glEnableVertexAttribArray(location);

		public static void DisableVertexAttribArray(uint location) => bindings.glDisableVertexAttribArray(location);

		public static void VertexAttribPointer(uint index, int size, GLEnum type, bool normalized, int stride, IntPtr pointer) => bindings.glVertexAttribPointer(index, size, type, normalized, stride, pointer);

		public static void VertexAttribDivisor(uint index, uint divisor) => bindings.glVertexAttribDivisor(index, divisor);

		public static uint CreateShader(GLEnum type) => bindings.glCreateShader(type);

		public static void AttachShader(uint program, uint shader) => bindings.glAttachShader(program, shader);

		public static void DetachShader(uint program, uint shader) => bindings.glDetachShader(program, shader);

		public static void DeleteShader(uint shader) => bindings.glDeleteShader(shader);

		public static void ShaderSource(uint shader, int count, string[] source, int[] length) => bindings.glShaderSource(shader, count, source, length);

		public static void CompileShader(uint shader) => bindings.glCompileShader(shader);
		
		public static uint CreateProgram() => bindings.glCreateProgram();
		
		public static void DeleteProgram(uint program) => bindings.glDeleteProgram(program);

		public static void LinkProgram(uint program) => bindings.glLinkProgram(program);
		
		public static void UseProgram(uint program) => bindings.glUseProgram(program);
		
		public static void BindVertexArray(uint id) => bindings.glBindVertexArray(id);

		public static void DrawArrays(GLEnum mode, int first, int count) => bindings.glDrawArrays(mode, first, count);
		
		public static unsafe void DeleteVertexArray(uint id) {
			bindings.glDeleteVertexArrays(1, &id);
		}
		
		public static void DrawElements(GLEnum mode, int count, GLEnum type, IntPtr indices) => bindings.glDrawElements(mode, count, type, indices);
		
		public static void DrawElementsInstanced(GLEnum mode, int count, GLEnum type, IntPtr indices, int amount) => bindings.glDrawElementsInstanced(mode, count, type, indices, amount);

		public static void DrawElementsBaseVertex(GLEnum mode, int count, GLEnum type, IntPtr indices, int amount) => bindings.glDrawElementsBaseVertex(mode, count, type, indices, amount);
		
		public static void GenTextures(int n, IntPtr textures) => bindings.glGenTextures(n, textures);

		public static unsafe uint GenTexture()
		{
			uint id;
			bindings.glGenTextures(1, new IntPtr(&id));
			return id;
		}
		
		public static void ActiveTexture(uint id) => bindings.glActiveTexture(id);
		
		public static void BindTexture(GLEnum target, uint id) => bindings.glBindTexture(target, id);
		
		public static unsafe void DeleteTexture(uint id) {
			bindings.glDeleteTextures(1, &id);
		}
		
		public static void TexParameteri(GLEnum target, GLEnum name, int param) => bindings.glTexParameteri(target, name, param);
		
		public static unsafe uint GenVertexArray()
		{
			uint id;
			bindings.glGenVertexArrays(1, &id);
			return id;
		}
		
		public static unsafe uint GenBuffer() {
			uint id;
			bindings.glGenBuffers(1, &id);
			return id;
		}
		
		public static unsafe void DeleteBuffer(uint id) {
			bindings.glDeleteBuffers(1, &id);
		}

		public static unsafe void DrawBuffer(GLEnum buf) => bindings.glDrawBuffer(buf);
		
		public static unsafe void ReadBuffer(GLEnum buf) => bindings.glReadBuffer(buf);
		
		public static void TexImage2D(GLEnum target, int level, GLEnum internalFormat, int width, int height, int border, GLEnum format, GLEnum type, IntPtr data) => bindings.glTexImage2D(target, level, internalFormat, width, height, border, format, type, data);
		
		public static void BindBuffer(GLEnum target, uint buffer) => bindings.glBindBuffer(target, buffer);
		
		public static void BufferData(GLEnum target, IntPtr size, IntPtr data, GLEnum usage) => bindings.glBufferData(target, size, data, usage);
		
		public static void BufferSubData(GLEnum target, IntPtr offset, IntPtr size, IntPtr data) => bindings.glBufferSubData(target, offset, size, data);

		public static void PolygonMode(GLEnum face, GLEnum mode) => bindings.glPolygonMode(face, mode);
		
		public static int GetUniformLocation(uint program, string name) => bindings.glGetUniformLocation(program, name);

		public static int GetAttribLocation(uint program, string name) => bindings.glGetAttribLocation(program, name);

		public static void Uniform1f(int location, float v0) => bindings.glUniform1f(location, v0);

        public static void Uniform2f(int location, float v0, float v1) => bindings.glUniform2f(location, v0, v1);

        public static void Uniform3f(int location, float v0, float v1, float v2) => bindings.glUniform3f(location, v0, v1, v2);

        public static void Uniform4f(int location, float v0, float v1, float v2, float v3) => bindings.glUniform4f(location, v0, v1, v2, v3);

        public static void Uniform1fv(int location, int count, IntPtr value) => bindings.glUniform1fv(location, count, value);

        public static void Uniform2fv(int location, int count, IntPtr value) => bindings.glUniform2fv(location, count, value);

        public static void Uniform3fv(int location, int count, IntPtr value) => bindings.glUniform3fv(location, count, value);

        public static void Uniform4fv(int location, int count, IntPtr value) => bindings.glUniform4fv(location, count, value);

        public static void Uniform1i(int location, int v0) => bindings.glUniform1i(location, v0);

        public static void Uniform2i(int location, int v0, int v1) => bindings.glUniform2i(location, v0, v1);

        public static void Uniform3i(int location, int v0, int v1, int v2) => bindings.glUniform3i(location, v0, v1, v2);

        public static void Uniform4i(int location, int v0, int v1, int v2, int v3) => bindings.glUniform4i(location, v0, v1, v2, v3);

        public static void Uniform1iv(int location, int count, IntPtr value) => bindings.glUniform1iv(location, count, value);

        public static void Uniform2iv(int location, int count, IntPtr value) => bindings.glUniform2iv(location, count, value);

        public static void Uniform3iv(int location, int count, IntPtr value) => bindings.glUniform3iv(location, count, value);

        public static void Uniform4iv(int location, int count, IntPtr value) => bindings.glUniform4iv(location, count, value);

        public static void Uniform1ui(int location, uint v0) => bindings.glUniform1ui(location, v0);

        public static void Uniform2ui(int location, uint v0, uint v1) => bindings.glUniform2ui(location, v0, v1);

        public static void Uniform3ui(int location, uint v0, uint v1, uint v2) => bindings.glUniform3ui(location, v0, v1, v2);

        public static void Uniform4ui(int location, uint v0, uint v1, uint v2, uint v3) => bindings.glUniform4ui(location, v0, v1, v2, v3);

        public static void Uniform1uiv(int location, int count, IntPtr value) => bindings.glUniform1uiv(location, count, value);

        public static void Uniform2uiv(int location, int count, IntPtr value) => bindings.glUniform2uiv(location, count, value);

        public static void Uniform3uiv(int location, int count, IntPtr value) => bindings.glUniform3uiv(location, count, value);

        public static void Uniform4uiv(int location, int count, IntPtr value) => bindings.glUniform4uiv(location, count, value);

        public static void UniformMatrix2fv(int location, int count, bool transpose, IntPtr value) => bindings.glUniformMatrix2fv(location, count, transpose, value);

        public static void UniformMatrix3fv(int location, int count, bool transpose, IntPtr value) => bindings.glUniformMatrix3fv(location, count, transpose, value);

        public static void UniformMatrix4fv(int location, int count, bool transpose, IntPtr value) => bindings.glUniformMatrix4fv(location, count, transpose, value);

        public static void UniformMatrix2x3fv(int location, int count, bool transpose, IntPtr value) => bindings.glUniformMatrix2x3fv(location, count, transpose, value);

        public static void UniformMatrix3x2fv(int location, int count, bool transpose, IntPtr value) => bindings.glUniformMatrix3x2fv(location, count, transpose, value);

        public static void UniformMatrix2x4fv(int location, int count, bool transpose, IntPtr value) => bindings.glUniformMatrix2x4fv(location, count, transpose, value);

        public static void UniformMatrix4x2fv(int location, int count, bool transpose, IntPtr value) => bindings.glUniformMatrix4x2fv(location, count, transpose, value);

        public static void UniformMatrix3x4fv(int location, int count, bool transpose, IntPtr value) => bindings.glUniformMatrix3x4fv(location, count, transpose, value);

        public static void UniformMatrix4x3fv(int location, int count, bool transpose, IntPtr value) => bindings.glUniformMatrix4x3fv(location, count, transpose, value);


		#endregion

		
	}
}