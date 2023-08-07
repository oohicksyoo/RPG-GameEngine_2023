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
			string directoryLocation = $"{Directory.GetCurrentDirectory()}/{specialFolders}";
			string path = $"{directoryLocation}{serializableAsset.AssetName}.{serializableAsset.AssetExtension}";
			Directory.CreateDirectory(directoryLocation);
			File.WriteAllText(path, data);
		}

		public void Deserialize(ISerialize serializableAsset) {
			string specialFolders = string.IsNullOrEmpty(serializableAsset.SpecialFolder) ? String.Empty : $"{serializableAsset.SpecialFolder}/";
			string directoryLocation = $"{Directory.GetCurrentDirectory()}/{specialFolders}";
			string path = $"{directoryLocation}{serializableAsset.AssetName}.{serializableAsset.AssetExtension}";
			
			JObject jObject = null;
			if (File.Exists(path)) {
				string data = File.ReadAllText(path);
				jObject = JObject.Parse(data);
			}

			//Only deserialize if the jObject was properly loaded from disk, otherwise it might not exist and ISerialize classes should self handle
			if (jObject != null) {
				serializableAsset.Deserialize(jObject);
			}
		}

		#endregion
		
	}
}