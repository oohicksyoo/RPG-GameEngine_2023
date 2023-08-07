namespace RPG.Engine.Components {
	using Attributes;
	using Graphics;
	using Interfaces;
	using Newtonsoft.Json.Linq;
	using Serialization;

	public class AsepriteComponent : AbstractComponent, IComponentRenderable {

		
		#region Properties

		[Inspector]
		public AsepriteAssetFile AsepriteAssetFile {
			get;
			set;
		}

		#endregion
		

		#region IComponentRenderable

		public bool CanRender {
			get;
		}

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
			return jsonObject;
		}

		public override void Deserialize(JObject jObject) {
			
		}

		#endregion

		
	}
}