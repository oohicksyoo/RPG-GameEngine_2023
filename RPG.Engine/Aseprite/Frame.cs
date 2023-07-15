namespace RPG.Engine.Aseprite {
	public class Frame {


		#region Constructor

		public Frame() {
			
		}

		#endregion


		#region Non Serialized Fields

		[NonSerialized]
		private List<Cel> cels;

		#endregion


		#region Properties

		public float Duration {
			get;
			set;
		}

		public List<Cel> Cels {
			get {
				return cels ??= new List<Cel>();
			}
		}

		#endregion
		
	}
}