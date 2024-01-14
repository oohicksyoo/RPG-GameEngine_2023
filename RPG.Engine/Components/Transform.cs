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


		#region Public Methods

		/// <summary>
		/// Model Matrix
		/// </summary>
		public Matrix4x4 LocalToParent() {

			float z = 0;
			AsepriteComponent asepriteComponent = this.Node.GetComponent<AsepriteComponent>();
			if (asepriteComponent != null) {
				z = asepriteComponent.Layer;
			}
			
			Matrix4x4 translation = Matrix4x4.CreateTranslation(new Vector3(this.Position, z));
			Matrix4x4 rotation = Matrix4x4.CreateFromYawPitchRoll(0, 0, MathHelper.ToRadians(this.Rotation));
			Matrix4x4 scale = Matrix4x4.CreateScale(new Vector3(this.Scale, 0));
			return scale * rotation * translation;
		}

		public Matrix4x4 ParentToLocal() {
			Matrix4x4 result = Matrix4x4.Identity;
			Matrix4x4.Invert(LocalToParent(), out result);
			return result;
		}

		public Matrix4x4 LocalToWorld() {
			if (this.Node.Parent != null) {
				return this.Node.Parent.Transform.LocalToWorld() * LocalToParent();
			}

			return LocalToParent();
		}

		public Matrix4x4 WorldToLocal() {
			if (this.Node.Parent != null) {
				return ParentToLocal() * this.Node.Parent.Transform.WorldToLocal();
			}

			return ParentToLocal();
		}

		#endregion

	}
}