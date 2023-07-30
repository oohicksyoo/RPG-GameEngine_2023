namespace RPG.Engine.Serialization {
	using Interfaces;
	using Newtonsoft.Json;
	using Newtonsoft.Json.Linq;
	using Utility;

	public class Serializer : Singleton<Serializer> {


		#region Public Methods

		public void Serialize(ISerialize serializableAsset) {
			JObject jObject = serializableAsset.Serialize();
			string data = JsonConvert.SerializeObject(jObject, Formatting.Indented);
			string specialFolders = string.IsNullOrEmpty(serializableAsset.SpecialFolder) ? String.Empty : $"{serializableAsset.SpecialFolder}/";
			File.WriteAllText($"{Directory.GetCurrentDirectory()}/{specialFolders}{serializableAsset.AssetName}.{serializableAsset.AssetExtension}", data);
		}

		public void Deserialize(ISerialize serializableAsset) {
			string specialFolders = string.IsNullOrEmpty(serializableAsset.SpecialFolder) ? String.Empty : $"{serializableAsset.SpecialFolder}/";
			string path = $"{Directory.GetCurrentDirectory()}/{specialFolders}{serializableAsset.AssetName}.{serializableAsset.AssetExtension}";
			JObject jObject = null;
			if (File.Exists(path)) {
				string data = File.ReadAllText(path);
				jObject = JObject.Parse(data);
			}
			
			serializableAsset.Deserialize(jObject);
		}

		#endregion
		
	}
}