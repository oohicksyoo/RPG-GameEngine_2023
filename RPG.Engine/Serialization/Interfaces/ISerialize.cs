namespace RPG.Engine.Serialization.Interfaces {
	using Newtonsoft.Json.Linq;

	public interface ISerialize {
		
		/// <summary>
		/// File name
		/// </summary>
		public string AssetName {
			get;
		}
		
		/// <summary>
		/// File extension of this asset; the . is not needed
		/// </summary>
		public string AssetExtension {
			get;
		}

		/// <summary>
		/// Any special folder structure between the exe and the asset
		/// </summary>
		public string SpecialFolder {
			get;
		}
		
		public JObject Serialize();

		public void Deserialize(JObject jObject);

	}
}