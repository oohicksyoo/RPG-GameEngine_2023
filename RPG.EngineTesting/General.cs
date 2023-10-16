using Module.OpenGL;
using Module.SDL2;
using NUnit.Framework;
using RPG.Engine.Core;

namespace RPG.EngineTesting {
	using Engine.Platform;

	public class General {


		#region Properties

		private bool IsTestComplete => this.TestState == TestState.Completed | this.TestState == TestState.Failed;

		private TestState TestState {
			get;
			set;
		} = TestState.Unknown;

		#endregion


		#region One Time Methods

		[OneTimeSetUp]
		protected void OneTimeSetup() {
			//System Module
			Application.Instance.Register<SDL2Module>();
			
			//Input Module
			Application.Instance.Register<SDL2InputModule>();
			
			//Graphics Module
			Application.Instance.Register<OpenGLModule>();
			
			//TODO: Find a way to make this project agnostic
			new Thread(() => Application.Instance.Start(Project.Instance, PlatformType.Windows)).Start();
			
			//Wait for the engine to fully start up before starting tests
			while (!Application.Instance.IsApplicationRunning) { }
		}
		
		[OneTimeTearDown]
		protected void OneTimeTearDown() {
			Application.Instance.RequestShutdown();
		}

		#endregion


		#region Pre & Post Test Methods

		/// <summary>
		/// Called before each Test has ran
		/// </summary>
		[SetUp]
		protected void PreTestRan() {
			this.TestState = TestState.Running;
		}

		/// <summary>
		/// Called after each Test has ran
		/// </summary>
		[TearDown]
		protected void PostTestRan() {
			this.TestState = TestState.Unknown;
		}

		#endregion


		#region Test Helper Methods

		private void TestRunner(Action<float> method) {
			while (!this.IsTestComplete) {}
		}
		
		private void GeneralTestComplete() {
			if (this.TestState == TestState.Completed) {
				Assert.Pass();
			} else {
				Assert.Fail();
			}
		}

		#endregion


		#region Tests

		[Test]
		public void TestWindowSetup() {
			if (Application.Instance.IsApplicationRunning) {
				this.TestState = TestState.Completed;
			} else {
				this.TestState = TestState.Failed;
			}
			GeneralTestComplete();
		}

		#endregion
		
	}
}