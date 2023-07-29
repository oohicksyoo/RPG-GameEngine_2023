namespace RPG.Engine.Serialization {
	using Interfaces;
	using Newtonsoft.Json;
	using Newtonsoft.Json.Linq;
	using Utility;

	public class Serializer : Singleton<Serializer> {


		#region Public Methods

		public void Serialize<T>(ISerialize<T> serializableAsset, string assetName) {
			JObject jObject = serializableAsset.Serialize();
			string data = JsonConvert.SerializeObject(jObject, Formatting.Indented);
			File.WriteAllText($"{Directory.GetCurrentDirectory()}/{assetName}.node", data);
		}

		#endregion
		
	}
}