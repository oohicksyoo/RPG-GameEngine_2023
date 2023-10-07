namespace RPG.Engine.Components {
	using System.Numerics;
	using Attributes;
	using Core;
	using Interfaces;
	using Newtonsoft.Json.Linq;
	using Serialization;
	using Utility;

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
			base.Deserialize(jsonObject);
			
			this.Position = ((JObject)jsonObject[nameof(this.Position)]).FromJObject();
			this.Rotation = (float)jsonObject[nameof(this.Rotation)];
			this.Scale = ((JObject)jsonObject[nameof(this.Scale)]).FromJObject();
		}

		#endregion

		public Matrix4x4 GetTransformMatrix() {
			Matrix4x4 modelMatrix = Matrix4x4.Identity;

			Matrix4x4 translation = Matrix4x4.CreateTranslation(new Vector3(this.Position, 0));
			Matrix4x4 rotation = Matrix4x4.CreateRotationZ(MathHelper.ToRadians(this.Rotation));
			Matrix4x4 scale = Matrix4x4.CreateScale(new Vector3(this.Scale, 1));
			modelMatrix = scale * rotation * translation;
			
			if (this.Node.Parent != null) {
				//TODO: Perhaps node should always have a reference handy to Transform to speed things up
				modelMatrix = this.Node.Parent.GetComponent<Transform>().GetTransformMatrix() * modelMatrix;
			}
			
			return modelMatrix;
		}
	}
}