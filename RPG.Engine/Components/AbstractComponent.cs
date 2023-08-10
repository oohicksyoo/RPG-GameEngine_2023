namespace RPG.Engine.Components {
	using Core;
	using Interfaces;
	using Newtonsoft.Json.Linq;
	using Serialization;

	public abstract class AbstractComponent : IComponent {


		#region Constructor

		public AbstractComponent() {
			//TODO: Be able to pass in guid
			this.Guid = Guid.NewGuid();
			GuidDatabase.Instance.ComponentMap.Add(this.Guid, this);
		}

		#endregion
		

		#region IComponent

		public Node Node {
			get;
			set;
		}
		
		public Guid Guid {
			get;
			private set;
		}

		#endregion


		#region ISerialize

		/// <summary>
		/// File name
		/// </summary>
		public string AssetName => GetType().Name;

		/// <summary>
		/// File extension of this asset; the . is not needed
		/// </summary>
		public string AssetExtension => "component";

		/// <summary>
		/// Any special folder structure between the exe and the asset
		/// </summary>
		public string SpecialFolder => string.Empty;

		public virtual JObject Serialize() {
			JObject jsonObject = new JObject();

			//Guid
			jsonObject[nameof(this.Guid)] = this.Guid;
			
			//Type
			jsonObject["Type"] = GetType().AssemblyQualifiedName;

			return jsonObject;
		}

		public virtual void Deserialize(JObject jsonObject) {
			//Remove Node From NodeDatabase
			GuidDatabase.Instance.ComponentMap.Remove(this.Guid);
			
			//Guid
			this.Guid = Guid.Parse((string)jsonObject[nameof(this.Guid)]);
			
			//Readd this Node back to the database
			GuidDatabase.Instance.ComponentMap.Add(this.Guid, this);
		}

		#endregion
		
	}
}