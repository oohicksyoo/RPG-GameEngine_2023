namespace RPG.Engine.Serialization {
	using Interfaces;

	public class AsepriteAssetFile : IAssetFile {


		#region IAssetFile

		public string FileName {
			get;
			set;
		}

		public string AssetPath {
			get;
			set;
		}

		public string FileExtension => ".aseprite";

		#endregion


	}
}