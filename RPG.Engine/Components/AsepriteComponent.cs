namespace RPG.Engine.Components {
	using Attributes;
	using Graphics;
	using Interfaces;
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

		
	}
}