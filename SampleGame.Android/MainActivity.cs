using Android.App;
using Android.Content.PM;
using Android.Views;
using RPG.Engine.Core;
using SampleGame.Core;
using Module.SDL2;
using Module.OpenGL;
using Org.Libsdl.App;

namespace SampleGame.Android {
	[Activity(
		Label = "@string/app_name", 
		MainLauncher = true, 
		HardwareAccelerated = true, 
		ScreenOrientation = ScreenOrientation.Landscape)]
	public class MainActivity : SDLActivity {

		public override void LoadLibraries() {
			base.LoadLibraries();
			Bootstrap.SetupMain();
		}
		
		public override void OnWindowFocusChanged(bool hasFocus) {
			base.OnWindowFocusChanged(hasFocus);
			if (hasFocus) {
				Window.DecorView.SystemUiVisibility = (StatusBarVisibility) (
					SystemUiFlags.LayoutStable |
					SystemUiFlags.LayoutHideNavigation |
					SystemUiFlags.LayoutFullscreen |
					SystemUiFlags.HideNavigation |
					SystemUiFlags.Fullscreen |
					SystemUiFlags.ImmersiveSticky
				);
			}
		}
		
	}
}
