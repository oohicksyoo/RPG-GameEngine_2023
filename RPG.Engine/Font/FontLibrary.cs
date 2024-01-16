namespace RPG.Engine.Font {
	using Interfaces;
	using Utility;

	/// <summary>
	/// Handles loading different font libraries and loading in fonts themselves
	/// </summary>
	public class FontLibrary : Singleton<FontLibrary> {


		#region Properties

		private bool HasInitialized {
			get;
			set;
		}

		private List<IFontLibrary> FontLibraries {
			get;
			set;
		}

		#endregion
		

		#region Public Methods

		public void Initialize() {
			if (this.HasInitialized) {
				return;
			}

			this.FontLibraries = new List<IFontLibrary>();
			this.FontLibraries.Add(new FreeTypeFontLibrary());

			this.HasInitialized = true;
		}

		public void Shutdown() {
			foreach (IFontLibrary library in this.FontLibraries) {
				library.Shutdown();
			}
		}

		public void LoadFont<T>(string font) where T : IFontLibrary {
			foreach (IFontLibrary library in this.FontLibraries) {
				if (library is T) {
					library.Load(font);		
				}
			}
		}

		#endregion
		
	}
}