using RPG.Engine.Modules.Interfaces;
using RPG.Engine.Utility;

namespace Module.SDL2 {
	using System.Numerics;
	using RPG.Engine.Input;

	public class SDL2InputModule : AbstractInputModule {
		
		
		#region IInputModule

		public override void Initialize() {
			base.Initialize();
			this.Version = new Version(SDL.SDL_MAJOR_VERSION, SDL.SDL_MINOR_VERSION);
			Debug.Log(this.ModuleName, $"{this.Name} {this.Version}");
		}

		public override bool Poll() {
			SDL.SDL_GetMouseState(out int mouseX, out int mouseY);
			OnMousePosition(new Vector2(mouseX, mouseY));
			
			while (SDL.SDL_PollEvent(out SDL.SDL_Event e) != 0) {
				switch (e.type) {
					case SDL.SDL_EventType.SDL_QUIT:
						return false;
					case SDL.SDL_EventType.SDL_KEYUP:
						OnKeyUp(SDL2ToKeyboardKey(e.key.keysym.sym));
						break;
					case SDL.SDL_EventType.SDL_KEYDOWN:
						OnKeyDown(SDL2ToKeyboardKey(e.key.keysym.sym));
						break;
					case SDL.SDL_EventType.SDL_MOUSEBUTTONDOWN:
						if (e.button.button == SDL.SDL_BUTTON_LEFT) {
							OnMouseDown(MouseButtons.Left);
						} else if (e.button.button == SDL.SDL_BUTTON_RIGHT) {
							OnMouseDown(MouseButtons.Right);
						} else if (e.button.button == SDL.SDL_BUTTON_MIDDLE) {
							OnMouseDown(MouseButtons.Middle);
						}

						break;
					case SDL.SDL_EventType.SDL_MOUSEBUTTONUP:
						if (e.button.button == SDL.SDL_BUTTON_LEFT) {
							OnMouseUp(MouseButtons.Left);
						} else if (e.button.button == SDL.SDL_BUTTON_RIGHT) {
							OnMouseUp(MouseButtons.Right);
						} else if (e.button.button == SDL.SDL_BUTTON_MIDDLE) {
							OnMouseUp(MouseButtons.Middle);
						}
						break;
					case SDL.SDL_EventType.SDL_MOUSEWHEEL:
						OnMouseWheel(e.wheel.x, e.wheel.y);
						break;
					case SDL.SDL_EventType.SDL_WINDOWEVENT:
						switch (e.window.windowEvent) {
							case SDL.SDL_WindowEventID.SDL_WINDOWEVENT_RESIZED:
								//Application.OnResize(e.window.data1, e.window.data2);
								break;
						}
						break;
				}
			}
			
			return true;
		}

		#endregion
		
		
		#region IModule
		
		public override string ModuleName => GetType().Name;
		
		public override string Name => $"SDL2";

		public override Version Version {
			get;
			protected set;
		} = new Version();
		
		public override int Priority => int.MaxValue - 2;

		public override void Awake() {
			
		}

		public override void Start() {
			
		}

		public override void Update() {
			
		}

		public override void Shutdown() {
			Debug.Log(this.ModuleName, $"Shutdown");
		}

		#endregion
		
		
		#region Private Functions

		private KeyboardKeys SDL2ToKeyboardKey(SDL.SDL_Keycode keycode) {
			
			switch (keycode) {
				//Normal Characters
				case SDL.SDL_Keycode.SDLK_BACKSPACE:
					return KeyboardKeys.Backspace;
				case SDL.SDL_Keycode.SDLK_PERIOD:
					return KeyboardKeys.Period;
				
				//Numbers
				case SDL.SDL_Keycode.SDLK_0:
					return KeyboardKeys.Num0;
				case SDL.SDL_Keycode.SDLK_1:
					return KeyboardKeys.Num1;
				case SDL.SDL_Keycode.SDLK_2:
					return KeyboardKeys.Num2;
				case SDL.SDL_Keycode.SDLK_3:
					return KeyboardKeys.Num3;
				case SDL.SDL_Keycode.SDLK_4:
					return KeyboardKeys.Num4;
				case SDL.SDL_Keycode.SDLK_5:
					return KeyboardKeys.Num5;
				case SDL.SDL_Keycode.SDLK_6:
					return KeyboardKeys.Num6;
				case SDL.SDL_Keycode.SDLK_7:
					return KeyboardKeys.Num7;
				case SDL.SDL_Keycode.SDLK_8:
					return KeyboardKeys.Num8;
				case SDL.SDL_Keycode.SDLK_9:
					return KeyboardKeys.Num9;
				
				//Uppercase Letters
				case SDL.SDL_Keycode.SDLK_KP_A:
					return KeyboardKeys.A;
				
				//Lowercase Letters
				case SDL.SDL_Keycode.SDLK_a:
					return KeyboardKeys.a;
				case SDL.SDL_Keycode.SDLK_b:
					return KeyboardKeys.b;
				case SDL.SDL_Keycode.SDLK_c:
					return KeyboardKeys.c;
				case SDL.SDL_Keycode.SDLK_d:
					return KeyboardKeys.d;
				case SDL.SDL_Keycode.SDLK_e:
					return KeyboardKeys.e;
				case SDL.SDL_Keycode.SDLK_f:
					return KeyboardKeys.f;
				case SDL.SDL_Keycode.SDLK_g:
					return KeyboardKeys.g;
				case SDL.SDL_Keycode.SDLK_h:
					return KeyboardKeys.h;
				case SDL.SDL_Keycode.SDLK_i:
					return KeyboardKeys.i;
				case SDL.SDL_Keycode.SDLK_j:
					return KeyboardKeys.j;
				case SDL.SDL_Keycode.SDLK_k:
					return KeyboardKeys.k;
				case SDL.SDL_Keycode.SDLK_l:
					return KeyboardKeys.l;
				case SDL.SDL_Keycode.SDLK_m:
					return KeyboardKeys.m;
				case SDL.SDL_Keycode.SDLK_n:
					return KeyboardKeys.n;
				case SDL.SDL_Keycode.SDLK_o:
					return KeyboardKeys.o;
				case SDL.SDL_Keycode.SDLK_p:
					return KeyboardKeys.p;
				case SDL.SDL_Keycode.SDLK_q:
					return KeyboardKeys.q;
				case SDL.SDL_Keycode.SDLK_r:
					return KeyboardKeys.r;
				case SDL.SDL_Keycode.SDLK_s:
					return KeyboardKeys.s;
				case SDL.SDL_Keycode.SDLK_t:
					return KeyboardKeys.t;
				case SDL.SDL_Keycode.SDLK_u:
					return KeyboardKeys.u;
				case SDL.SDL_Keycode.SDLK_v:
					return KeyboardKeys.v;
				case SDL.SDL_Keycode.SDLK_w:
					return KeyboardKeys.w;
				case SDL.SDL_Keycode.SDLK_x:
					return KeyboardKeys.x;
				case SDL.SDL_Keycode.SDLK_y:
					return KeyboardKeys.y;
				case SDL.SDL_Keycode.SDLK_z:
					return KeyboardKeys.z;
				default:
					return KeyboardKeys.Enter;
			}
		}

		#endregion
		
	}
}