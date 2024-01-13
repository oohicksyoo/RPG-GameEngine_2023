namespace RPG.Engine.Aseprite {
	using System.Drawing;
	using System.IO.Compression;
	using System.Numerics;
	using System.Text;
	using Utility;

	public class AsepriteFile {


		#region Constructor

		public AsepriteFile(string filePath) {
			this.FilePath = filePath;
			using var stream = File.Open(Directory.GetCurrentDirectory() + this.FilePath, FileMode.Open);
			Decode(stream);
		}

		#endregion


		#region Types

		private struct OrderedCel {
			public Cel Cel;
			public int Index;
		}

		#endregion


		#region Non Serialized Fields

		[NonSerialized]
		private List<Frame> frames;
		
		[NonSerialized]
		private List<Slice> slices;
		
		[NonSerialized]
		private List<Tag> tags;
		
		[NonSerialized]
		private List<Layer> layers;

		[NonSerialized]
		private Dictionary<string, Animation> animations;

		#endregion


		#region Properties

		public string FilePath {
			get;
			private set;
		}
		
		public int SingleFrameWidth => this.Width;

		public int SingleFrameHeight => this.Height;

		public int TextureWidth => this.Width * this.FrameCount;

		public int TextureHeight => this.Height;

		public float PivotX {
			get;
			private set;
		}
		
		public float PivotY {
			get;
			private set;
		}

		public int FrameCount {
			get;
			private set;
		}

		private int Width {
			get;
			set;
		}

		private int Height {
			get;
			set;
		}

		private ColorDepth ColorDepth {
			get;
			set;
		}

		private List<Frame> Frames {
			get {
				return frames ??= new List<Frame>();
			}
		}
		
		private List<Slice> Slices {
			get {
				return slices ??= new List<Slice>();
			}
		}
		
		private List<Tag> Tags {
			get {
				return tags ??= new List<Tag>();
			}
		}
		
		private List<Layer> Layers {
			get {
				return layers ??= new List<Layer>();
			}
		}

		private Dictionary<string, Animation> Animations {
			get {
				return animations ??= new Dictionary<string, Animation>();
			}
		}

		private Animation CurrentAnimation {
			get;
			set;
		}

		public int CurrentFrame {
			get {
				if (this.CurrentAnimation != null) {
					return this.CurrentAnimation.CurrentFrame;
				}

				return 0;
			}
		}

		#endregion


		#region Public Methods

		/// <summary>
		/// Animation Update using the engine's Update method (AsepriteComponent)
		/// </summary>
		public void Update() {
			this.CurrentAnimation?.Update();
		}

		public void SetAnimation(string name) {
			if (this.CurrentAnimation != null && this.CurrentAnimation.Name == name) {
				//This animation is currently running
				return;
			}
			
			if (this.Animations.ContainsKey(name)) {
				this.CurrentAnimation = this.Animations[name];
				this.CurrentAnimation.Reset();
			}
		}
		
		public string[] GetAnimationList() {
			return this.Animations.Keys.ToArray();
		}

		public byte[] GetPixels() {
			//TODO: Maybe double check loading the file went okay
			int size = this.TextureWidth * this.TextureHeight;
			List<Color> pixels = new List<Color>();
			
			for (int i = 0; i < size; i++) {
				pixels.Add(SetupColor(0,0,0,0, false));
			}

			for (int f = 0; f < this.FrameCount; f++) {
				Frame frame = this.Frames[f];
				int offsetX = this.Width * f;
				int offsetY = this.Width * this.FrameCount;
				
				//Reorder cels based on their current position + zIndex of the cell
				List<OrderedCel> orderedCels = new List<OrderedCel>();
				for (int c = 0; c < frame.Cels.Count; c++) {
					Debug.Log(GetType().Name, $"{c} + {frame.Cels[c].ZIndex}");
					orderedCels.Add(new OrderedCel() {
						Cel = frame.Cels[c],
						Index = frame.Cels[c].ZIndex
					});
				}
				
				orderedCels = orderedCels.OrderBy(x => x.Index).ToList();

				foreach (OrderedCel orderedCel in orderedCels) {
					Cel cel = orderedCel.Cel;
					
					//If Layer is marked as invisible skip the rendering on it
					if (!cel.Layer.IsVisible) {
						continue;
					}

					for (int celY = 0; celY < cel.Height; celY++) {
						for (int celX = 0; celX < cel.Width; celX++) {
							
							//Out of bounds checking
							if (cel.X + celX < 0 || cel.X + celX >= this.Width) {
								continue;
							}
							if (cel.Y + celY < 0 || cel.Y + celY >= this.Height) {
								continue;
							}

							int index = celX + celY * cel.Width;
							Color pixel = cel.Pixels[index];

							int totalOffset = cel.Y * offsetY + cel.X;
							int totalYShift = celY * offsetY;
							int totalXShift = celX;
							int totalShift = totalXShift + totalYShift;
							int globalIndex = totalOffset + totalShift + offsetX;

							pixels[globalIndex] = Blend(BlendType.Normal, pixels[globalIndex], pixel);
						}
					}
				}
			}


			byte[] bytes = new byte[pixels.Count * 4];
			int count = 0;
			foreach (Color pixel in pixels) {
				bytes[count + 0] = pixel.R;
				bytes[count + 1] = pixel.G;
				bytes[count + 2] = pixel.B;
				bytes[count + 3] = pixel.A;
				count += 4;
			}

			return bytes;
		}

		#endregion


		#region Private Methods

		private void Decode(Stream stream) {
			BinaryReader binaryReader = new BinaryReader(stream);
			
			byte BYTE() => binaryReader.ReadByte();
			ushort WORD() => binaryReader.ReadUInt16();
			short SHORT() => binaryReader.ReadInt16();
			uint DWORD() => binaryReader.ReadUInt32();
			long LONG() => binaryReader.ReadInt32();
			string STRING() => Encoding.UTF8.GetString(BYTES(WORD()));
			byte[] BYTES(int number) => binaryReader.ReadBytes(number);
			void SEEK(int number) => binaryReader.BaseStream.Position += number;
			
			//Header
			DWORD(); //Filesize
			
			//Magic Number
			if (WORD() != 0xA5E0) {
				throw new Exception("File is not an aseprite file");
			}
			
			//Basic Information
			this.FrameCount = WORD();
			this.Width = WORD();
			this.Height = WORD();
			this.ColorDepth = (ColorDepth)(WORD() / 8);
			
			Debug.Log(GetType().Name, $"{this.Width}x{this.Height} - {this.FrameCount}");

			//Extra Information
			DWORD(); // Flags
			WORD(); // Speed (deprecated)
			DWORD(); // Set be 0
			DWORD(); // Set be 0
			BYTE(); // Palette entry 
			SEEK(3); // Ignore these bytes
			WORD(); // Number of colors (0 means 256 for old sprites)
			BYTE(); // Pixel width
			BYTE(); // Pixel height
			SEEK(92); // For Future

			byte[] temp = new byte[this.Width * this.Height * (int)ColorDepth];
			Color[] palette = new Color[256];
			IUserData? lastUserData = null;
			
			for (int i = 0; i < this.FrameCount; i++) {
				Frame frame = new Frame();
				this.Frames.Add(frame);

				long frameStart;
				long frameEnd;
				int chunkCount;
				
				//Frame Header
				frameStart = binaryReader.BaseStream.Position;
				frameEnd = frameStart + DWORD();
				WORD(); //Magic Number
				chunkCount = WORD(); //Number of chunks in this frame
				frame.Duration = WORD(); //Frame duration in milliseconds
				SEEK(6); //Future
				
				//Chunks
				for (int j = 0; j < chunkCount; j++) {
					long chunkStart;
					long chunkEnd;
					ChunkType chunkType;
					
					//Header
					chunkStart = binaryReader.BaseStream.Position;
					chunkEnd = chunkStart + DWORD();
					chunkType = (ChunkType)WORD();

					switch (chunkType) {
						case ChunkType.Layer: {
							Layer layer = new Layer();

							layer.Flag = (Layer.Flags)WORD();
							layer.Type = (Layer.Types)WORD();
							layer.ChildLevel = WORD();
							WORD(); // Width unused
							WORD(); // Height unused
							layer.BlendMode = WORD();
							layer.Alpha = (BYTE() / 255f);
							SEEK(3);// Future
							layer.Name = STRING();

							lastUserData = layer;
							this.Layers.Add(layer);
							
							break;
						}
						case ChunkType.Cel: {
							Cel cel = new Cel();
							cel.Layer = Layers[WORD()];
							cel.X = SHORT();
							cel.Y = SHORT();
							cel.Opactiy = BYTE() / 255f;
							CelType celType = (CelType)WORD();
							
							//Z Index
							cel.ZIndex = SHORT();
							//0 = default layer ordering
							//+N = show this cel N layers later
							//-N = show this cel N layers back
							
							SEEK(5);

							if (celType == CelType.RawCel || celType == CelType.CompressedImage) {
								cel.Width = WORD();
								cel.Height = WORD();
								var byteCount = cel.Width * cel.Height * (int)this.ColorDepth;

								if (byteCount > temp.Length) {
									temp = new byte[byteCount];
								}

								if (celType == CelType.RawCel) {
									binaryReader.Read(temp, 0, byteCount);
								} else {
									SEEK(2);

									using var deflateStream = new DeflateStream(binaryReader.BaseStream, CompressionMode.Decompress, true);
									deflateStream.Read(temp, 0, byteCount);
								}

								cel.Pixels = new Color[cel.Width * cel.Height];
								BytesToPixels(temp, cel.Pixels, this.ColorDepth, palette);
							} else if (celType == CelType.LinkedCel) {
								Frame linkedFrame = this.Frames[WORD()];
								Cel linkedCel = linkedFrame.Cels[frame.Cels.Count];

								cel.Width = linkedCel.Width;
								cel.Height = linkedCel.Height;
								cel.Pixels = linkedCel.Pixels;
							}

							lastUserData = cel;
							frame.Cels.Add(cel);
							break;
						}
						case ChunkType.Palette: {
							var size = DWORD();
							var start = DWORD();
							var end = DWORD();
							SEEK(8); //Future

							for (int k = 0; k < (end - start) + 1; k++) {
								var hasName = WORD();
								palette[start + k] = SetupColor(BYTE(), BYTE(), BYTE(), BYTE());

								if (IsBitSet(hasName, 0)) {
									STRING();
								}
							}
							
							break;
						}
						case ChunkType.UserData: {
							if (lastUserData != null) {
								var flags = (int)DWORD();
								
								//Has Text?
								if (IsBitSet(flags, 0)) {
									lastUserData.UserDataText = STRING();
								}
								
								//Has Color
								if (IsBitSet(flags, 0)) {
									lastUserData.UserDataColor = SetupColor(BYTE(), BYTE(), BYTE(), BYTE());
								}
							}
							break;
						}
						case ChunkType.FrameTags: {
							var count = WORD();
							SEEK(8);

							for (int t = 0; t < count; t++) {
								Tag tag = new Tag();
								tag.From = WORD();
								tag.To = WORD();
								tag.LoopDirection = (LoopDirection)BYTE();
								SEEK(8);
								tag.Color = SetupColor(BYTE(), BYTE(), BYTE(), 255);
								//tag.Color = new Color(BYTE(), BYTE(), BYTE(), 255).Premultiply;
								SEEK(1);
								tag.Name = STRING();
								this.Tags.Add(tag);
							}

							break;
						}
						case ChunkType.Slice: {
							var count = DWORD();
							var flags = (int)DWORD();
							DWORD(); //Reserved
							var name = STRING();

							for (int s = 0; s < count; s++) {
								Slice slice = new Slice();
								slice.Name = name;
								slice.Frame = (int)DWORD();
								slice.OriginX = (int)LONG();
								slice.OriginY = (int)LONG();
								slice.Width = (int)DWORD();
								slice.Height = (int)DWORD();

								//Check for 9 slice
								if (IsBitSet(flags, 0)) {
									slice.NineSlice = new Vector4(
										(int)LONG(),
										(int)LONG(),
										(int)DWORD(),
										(int)DWORD()
									);
								}

								//Pivot Point
								if (IsBitSet(flags, 1)) {
									slice.Pivot = new Vector2(
										(int)DWORD(),
										(int)DWORD()
									);
								}

								lastUserData = slice;
								this.Slices.Add(slice);
							}

							break;
						}
						default:
							//Chunktype not implemented yet
							break;
					}

					binaryReader.BaseStream.Position = chunkEnd;
				}

				binaryReader.BaseStream.Position = frameEnd;
			}
			
			//TODO: Animations
			ProcessAnimations();
			ProcessSlices();
		}

		private void ProcessSlices() {
			this.PivotX = 0;
			this.PivotY = 0;

			foreach (Slice slice in this.Slices) {
				if (slice.Name == "Pivot") {
					this.PivotX = (this.SingleFrameWidth * 0.5f) - (slice.OriginX + (slice.Width * 0.5f));
					this.PivotY = -1 * ((this.SingleFrameHeight * 0.5f) - (slice.OriginY + (slice.Height * 0.5f)));
				}
				//TODO: Hitbox implementation here for per frame hitboxes
			}
		}

		private void ProcessAnimations() {
			string firstAnimation = String.Empty;
			foreach (Tag tag in this.Tags) {
				if (string.IsNullOrEmpty(firstAnimation)) {
					firstAnimation = tag.Name;
				}
				
				List<float> frameTimes = new List<float>();

				for (int i = tag.From; i <= tag.To; i++) {
					frameTimes.Add(this.Frames[i].Duration);
				}

				Animation animation = new Animation(tag, frameTimes);
				this.Animations.Add(tag.Name, animation);
			}

			if (this.Animations.Count == 0 || string.IsNullOrEmpty(firstAnimation)) {
				//Create a default animation using the first frame
				Tag tag = new Tag();
				tag.Name = "No Animation";
				tag.From = 0;
				tag.To = 0;
				tag.Color = Color.White;
				Animation animation = new Animation(tag, new List<float>() { 100 });
				this.Animations.Add(tag.Name, animation);

				firstAnimation = tag.Name;
			}

			this.CurrentAnimation = this.Animations[firstAnimation];
		}

		private bool IsBitSet(byte b, int pos) {
			return (b & (1 << pos)) != 0;
		}
		
		private bool IsBitSet(int b, int pos) {
			return (b & (1 << pos)) != 0;
		}

		private Color SetupColor(byte r, byte g, byte b, byte a, bool premultiply = true) {
			Color c = new Color();
			
			if (premultiply) {
				c = Color.FromArgb(a, (byte)(r * a / 255), (byte)(g * a / 255), (byte)(b * a / 255));
			} else {
				c = Color.FromArgb(a, r, g, b);
			}

			return c;
		}
		
		private void BytesToPixels(byte[] bytes, Color[] pixels, ColorDepth colorDepth, Color[] palette) {
			int length = pixels.Length;
			switch (colorDepth) {
				case ColorDepth.RGBA: {
					for (int i = 0, b = 0; i < length; i++, b += 4) {
						pixels[i] = SetupColor(bytes[b + 0], bytes[b + 1], bytes[b + 2], bytes[b + 3]);
					}
					break;
				}
				case ColorDepth.Greyscale: {
					for (int i = 0, b = 0; i < length; i++, b += 2) {
						pixels[i] = SetupColor(bytes[b + 0], bytes[b + 0], bytes[b + 0], bytes[b + 1]);
					}
					break;
				}
				case ColorDepth.Indexed: {
					for (int i = 0; i < length; i++) {
						pixels[i] = palette[bytes[i]];
					}
					break;
				}
			}
		}
		
		private Color Blend(BlendType blendType, Color a, Color b) {
			switch (blendType) {
				case BlendType.Normal:
				default:
					if (b.A == 0) {
						return a;
					}

					if (b.A == 255) {
						return b;
					}

					int alpha = 255 - (255 - b.A) * (255 - a.A);
					int rr = a.R + (b.R - a.R) * b.A / alpha;
					int rg = a.G + (b.G - a.G) * b.A / alpha;
					int rb = a.B + (b.B - a.B) * b.A / alpha;

					return SetupColor((byte)rr, (byte)rg, (byte)rb, (byte)alpha, false);
			}
		}

		#endregion

		
	}
}