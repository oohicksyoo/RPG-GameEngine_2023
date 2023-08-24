namespace RPG.Engine.Aseprite {
	using Core;
	using Utility;

	public class Animation {
		

		#region Properties

		public string Name => this.Tag.Name;

		public int FrameLength {
			get;
			private set;
		}

		public float TimeLength {
			get;
			private set;
		}

		public int CurrentFrame {
			get;
			private set;
		}

		private Tag Tag {
			get;
			set;
		}

		private List<float> FrameTimes {
			get;
			set;
		}

		private float CurrentTime {
			get;
			set;
		}

		private bool IsPingPongForward {
			get;
			set;
		}

		#endregion
		
		
		#region Constructor

		public Animation(Tag tag, List<float> frameTimes) {
			this.Tag = tag;
			this.FrameTimes = frameTimes;
			this.TimeLength = CalculateAnimationTimeLength();
			Reset();
		}

		#endregion


		#region Public Methods

		public void Reset() {
			this.FrameLength = this.Tag.To - this.Tag.From;
			this.CurrentTime = 0;
			this.CurrentFrame = (this.Tag.LoopDirection == LoopDirection.Reverse) ? this.Tag.To : this.Tag.From;
			this.IsPingPongForward = true;
		}

		public void Update() {
			if (this.FrameTimes.Count <= 1) {
				return;
			}

			this.CurrentTime += Time.Delta * 1000; //Bump this counter up to milliseconds which the frameTimes is referring too
			int frame = (int)MathHelper.Remap(this.CurrentFrame, this.Tag.From, this.Tag.To, 0, this.FrameLength);
			if (this.CurrentTime >= this.FrameTimes[frame]) {
				this.CurrentTime = 0;
				switch (this.Tag.LoopDirection) {
					case LoopDirection.Forward:
						this.CurrentFrame = (this.CurrentFrame + 1 <= this.Tag.To) ? this.CurrentFrame + 1 : this.Tag.From;
						break;
					case LoopDirection.Reverse:
						this.CurrentFrame = (this.CurrentFrame - 1 >= this.Tag.From) ? this.CurrentFrame - 1 : this.Tag.To;
						break;
					case LoopDirection.PingPong:
						if (this.IsPingPongForward) {
							this.CurrentFrame = (this.CurrentFrame + 1 <= this.Tag.To) ? this.CurrentFrame + 1 : this.Tag.From;
						} else {
							this.CurrentFrame = (this.CurrentFrame - 1 >= this.Tag.From) ? this.CurrentFrame - 1 : this.Tag.To;
						}

						//Flip animation to ping pong back
						if (this.CurrentFrame == this.Tag.From || this.CurrentFrame == this.Tag.To) {
							this.IsPingPongForward = !this.IsPingPongForward;
						}
						break;
				}
			}
		}

		#endregion


		#region Private Methods

		private float CalculateAnimationTimeLength() {
			float count = 0;

			foreach (float frameTime in this.FrameTimes) {
				count += frameTime;
			}
			
			return count;
		}

		#endregion
		
		
	}
}