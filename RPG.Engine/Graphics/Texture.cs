﻿namespace RPG.Engine.Graphics {
	using Core;
	using Modules.Interfaces;
	using Utility;

	public class Texture {

		#region Properties

		public uint ID {
			get;
			private set;
		}
		
		public uint Width {
			get;
			private set;
		}
		
		public uint Height {
			get;
			private set;
		}

		public string FilePath {
			get;
			private set;
		}

		#endregion


		#region Constructor

		public Texture(byte[] data, uint width, uint height, string filePath, ColorType colorType = ColorType.RGB, WrapModeType wrapModeType = WrapModeType.Repeat) {
			this.Width = width;
			this.Height = height;
			this.FilePath = filePath;

			CreateTexture(data, width, height, colorType, wrapModeType);
		}

		#endregion


		#region Private Methods

		private void CreateTexture(byte[] data, uint width, uint height, ColorType colorType, WrapModeType wrapModeType) {
			Debug.Log(GetType().Name, $"Creating Texture: {this.FilePath}");
			if (Application.Instance.GraphicsModule is IGLGraphicsModule) {
				this.ID = ((IGLGraphicsModule) Application.Instance.GraphicsModule).CompileTexture(data, width, height, colorType, wrapModeType);
			}
		}

		#endregion
		
	}
}