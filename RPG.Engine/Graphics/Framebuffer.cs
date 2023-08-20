namespace RPG.Engine.Graphics {
	using System.Numerics;
	using Core;
	using Utility;

	public class Framebuffer {


		#region Constructor

		public Framebuffer(uint id, uint renderTextureId, uint depthStencilId) {
			this.Id = id;
			this.RenderTextureId = renderTextureId;
			this.DepthStencilId = depthStencilId;
		}

		#endregion
		
		
		#region Properties

		public uint Id {
			get;
			private set;
		}

		public uint RenderTextureId {
			get;
			private set;
		}

		public uint DepthStencilId {
			get;
			private set;
		}

		#endregion
		
	}
}