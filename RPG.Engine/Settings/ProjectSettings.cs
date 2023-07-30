namespace RPG.Engine.Settings {
	using Newtonsoft.Json.Linq;
	using Serialization.Interfaces;using Utility;

	[Serializable]
	public class ProjectSettings : Singleton<ProjectSettings>, ISerialize {

		#region Properties

		public string Name {
			get;
			set;
		}

		#endregion
		
		
		#region ISerialize

		public string AssetName => "ProjectSettings";

		public string AssetExtension => "settings";
		
		public string SpecialFolder => String.Empty;
		
		public JObject Serialize() {
			JObject jObject = new JObject();

			jObject["Name"] = this.Name;

			return jObject;
		}

		public void Deserialize(JObject jObject) {
			//Asset doesnt exist so lets setup defaults that can be saved
			if (jObject == null) {
				this.Name = "Project Name";
				return;
			}

			this.Name = (string)jObject["Name"];

		}

		#endregion
		
		
	}
}