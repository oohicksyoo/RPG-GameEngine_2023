namespace RPG.Engine.Components {
	using System.Numerics;
	using Attributes;
	using Core;
	using Interfaces;
	using Newtonsoft.Json.Linq;
	using Serialization;

	[Singular]
	public class Transform : AbstractComponent {


		#region Constructor

		public Transform() : base() {
			
		}

		#endregion


		#region Properties

		[Inspector]
		public Vector2 Position {
			get;
			set;
		} = Vector2.Zero;

		[Inspector]
		public float Rotation {
			get;
			set;
		} = 0;
		
		[Inspector]
		public Vector2 Scale {
			get;
			set;
		} = Vector2.One;

		#endregion
		
		
		#region ISerialize

		public override JObject Serialize() {
			JObject jsonObject = base.Serialize();

			jsonObject[nameof(this.Position)] = this.Position.ToJObject();
			jsonObject[nameof(this.Rotation)] = this.Rotation;
			jsonObject[nameof(this.Scale)] = this.Scale.ToJObject();
			
			return jsonObject;
		}

		public override void Deserialize(JObject jsonObject) {
			this.Position = ((JObject)jsonObject[nameof(this.Position)]).FromJObject();
			this.Rotation = (float)jsonObject[nameof(this.Rotation)];
			this.Scale = ((JObject)jsonObject[nameof(this.Scale)]).FromJObject();
		}

		#endregion
		
	}
}