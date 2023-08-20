namespace RPG.Engine.Utility {
	public static class Debug {

		
		#region Private Variables

		private static Queue<Log> logs;

		#endregion


		#region Properties

		public static Queue<Log> Logs {
			get {
				return logs ??= new Queue<Log>();
			}
		}

		#endregion


		#region Public Methods

		public static void Log(string caller, string value) {
			SetColor(ConsoleColor.White);
			SetColor(ConsoleColor.DarkGray);
			Console.Write($"{caller}: ");
			SetColor(ConsoleColor.White);
			Console.Write($"{value}");
			Console.WriteLine();
			AddToLog(caller, $"{value}", LogType.Standard);
			SetColor(ConsoleColor.White);
		}
		
		public static void Warning(string caller, string value) {
			SetColor(ConsoleColor.DarkYellow);
			Console.WriteLine($"[Warning] {value}");
			AddToLog(caller, $"[Warning] {value}", LogType.Warning);
			SetColor(ConsoleColor.White);
		}
		
		public static void Error(string caller, string value) {
			SetColor(ConsoleColor.DarkRed);
			Console.WriteLine($"[Error] {value}");
			AddToLog(caller, $"[Error] {value}", LogType.Error);
			SetColor(ConsoleColor.White);
		}

		#endregion


		#region Private Methods

		private static void AddToLog(string caller, string value, LogType type) {
			Log log = new Log() {
				caller = caller,
				message = value,
				logType = type,
				timeStamp = DateTime.Now
			};
			
			Logs.Enqueue(log);
			if (Logs.Count > 1000) {
				Logs.Dequeue();
			}
		}

		private static void SetColor(ConsoleColor consoleColor) {
			#if RPG_COLOR_LOG
				Console.ForegroundColor = consoleColor;
			#endif
		}

		#endregion

	}
}