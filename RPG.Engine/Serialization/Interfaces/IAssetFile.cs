namespace RPG.Engine.Serialization.Interfaces {
	
	/// <summary>
	/// Generic information about a asset file
	/// </summary>
	public interface IAssetFile {

		public string FileName {
			get;
			set;
		}

		public string AssetPath {
			get;
			set;
		}

		public string FileExtension {
			get;
		}
		
	}
}