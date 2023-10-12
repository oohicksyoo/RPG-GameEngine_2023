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
		
		public static JObject ToJObject(this Color value) {
			JObject vectorJson = new JObject();

			vectorJson.Add(nameof(value.R), value.R);
			vectorJson.Add(nameof(value.G), value.G);
			vectorJson.Add(nameof(value.B), value.B);
			vectorJson.Add(nameof(value.A), value.A);
			
			return vectorJson;
		}

		public static Color FromJObjectColor(this JObject jsonObject) {
			Color color = Color.FromArgb(
				(int)jsonObject[nameof(color.A)],
				(int)jsonObject[nameof(color.R)],
				(int)jsonObject[nameof(color.G)],
				(int)jsonObject[nameof(color.B)]
			);
			
			return color;
		}
	}
}