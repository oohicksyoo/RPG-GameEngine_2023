namespace RPG.Engine.Utility {
	using System.Drawing;
	using System.Numerics;
	using Newtonsoft.Json.Linq;

	public static class ColorExtensions {
		public static Vector3 ToVector3(this Color value) {
			return new Vector3((float)value.R / 255, (float)value.G / 255, (float)value.B / 255);
		}
		
		public static Vector4 ToVector4(this Color value) {
			return new Vector4((float)value.R / 255, (float)value.G / 255, (float)value.B / 255, (float)value.A / 255);
		}
		
		public static JObject ToJson(this Color value) {
			JObject vectorJson = new JObject();

			vectorJson.Add("r", value.R);
			vectorJson.Add("g", value.G);
			vectorJson.Add("b", value.B);
			vectorJson.Add("a", value.A);
			
			return vectorJson;
		}
	}
}