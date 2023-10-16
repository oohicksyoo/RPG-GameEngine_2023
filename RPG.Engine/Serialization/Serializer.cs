namespace RPG.Engine.Serialization {
	using Core;
	using Interfaces;
	using Newtonsoft.Json;
	using Newtonsoft.Json.Linq;
	using Platform;
	using Utility;

	public class Serializer : Singleton<Serializer> {


		#region Public Methods

		public void Serialize(ISerialize serializableAsset) {
			JObject jObject = serializableAsset.Serialize();
			string data = JsonConvert.SerializeObject(jObject, Formatting.Indented);
			string specialFolders = string.IsNullOrEmpty(serializableAsset.SpecialFolder) ? String.Empty : $"{serializableAsset.SpecialFolder}/";
			string directoryLocation = $"{Directory.GetCurrentDirectory()}/{specialFolders}";
			string path = $"{directoryLocation}{serializableAsset.AssetName}.{serializableAsset.AssetExtension}";

			if (Application.Instance.PlatformType != PlatformType.Android) {
				if (!Directory.Exists(directoryLocation)) {
					Directory.CreateDirectory(directoryLocation);
				}
				File.WriteAllText(path, data);
			}
		}

		public void Deserialize(ISerialize serializableAsset) {
			string specialFolders = string.IsNullOrEmpty(serializableAsset.SpecialFolder) ? String.Empty : $"{serializableAsset.SpecialFolder}/";
			string directoryLocation = Application.Instance.PlatformType != PlatformType.Android ?
				$"{Directory.GetCurrentDirectory()}/{specialFolders}" :
				$"{specialFolders}";
			string path = $"{directoryLocation}{serializableAsset.AssetName}.{serializableAsset.AssetExtension}";
			
			JObject jObject = null;
			if (File.Exists(path)) {
				string data = File.ReadAllText(path);
				jObject = JObject.Parse(data);
			}

			//Only deserialize if the jObject was properly loaded from disk, otherwise it might not exist and ISerialize classes should self handle
			if (jObject != null) {
				serializableAsset.Deserialize(jObject);
			} else {
				Debug.Log(GetType().Name, $"Failed to find path: ({path})");
				serializableAsset.FileDoesntExist();
			}
		}

		public void Remove(ISerialize serializableAsset) {
			string specialFolders = string.IsNullOrEmpty(serializableAsset.SpecialFolder) ? String.Empty : $"{serializableAsset.SpecialFolder}/";
			string directoryLocation = $"{Directory.GetCurrentDirectory()}/{specialFolders}";
			string path = $"{directoryLocation}{serializableAsset.AssetName}.{serializableAsset.AssetExtension}";

			if (File.Exists(path)) {
				File.Delete(path);
			}
		}

		#endregion
		
	}
}