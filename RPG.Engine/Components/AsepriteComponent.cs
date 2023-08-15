namespace RPG.Engine.Components {
	
	using Aseprite;
	using Attributes;
	using Graphics;
	using Interfaces;
	using Newtonsoft.Json.Linq;
	using Serialization;

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
				} else {
					this.Texture = null;
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
		}

		public uint TextureId {
			get;
		}

		public int CurrentFrame {
			get;
		}

		public int SingleFrameWidth {
			get;
		}

		public int TotalFrameCount {
			get;
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

		
	}
}