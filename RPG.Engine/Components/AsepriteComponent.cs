namespace RPG.Engine.Components {

	using System.Numerics;
	using Aseprite;
	using Attributes;
	using Core;
	using Graphics;
	using Inspector;
	using Interfaces;
	using Newtonsoft.Json.Linq;
	using Serialization;
	using Settings;
	using Utility;

	//TODO: Need a way to mark this Component for cleanup so we can safely dispose of the texture and mesh assets out of memory
	public class AsepriteComponent : AbstractComponent, IComponentRenderable {


		#region Types

		public class Dropdown : AbstractDropdown {
			public Dropdown(string[] list) : base(list) {}
		}

		#endregion
		

		#region Non Serialized

		[NonSerialized]
		private AsepriteFile asepriteFile;

		[NonSerialized]
		private Dropdown animationDropdown;

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
					this.Animation = new Dropdown(this.AsepriteFile.GetAnimationList());
				} else {
					this.Texture = null;
					this.Mesh = null;
				}
			}
		}

		[Inspector]
		public int Layer {
			get;
			set;
		}

		[Inspector]
		public Dropdown Animation {
			get {
				if (animationDropdown == null) {
					string[] list = new[] {"AsepriteFile Missing!"};
					if (this.AsepriteFile != null) {
						list = this.AsepriteFile.GetAnimationList();
					}
					animationDropdown = new Dropdown(list);
				}
				
				return animationDropdown;
			}
			set {
				animationDropdown = value;
				if (this.AsepriteFile != null) {
					this.AsepriteFile.SetAnimation(animationDropdown.SelectedValue);
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
			get {
				if (this.AsepriteFile != null) {
					return this.AsepriteFile.CurrentFrame;
				}

				return 0;
			}
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

			jsonObject[nameof(this.Layer)] = this.Layer;
			jsonObject[nameof(this.Animation)] = this.Animation.SelectedValue;

			return jsonObject;
		}

		public override void Deserialize(JObject jsonObject) {
			base.Deserialize(jsonObject);

			if (jsonObject.ContainsKey(nameof(this.AsepriteFile))) {
				this.AsepriteFile = new AsepriteFile((string)jsonObject[nameof(this.AsepriteFile)]);
			}

			if (jsonObject.ContainsKey(nameof(this.Layer))) {
				this.Layer = (int)jsonObject[nameof(this.Layer)];
			}

			if (jsonObject.ContainsKey(nameof(this.Animation))) {
				this.Animation.PresetValue((string)jsonObject[nameof(this.Animation)]);
			}
		}

		#endregion


		#region Overrides

		public override void Update() {
			this.AsepriteFile?.Update();
		}

		#endregion


		#region Public Methods

		public void SetAnimation(string name) {
			this.Animation.PresetValue(name);
			this.AsepriteFile?.SetAnimation(name);
		}

		#endregion


		#region Private Methods

		private void BuildMesh() {
			float width = this.AsepriteFile.SingleFrameWidth;
			float height = this.AsepriteFile.SingleFrameHeight;
			float pixelPerMeter = Application.Instance.Project.PixelsPerMetre;
			float baseWidth = (width / pixelPerMeter) * 0.5f;
			float baseHeight = (height / pixelPerMeter) * 0.5f;
			float textureWidth = baseWidth;
			float textureHeight = baseHeight;
			float offsetX = (this.AsepriteFile.PivotX / (width * 0.5f)) * textureWidth;
			float offsetY = (this.AsepriteFile.PivotY / (height * 0.5f)) * textureHeight;
			
			this.Mesh = new Mesh(
				new List<Vertex>() {
					new Vertex(new Vector3(-textureWidth + offsetX, -textureHeight + offsetY, 0), new Vector2(0, 1), new Vector4(1, 0, 0, 1)),
					new Vertex(new Vector3(textureWidth + offsetX, -textureHeight + offsetY,0), new Vector2(1, 1),new Vector4(0, 1, 0, 1)),
					new Vertex(new Vector3(-textureWidth + offsetX, textureHeight + offsetY,0), new Vector2(0, 0), new Vector4(0, 0, 1, 1)),
					new Vertex(new Vector3(textureWidth + offsetX, textureHeight + offsetY,0), new Vector2(1, 0), new Vector4(1, 0, 1, 1))
				}, 
				new List<int>() {
					0, 1, 2,
					2, 3, 1,
				});
			
			//TODO: Setup starting animation
			
			//Setup Properties
			this.SingleFrameWidth = this.AsepriteFile.SingleFrameWidth;
			this.TotalFrameCount = this.AsepriteFile.FrameCount;
		}

		#endregion

		
	}
}