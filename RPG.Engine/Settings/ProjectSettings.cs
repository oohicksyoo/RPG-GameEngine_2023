namespace RPG.Engine.Settings {
	using System.Drawing;
	using Core;
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

		public Color BackgroundColor {
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
		
		public string SpecialFolder => "Assets/Settings";
		
		public JObject Serialize() {
			JObject jsonObject = new JObject();

			jsonObject[nameof(this.Name)] = this.Name;
			jsonObject[nameof(this.StartingNode)] = this.StartingNode;
			jsonObject[nameof(this.BackgroundColor)] = this.BackgroundColor.ToJObject();

			return jsonObject;
		}

		public void Deserialize(JObject jObject) {
			this.Name = (string)jObject[nameof(this.Name)];
			this.StartingNode = (string)jObject[nameof(this.StartingNode)];

			if (jObject.ContainsKey(nameof(this.BackgroundColor))) {
				this.BackgroundColor = ((JObject)jObject[nameof(this.BackgroundColor)]).FromJObjectColor();
			} else {
				this.BackgroundColor = Color.Black;
			}
		}

		public void FileDoesntExist() {
			this.Name = Application.Instance.Project.Name;
			this.StartingNode = string.Empty;
			
			//Serialize asset with fresh stuff to disk
			Serializer.Instance.Serialize(this);
		}

		#endregion
		
		
	}
}