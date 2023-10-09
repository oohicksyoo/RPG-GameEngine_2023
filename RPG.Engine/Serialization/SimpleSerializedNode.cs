namespace RPG.Engine.Serialization {
	using Interfaces;
	using Newtonsoft.Json.Linq;

	
	/// <summary>
	/// Dumb version of a Node used mainly for deleting a Node from disk without creating unneeded entries in the cache system
	/// </summary>
	public class SimpleSerializedNode : ISerialize {


		#region Constructor

		public SimpleSerializedNode(string name) {
			this.AssetName = name;
		}

		#endregion
		
		
		#region ISerialize

		public string AssetName {
			get;
			set;
		}

		public string AssetExtension => "node";
		
		public string SpecialFolder => $"Assets/Nodes";

		public JObject Serialize() {
			return new JObject();
		}

		public void Deserialize(JObject jObject) {}

		public void FileDoesntExist() {}

		#endregion
		
	}
}