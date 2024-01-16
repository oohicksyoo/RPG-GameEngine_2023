namespace RPG.Engine.Font {

	using Core;
	using FreeTypeSharp;
	using FreeTypeSharp.Native;
	using Interfaces;
	using Utility;
	using static FreeTypeSharp.Native.FT;
	
	public class FreeTypeFontLibrary : IFontLibrary {


		#region Constructor

		public FreeTypeFontLibrary() {
			this.FreeTypeLibrary = new FreeTypeLibrary();
			FT_Library_Version(this.FreeTypeLibrary.Native, out var major, out var minor, out var patch);
			Debug.Log(GetType().Name, $"FreeType version: {major}.{minor}.{patch}");
		}

		#endregion
		

		#region Properties

		private FreeTypeLibrary FreeTypeLibrary {
			get;
			set;
		}

		#endregion


		#region IFontLibrary

		public void Load(string font) {
			IntPtr face;
			FT_Error error = FT_New_Face(this.FreeTypeLibrary.Native, font, 0, out face);
			if (error != FT_Error.FT_Err_Ok) {
				throw new FreeTypeException(error);
			} else {
				Debug.Log(GetType().Name, $"({font}) font loaded");
			}

			FT_Set_Pixel_Sizes(face, 0, 48);
			
			//TODO: Using GraphicsModule aid in fully loading everything into a texture atlas
			
			//Free Everything
			FT_Done_Face(face);
		}

		public void Shutdown() {
			Debug.Log(GetType().Name, $"FreeTypeLibrary Cleanup");
			FT_Done_FreeType(this.FreeTypeLibrary.Native);
			this.FreeTypeLibrary = null;
		}

		#endregion
		
		
	}
}