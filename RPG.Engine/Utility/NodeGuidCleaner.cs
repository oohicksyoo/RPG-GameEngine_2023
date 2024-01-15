namespace RPG.Engine.Utility {
	using Newtonsoft.Json.Linq;

	public static class NodeGuidCleaner {
		
		//Used in referencing naming locations so we dont have a hardcoded value
		#region Private Static Properties

		private static string Guid {
			get;
			set;
		}

		#endregion
		

		#region Public Static Methods

		public static JObject CleanNode(JObject serializedNode) {
			Debug.Log("NodeGuidCleaner", $"Cleaning guids");
			List<JProperty> list = GetGuidsFromNode(serializedNode);
			Dictionary<string, string> mappedGuids = new Dictionary<string, string>(); 

			foreach (JProperty jProperty in list) {
				Debug.Log("NodeGuidCleaner", $"{jProperty.Value}");
				string guid = System.Guid.NewGuid().ToString();
				mappedGuids.Add((string)jProperty.Value, guid);
				jProperty.Value = guid;
			}
			
			serializedNode = ReplaceGuids(mappedGuids, serializedNode);

			return serializedNode;
		}
		

		#endregion


		#region Private Static Methods

		private static List<JProperty> GetGuidsFromNode(JObject jObject) {
			List<JProperty> list = jObject.Properties().Where(x => x.Name == nameof(Guid)).ToList();

			foreach (JProperty child in jObject.Children()) {
				if (child.Value.Type == JTokenType.Object) {
					list.AddRange(GetGuidsFromNode((JObject)child.Value));
				} else if (child.Value.Type == JTokenType.Array) {
					foreach (JObject childObject in child.Value) {
						list.AddRange(GetGuidsFromNode(childObject));
					}
				}
			}

			return list;
		}

		private static JObject ReplaceGuids(Dictionary<string, string> mappedGuids, JObject jObject) {
			foreach (JProperty child in jObject.Children()) {
				switch (child.Value.Type) {
					case JTokenType.Object:
						child.Value = ReplaceGuids(mappedGuids, (JObject)child.Value);
						break;
					case JTokenType.Array:
						child.Value = ReplaceGuids(mappedGuids, (JArray)child.Value);
						break;
					case JTokenType.String:
					case JTokenType.Guid:
						string value = (string)child.Value;
						if (mappedGuids.ContainsKey(value)) {
							//Swap to new guid
							Debug.Log("NodeGuidCleaner", $"Swapping guid ({value}) -> ({mappedGuids[value]})");
							child.Value = mappedGuids[value];
						}
						break;
					default:
						Debug.Log("NodeGuidCleaner", $"Missing type ({child.Value.Type}) for ({child.Value})");
						break;
				}
			}

			return jObject;
		}

		private static JArray ReplaceGuids(Dictionary<string, string> mappedGuids, JArray jArray) {
			for (int i = 0; i < jArray.Count; i++) {
				//Our system of node should always be an array of objects
				jArray[i] = ReplaceGuids(mappedGuids, (JObject)jArray[i]);
			}

			return jArray;
		}

		#endregion
		
	}
}