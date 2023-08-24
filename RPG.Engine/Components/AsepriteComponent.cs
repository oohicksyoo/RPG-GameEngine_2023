namespace RPG.Engine.Components {

	using System.Numerics;
	using Aseprite;
	using Attributes;
	using Core;
	using Graphics;
	using Interfaces;
	using Newtonsoft.Json.Linq;
	using Serialization;
	using Settings;
	using Utility;

	//TODO: Need a way to mark this Component for cleanup so we can safely dispose of the texture and mesh assets out of memory
	public class AsepriteComponent : AbstractComponent, IComponentRenderable {


		#region Non Serialized

		[NonSerialized]
		private AsepriteFile asepriteFile;

		#endregion
		
		
		#region Properties

		[Inspector]
		public AsepriteFile AsepriteFile {
			get {
				return asepriteFile;
			}
			set {
				asepriteFile = value;
				if (value != null) {
					this.Texture = new Texture(this.AsepriteFile.GetPixels(), (uint)this.AsepriteFile.TextureWidth, (uint)this.AsepriteFile.TextureHeight, ColorType.RGBA);
					BuildMesh();
				} else {
					this.Texture = null;
					this.Mesh = null;
				}
			}
		}

		public Texture Texture {
			get;
			private set;
		}

		#endregion
		

		#region IComponentRenderable

		public bool CanRender => this.AsepriteFile != null;

		public Mesh Mesh {
			get;
			private set;
		}

		public uint TextureId {
			get {
				if (this.Texture != null) {
					return this.Texture.ID;
				}

				return 0;
			}
		}

		public int CurrentFrame {
			get;
			private set;
		}

		public int SingleFrameWidth {
			get;
			private set;
		}

		public int TotalFrameCount {
			get;
			private set;
		}

		#endregion
		
		
		#region ISerialize

		public override JObject Serialize() {
			JObject jsonObject = base.Serialize();

			if (this.AsepriteFile != null) {
				jsonObject[nameof(this.AsepriteFile)] = this.AsepriteFile.FilePath;
			}

			return jsonObject;
		}

		public override void Deserialize(JObject jsonObject) {
			base.Deserialize(jsonObject);

			if (jsonObject.ContainsKey(nameof(this.AsepriteFile))) {
				this.AsepriteFile = new AsepriteFile((string)jsonObject[nameof(this.AsepriteFile)]);
			}
		}

		#endregion


		#region Private Methods

		private void BuildMesh() {
			float width = this.AsepriteFile.SingleFrameWidth;
			float height = this.AsepriteFile.SingleFrameHeight;
			float pixelPerMeter = Application.Instance.Project.PixelsPerMetre;
			float ppmWidth = width / pixelPerMeter;
			float ppmHeight = height / pixelPerMeter;
			float halfWidth = ppmWidth * 0.5f;
			float halfHeight = ppmHeight * 0.5f;
			float offsetX = this.AsepriteFile.PivotX * ppmWidth * 0.5f;
			float offsetY = this.AsepriteFile.PivotY * ppmHeight * 0.5f;
			
			this.Mesh = new Mesh(
				new List<Vertex>() {
					new Vertex(new Vector3(-halfWidth + offsetX, -halfHeight + offsetY, 0), new Vector2(0, 1), new Vector4(1, 0, 0, 1)),
					new Vertex(new Vector3(halfWidth + offsetX, -halfHeight + offsetY,0), new Vector2(1, 1),new Vector4(0, 1, 0, 1)),
					new Vertex(new Vector3(-halfWidth + offsetX, halfHeight + offsetY,0), new Vector2(0, 0), new Vector4(0, 0, 1, 1)),
					new Vertex(new Vector3(halfWidth + offsetX, halfHeight + offsetY,0), new Vector2(1, 0), new Vector4(1, 0, 1, 1))
				}, 
				new List<int>() {
					0, 1, 2,
					2, 3, 1,
				});
			
			//TODO: Setup starting animation
			
			//Setup Properties
			this.SingleFrameWidth = this.AsepriteFile.SingleFrameWidth;
			this.TotalFrameCount = this.AsepriteFile.FrameCount;
			this.CurrentFrame = 0;
		}

		#endregion

		
	}
}