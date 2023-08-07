namespace RPG.Engine.Settings {
	using Newtonsoft.Json.Linq;
	using Serialization;
	using Serialization.Interfaces;using Utility;

	[Serializable]
	public class ProjectSettings : Singleton<ProjectSettings>, ISerialize {

		#region Properties

		public string Name {
			get;
			set;
		}

		public string StartingNode {
			get;
			set;
		}

		#endregion


		#region Constructor

		public ProjectSettings() {
			Load();
		}

		#endregion


		#region Public Methods

		public void Save() {
			Serializer.Instance.Serialize(this);
		}

		#endregion


		#region Private Methods

		private void Load() {
			Serializer.Instance.Deserialize(this);
		}

		#endregion
		
		
		#region ISerialize

		public string AssetName => "ProjectSettings";

		public string AssetExtension => "settings";
		
		public string SpecialFolder => String.Empty;
		
		public JObject Serialize() {
			JObject jsonObject = new JObject();

			jsonObject[nameof(this.Name)] = this.Name;
			jsonObject[nameof(this.StartingNode)] = this.StartingNode;

			return jsonObject;
		}

		public void Deserialize(JObject jObject) {
			//Asset doesnt exist so lets setup defaults that can be saved
			if (jObject == null) {
				this.Name = "Project Name";
				this.StartingNode = string.Empty;
				return;
			}

			this.Name = (string)jObject[nameof(this.Name)];
			this.StartingNode = (string)jObject[nameof(this.StartingNode)];

		}

		#endregion
		
		
	}
}