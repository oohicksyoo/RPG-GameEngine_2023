namespace RPG.Engine.Components {
	
	using Core;
	using Interfaces;
	using Newtonsoft.Json.Linq;
	using Serialization;
	using Serialization.Interfaces;
	using Utility;

	public abstract class AbstractComponent : IComponent, IGuidDatabase {


		#region Constructor

		public AbstractComponent() {
			this.Guid = Guid.NewGuid();
			AddToGuidDatabase();
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

		public virtual void Awake() {
			//Debug.Log(GetType().Name, $"{GetType().Name} - Awake");
		}
		
		public virtual void Start() {
			//Debug.Log(GetType().Name, $"{GetType().Name} - Start");
		}
		
		public virtual void Update() {
			//Debug.Log(GetType().Name, $"{GetType().Name} - Update");
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
			RemoveFromGuidDatabase();
			
			//Guid
			this.Guid = Guid.Parse((string)jsonObject[nameof(this.Guid)]);
			
			//Readd this Node back to the database
			AddToGuidDatabase();
		}
		
		public virtual void FileDoesntExist() {
		}

		#endregion
		
		
		#region IGuidDatabase

		public void AddToGuidDatabase() {
			GuidDatabase.Instance.Add(this.Guid, this);
		}

		public void RemoveFromGuidDatabase() {
			GuidDatabase.Instance.ComponentMap.Remove(this.Guid);
		}

		#endregion
		
	}
}