using System;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using ImGuiNET;

namespace RPG.DearImGUI {
	using System.Runtime.CompilerServices;
	using Engine.Core;
	using Engine.Graphics;
	using Engine.Input;
	using Engine.Utility;
	using Module.OpenGL;
	using Module.SDL2;

	public static class ImGUISystem {
		
		public static Shader Shader {
			get;
			set;
		}

		private static uint VertexBuffer {
			get;
			set;
		}

		private static uint IndexBuffer {
			get;
			set;
		}

		private static uint VertexArray {
			get;
			set;
		}

		private static int VertexBufferSize {
			get;
			set;
		}
		
		private static int IndexBufferSize {
			get;
			set;
		}

		public static IntPtr Context {
			get;
			private set;
		}

		public static Texture FontTexture {
			get;
			private set;
		}

		private static Vector2 DisplaySize {
			get;
			set;
		}

		public static void Initialize() {
			DisplaySize = Application.Instance.Project.WindowSize;
			Context = ImGui.CreateContext();
			ImGui.SetCurrentContext(Context);
			
			ImGuiIOPtr io = ImGui.GetIO();
			io.Fonts.AddFontDefault();
			io.DisplaySize = DisplaySize;
			io.ConfigFlags |= ImGuiConfigFlags.NavEnableKeyboard;
			io.ConfigFlags |= ImGuiConfigFlags.DockingEnable;

			CreateDeviceResources();
			SetKeyMapping();

			SetPerFrameImGuiData();
			
			Styling();
			
			ImGui.NewFrame();
		}

		#region Private Functions

		public static void CreateDeviceResources() {
			VertexArray = GL.GenVertexArray();
			VertexBuffer = GL.GenBuffer();
			IndexBuffer = GL.GenBuffer();

			VertexBufferSize = 10000;
			IndexBufferSize = 2000;
			
			GL.BindBuffer(GLEnum.ARRAY_BUFFER, VertexBuffer);
			GL.BufferData(GLEnum.ARRAY_BUFFER, (IntPtr)VertexBufferSize, IntPtr.Zero, GLEnum.DYNAMIC_DRAW);
			GL.BindBuffer(GLEnum.ARRAY_BUFFER, IndexBuffer);
			GL.BufferData(GLEnum.ARRAY_BUFFER, (IntPtr)IndexBufferSize, IntPtr.Zero, GLEnum.DYNAMIC_DRAW);
			
			RecreateFontDeviceTexture();

			Shader = Shader.DefaultImGui;
			
			GL.BindVertexArray(VertexArray);
			GL.BindBuffer(GLEnum.ARRAY_BUFFER, VertexBuffer);
			GL.BindBuffer(GLEnum.ELEMENT_ARRAY_BUFFER, IndexBuffer);

			int stride = Marshal.SizeOf<ImDrawVert>();
			GL.VertexAttribPointer(0, 2, GLEnum.FLOAT, false, stride, IntPtr.Zero);
			GL.EnableVertexAttribArray(0);
			GL.VertexAttribPointer(1, 2, GLEnum.FLOAT, false, stride, (IntPtr)8);
			GL.EnableVertexAttribArray(1);
			GL.VertexAttribPointer(2, 4, GLEnum.UNSIGNED_BYTE, true, stride, (IntPtr)16);
			GL.EnableVertexAttribArray(2);
			
			GL.BindVertexArray(0);
			GL.BindBuffer(GLEnum.ARRAY_BUFFER, 0);
			
		}

		private static void RecreateFontDeviceTexture() {
			ImGuiIOPtr io = ImGui.GetIO();
			io.Fonts.GetTexDataAsRGBA32(out IntPtr pixels, out int width, out int height, out int bytesPerPixel);

			byte[] data = new byte[width* height * bytesPerPixel];
			Marshal.Copy(pixels, data, 0, data.Length);
			
			// take the bytes returned by the font texture and then turn it into a texture
			FontTexture = new Texture(data, (uint)width, (uint)height, String.Empty, ColorType.RGBA);
			
			io.Fonts.SetTexID((IntPtr)FontTexture.ID);
			io.Fonts.ClearTexData();
		}

		public static void Render() {
			ImGui.Render();
			RenderImDrawData(ImGui.GetDrawData());
		}

		public static void Update() {
			//Input
			SetPerFrameImGuiData();
			UpdateInput();

			ImGui.NewFrame();
		}

		private static void SetPerFrameImGuiData() {
			ImGuiIOPtr io = ImGui.GetIO();
			io.DisplaySize = DisplaySize;
			io.DeltaTime = Time.Delta;
			io.DisplayFramebufferScale = Vector2.One;
		}

		private static void UpdateInput() {
			ImGuiIOPtr io = ImGui.GetIO();

			//Grab current state of inputs
			Mouse mouse = Application.Instance.InputModule.Mouse;
			Keyboard keyboard = Application.Instance.InputModule.Keyboard;
			
			io.AddMouseButtonEvent(0, mouse.IsDown(MouseButtons.Left));
			io.AddMouseButtonEvent(1, mouse.IsDown(MouseButtons.Right));
			io.AddMouseButtonEvent(2, mouse.IsDown(MouseButtons.Middle));
			
			io.AddMousePosEvent(mouse.Position.X, mouse.Position.Y);
			io.AddMouseWheelEvent(mouse.Wheel.X, mouse.Wheel.Y);

			for (int i = 0; i < keyboard.Down.Length; i++) {
				io.AddKeyEvent(KeyboardKeysToImGuiKey((SDL.SDL_Keycode)i), keyboard.Down[i]);
			}
			
			for (int i = 0; i < keyboard.Pressed.Length; i++) {
				if (keyboard.Pressed[i]) {
					io.AddInputCharacter((uint)i);
				}
			}

			io.AddKeyEvent(ImGuiKey.ModCtrl, keyboard.IsDown(KeyboardKeys.Control));
			io.AddKeyEvent(ImGuiKey.ModAlt, keyboard.IsDown(KeyboardKeys.Alt));
			io.AddKeyEvent(ImGuiKey.ModShift, keyboard.IsDown(KeyboardKeys.Shift));
			io.AddKeyEvent(ImGuiKey.ModSuper, keyboard.IsDown(KeyboardKeys.LeftSuper) || keyboard.IsDown(KeyboardKeys.RightSuper));
		}

		public static void RenderImDrawData(ImDrawDataPtr drawDataPtr) {
			if (drawDataPtr.CmdListsCount == 0) {
				return;
			}

			if (Application.Instance.EditorFramebuffer == null) {
				return;
			}
			
			//Bind Framebuffer for usage
			GL.BindFramebuffer(GLEnum.FRAMEBUFFER, Application.Instance.EditorFramebuffer.Id);
			GL.Enable(GLEnum.DEPTH_TEST);
			
			//Clear Framebuffer
			Color clearColor = Color.MediumVioletRed;
			GL.ClearColor((float)clearColor.R / 255, (float)clearColor.G / 255, (float)clearColor.B / 255, 1);
			GL.Clear(GLEnum.COLOR_BUFFER_BIT | GLEnum.DEPTH_BUFFER_BIT);

			uint vertexOffsetInVertices = 0;
			uint indexOffsetInElements = 0;
			
			uint totalVBSize = (uint)(drawDataPtr.TotalVtxCount * Marshal.SizeOf<ImDrawVert>());
			if (totalVBSize > VertexBufferSize) {
				int newSize = (int) Math.Max(VertexBufferSize * 1.5f, totalVBSize);

				GL.BindBuffer(GLEnum.ARRAY_BUFFER, VertexBuffer);
				GL.BufferData(GLEnum.ARRAY_BUFFER, (IntPtr)newSize, IntPtr.Zero, GLEnum.DYNAMIC_DRAW);
				GL.BindBuffer(GLEnum.ARRAY_BUFFER, 0);
				
				VertexBufferSize = newSize;
				
				//Debug.Log("ImGui", $"Resized vertex buffer to new size {VertexBufferSize}");
			}

			uint totalIBSize = (uint)(drawDataPtr.TotalIdxCount * sizeof(ushort));
			if (totalIBSize > IndexBufferSize) {
				int newSize = (int)Math.Max(IndexBufferSize * 1.5f, totalIBSize);

				GL.BindBuffer(GLEnum.ARRAY_BUFFER, IndexBuffer);
				GL.BufferData(GLEnum.ARRAY_BUFFER, (IntPtr)newSize, IntPtr.Zero, GLEnum.DYNAMIC_DRAW);
				GL.BindBuffer(GLEnum.ARRAY_BUFFER, 0);

				IndexBufferSize = newSize;

				//Debug.Log("ImGui", $"Resized index buffer to new size {IndexBufferSize}");
			}
			
			for (int i = 0; i < drawDataPtr.CmdListsCount; i++) {
				ImDrawListPtr cmd_list = drawDataPtr.CmdListsRange[i];

				GL.BindBuffer(GLEnum.ARRAY_BUFFER, VertexBuffer);
				GL.BufferSubData(GLEnum.ARRAY_BUFFER, (IntPtr)(vertexOffsetInVertices * Marshal.SizeOf<ImDrawVert>()), (IntPtr)(cmd_list.VtxBuffer.Size * Marshal.SizeOf<ImDrawVert>()), cmd_list.VtxBuffer.Data);


				GL.BindBuffer(GLEnum.ARRAY_BUFFER, IndexBuffer);
				GL.BufferSubData(GLEnum.ARRAY_BUFFER, (IntPtr)(indexOffsetInElements * sizeof(ushort)), (IntPtr)(cmd_list.IdxBuffer.Size * sizeof(ushort)), cmd_list.IdxBuffer.Data);


				vertexOffsetInVertices += (uint)cmd_list.VtxBuffer.Size;
				indexOffsetInElements += (uint)cmd_list.IdxBuffer.Size;
			}
			GL.BindBuffer(GLEnum.ARRAY_BUFFER, 0);

			ImGuiIOPtr io = ImGui.GetIO();
			Matrix4x4 mvp = Matrix4x4.CreateOrthographicOffCenter(0f, io.DisplaySize.X, io.DisplaySize.Y, 0.0f, -1.0f, 1.0f);
			
			Shader.Use();
			Shader.SetInt("fontTexture", 0);
			Shader.SetMatrix4x4("projection", mvp);
			
			GL.BindVertexArray(VertexArray);

			drawDataPtr.ScaleClipRects(io.DisplayFramebufferScale);
			
			GL.Enable(GLEnum.BLEND);
			GL.Enable(GLEnum.SCISSOR_TEST);
			GL.BlendEquation(GLEnum.FUNC_ADD);
			GL.BlendFunc(GLEnum.SRC_ALPHA, GLEnum.ONE_MINUS_SRC_ALPHA);
			GL.Disable(GLEnum.CULL_FACE);
			GL.Disable(GLEnum.DEPTH_TEST);
			
			// Render command lists
			int vtx_offset = 0;
			int idx_offset = 0;
			for (int n = 0; n < drawDataPtr.CmdListsCount; n++) {
				ImDrawListPtr cmd_list = drawDataPtr.CmdListsRange[n];
				for (int cmd_i = 0; cmd_i < cmd_list.CmdBuffer.Size; cmd_i++) {
					ImDrawCmdPtr pcmd = cmd_list.CmdBuffer[cmd_i];
					if (pcmd.UserCallback != IntPtr.Zero) {
						throw new NotImplementedException();
					}
					
					GL.ActiveTexture((uint) GLEnum.TEXTURE0);
					GL.BindTexture(GLEnum.TEXTURE_2D, (uint)pcmd.TextureId);

					// We do _windowHeight - (int)clip.W instead of (int)clip.Y because gl has flipped Y when it comes to these coordinates
					var clip = pcmd.ClipRect;
					GL.Scissor((int)clip.X, Application.Instance.Project.WindowHeight - (int)clip.W, (int)(clip.Z - clip.X), (int)(clip.W - clip.Y));

					GL.DrawElementsBaseVertex(GLEnum.TRIANGLES, (int)pcmd.ElemCount, GLEnum.UNSIGNED_SHORT, (IntPtr)(idx_offset * sizeof(ushort)), vtx_offset);

					idx_offset += (int)pcmd.ElemCount;
				}
				vtx_offset += cmd_list.VtxBuffer.Size;
			}

			GL.Disable(GLEnum.BLEND);
			GL.Disable(GLEnum.SCISSOR_TEST);
			Shader.EndUse();
			
			GL.BindVertexArray(0);
			GL.BindFramebuffer(GLEnum.FRAMEBUFFER, 0);
		}

		private static void SetKeyMapping() {
			//TODO: Remove SDL reference for something a bit more framework general
			
			ImGuiIOPtr io = ImGui.GetIO();
			io.KeyMap[(int)ImGuiKey.Tab] = (int)KeyboardKeysToImGuiKey(SDL.SDL_Keycode.SDLK_TAB);
			io.KeyMap[(int)ImGuiKey.LeftArrow] = (int)KeyboardKeysToImGuiKey(SDL.SDL_Keycode.SDLK_LEFT);
			io.KeyMap[(int)ImGuiKey.RightArrow] = (int)KeyboardKeysToImGuiKey(SDL.SDL_Keycode.SDLK_RIGHT);
			io.KeyMap[(int)ImGuiKey.UpArrow] = (int)KeyboardKeysToImGuiKey(SDL.SDL_Keycode.SDLK_UP);
			io.KeyMap[(int)ImGuiKey.DownArrow] = (int)KeyboardKeysToImGuiKey(SDL.SDL_Keycode.SDLK_DOWN);
			io.KeyMap[(int)ImGuiKey.PageUp] = (int)KeyboardKeysToImGuiKey(SDL.SDL_Keycode.SDLK_PAGEUP);
			io.KeyMap[(int)ImGuiKey.PageDown] = (int)KeyboardKeysToImGuiKey(SDL.SDL_Keycode.SDLK_PAGEDOWN);
			io.KeyMap[(int)ImGuiKey.Home] = (int)KeyboardKeysToImGuiKey(SDL.SDL_Keycode.SDLK_HOME);
			io.KeyMap[(int)ImGuiKey.End] = (int)KeyboardKeysToImGuiKey(SDL.SDL_Keycode.SDLK_END);
			io.KeyMap[(int)ImGuiKey.Delete] = (int)KeyboardKeysToImGuiKey(SDL.SDL_Keycode.SDLK_DELETE);
			io.KeyMap[(int)ImGuiKey.Backspace] = (int)KeyboardKeysToImGuiKey(SDL.SDL_Keycode.SDLK_BACKSPACE);
			io.KeyMap[(int)ImGuiKey.Enter] = (int)KeyboardKeysToImGuiKey(SDL.SDL_Keycode.SDLK_RETURN);
			io.KeyMap[(int)ImGuiKey.Escape] = (int)KeyboardKeysToImGuiKey(SDL.SDL_Keycode.SDLK_ESCAPE);
			io.KeyMap[(int)ImGuiKey.A] = (int)KeyboardKeysToImGuiKey(SDL.SDL_Keycode.SDLK_a);
			io.KeyMap[(int)ImGuiKey.C] = (int)KeyboardKeysToImGuiKey(SDL.SDL_Keycode.SDLK_c);
			io.KeyMap[(int)ImGuiKey.V] = (int)KeyboardKeysToImGuiKey(SDL.SDL_Keycode.SDLK_v);
			io.KeyMap[(int)ImGuiKey.X] = (int)KeyboardKeysToImGuiKey(SDL.SDL_Keycode.SDLK_x);
			io.KeyMap[(int)ImGuiKey.Y] = (int)KeyboardKeysToImGuiKey(SDL.SDL_Keycode.SDLK_y);
			io.KeyMap[(int)ImGuiKey.Z] = (int)KeyboardKeysToImGuiKey(SDL.SDL_Keycode.SDLK_z);
		}

		private static void Styling() {
			ImGuiStylePtr style = ImGui.GetStyle();
			
			style.Colors[(int)ImGuiCol.Text]                   = new Vector4(1.000f, 1.000f, 1.000f, 1.000f);
			style.Colors[(int)ImGuiCol.TextDisabled]           = new Vector4(0.500f, 0.500f, 0.500f, 1.000f);
			style.Colors[(int)ImGuiCol.WindowBg]               = new Vector4(0.180f, 0.180f, 0.180f, 1.000f);
			style.Colors[(int)ImGuiCol.ChildBg]                = new Vector4(0.280f, 0.280f, 0.280f, 0.000f);
			style.Colors[(int)ImGuiCol.PopupBg]                = new Vector4(0.313f, 0.313f, 0.313f, 1.000f);
			style.Colors[(int)ImGuiCol.Border]                 = new Vector4(0.266f, 0.266f, 0.266f, 1.000f);
			style.Colors[(int)ImGuiCol.BorderShadow]           = new Vector4(0.000f, 0.000f, 0.000f, 0.000f);
			style.Colors[(int)ImGuiCol.FrameBg]                = new Vector4(0.160f, 0.160f, 0.160f, 1.000f);
			style.Colors[(int)ImGuiCol.FrameBgHovered]         = new Vector4(0.200f, 0.200f, 0.200f, 1.000f);
			style.Colors[(int)ImGuiCol.FrameBgActive]          = new Vector4(0.280f, 0.280f, 0.280f, 1.000f);
			style.Colors[(int)ImGuiCol.TitleBg]                = new Vector4(0.148f, 0.148f, 0.148f, 1.000f);
			style.Colors[(int)ImGuiCol.TitleBgActive]          = new Vector4(0.148f, 0.148f, 0.148f, 1.000f);
			style.Colors[(int)ImGuiCol.TitleBgCollapsed]       = new Vector4(0.148f, 0.148f, 0.148f, 1.000f);
			style.Colors[(int)ImGuiCol.MenuBarBg]              = new Vector4(0.195f, 0.195f, 0.195f, 1.000f);
			style.Colors[(int)ImGuiCol.ScrollbarBg]            = new Vector4(0.160f, 0.160f, 0.160f, 1.000f);
			style.Colors[(int)ImGuiCol.ScrollbarGrab]          = new Vector4(0.277f, 0.277f, 0.277f, 1.000f);
			style.Colors[(int)ImGuiCol.ScrollbarGrabHovered]   = new Vector4(0.300f, 0.300f, 0.300f, 1.000f);
			style.Colors[(int)ImGuiCol.ScrollbarGrabActive]    = new Vector4(1.000f, 0.391f, 0.000f, 1.000f);
			style.Colors[(int)ImGuiCol.CheckMark]              = new Vector4(1.000f, 1.000f, 1.000f, 1.000f);
			style.Colors[(int)ImGuiCol.SliderGrab]             = new Vector4(0.391f, 0.391f, 0.391f, 1.000f);
			style.Colors[(int)ImGuiCol.SliderGrabActive]       = new Vector4(1.000f, 0.391f, 0.000f, 1.000f);
			style.Colors[(int)ImGuiCol.Button]                 = new Vector4(1.000f, 1.000f, 1.000f, 0.000f);
			style.Colors[(int)ImGuiCol.ButtonHovered]          = new Vector4(1.000f, 1.000f, 1.000f, 0.156f);
			style.Colors[(int)ImGuiCol.ButtonActive]           = new Vector4(1.000f, 1.000f, 1.000f, 0.391f);
			style.Colors[(int)ImGuiCol.Header]                 = new Vector4(0.313f, 0.313f, 0.313f, 1.000f);
			style.Colors[(int)ImGuiCol.HeaderHovered]          = new Vector4(0.469f, 0.469f, 0.469f, 1.000f);
			style.Colors[(int)ImGuiCol.HeaderActive]           = new Vector4(0.469f, 0.469f, 0.469f, 1.000f);
			style.Colors[(int)ImGuiCol.Separator]              = new Vector4(0.266f, 0.266f, 0.266f, 1.000f);
			style.Colors[(int)ImGuiCol.SeparatorHovered]       = new Vector4(0.391f, 0.391f, 0.391f, 1.000f);
			style.Colors[(int)ImGuiCol.SeparatorActive]        = new Vector4(1.000f, 0.391f, 0.000f, 1.000f);
			style.Colors[(int)ImGuiCol.ResizeGrip]             = new Vector4(1.000f, 1.000f, 1.000f, 0.250f);
			style.Colors[(int)ImGuiCol.ResizeGripHovered]      = new Vector4(1.000f, 1.000f, 1.000f, 0.670f);
			style.Colors[(int)ImGuiCol.ResizeGripActive]       = new Vector4(1.000f, 0.391f, 0.000f, 1.000f);
			style.Colors[(int)ImGuiCol.Tab]                    = new Vector4(0.098f, 0.098f, 0.098f, 1.000f);
			style.Colors[(int)ImGuiCol.TabHovered]             = new Vector4(0.352f, 0.352f, 0.352f, 1.000f);
			style.Colors[(int)ImGuiCol.TabActive]              = new Vector4(0.195f, 0.195f, 0.195f, 1.000f);
			style.Colors[(int)ImGuiCol.TabUnfocused]           = new Vector4(0.098f, 0.098f, 0.098f, 1.000f);
			style.Colors[(int)ImGuiCol.TabUnfocusedActive]     = new Vector4(0.195f, 0.195f, 0.195f, 1.000f);
			style.Colors[(int)ImGuiCol.DockingPreview]         = new Vector4(1.000f, 0.391f, 0.000f, 0.781f);
			style.Colors[(int)ImGuiCol.DockingEmptyBg]         = new Vector4(0.180f, 0.180f, 0.180f, 1.000f);
			style.Colors[(int)ImGuiCol.PlotLines]              = new Vector4(0.469f, 0.469f, 0.469f, 1.000f);
			style.Colors[(int)ImGuiCol.PlotLinesHovered]       = new Vector4(1.000f, 0.391f, 0.000f, 1.000f);
			style.Colors[(int)ImGuiCol.PlotHistogram]          = new Vector4(0.586f, 0.586f, 0.586f, 1.000f);
			style.Colors[(int)ImGuiCol.PlotHistogramHovered]   = new Vector4(1.000f, 0.391f, 0.000f, 1.000f);
			style.Colors[(int)ImGuiCol.TextSelectedBg]         = new Vector4(1.000f, 1.000f, 1.000f, 0.156f);
			style.Colors[(int)ImGuiCol.DragDropTarget]         = new Vector4(1.000f, 0.391f, 0.000f, 1.000f);
			style.Colors[(int)ImGuiCol.NavHighlight]           = new Vector4(1.000f, 0.391f, 0.000f, 1.000f);
			style.Colors[(int)ImGuiCol.NavWindowingHighlight]  = new Vector4(1.000f, 0.391f, 0.000f, 1.000f);
			style.Colors[(int)ImGuiCol.NavWindowingDimBg]      = new Vector4(0.000f, 0.000f, 0.000f, 0.586f);
			style.Colors[(int)ImGuiCol.ModalWindowDimBg]       = new Vector4(0.000f, 0.000f, 0.000f, 0.586f);

			style.ChildRounding = 4.0f;
			style.FrameBorderSize = 1.0f;
			style.FrameRounding = 2.0f;
			style.GrabMinSize = 7.0f;
			style.PopupRounding = 2.0f;
			style.ScrollbarRounding = 12.0f;
			style.ScrollbarSize = 13.0f;
			style.TabBorderSize = 1.0f;
			style.TabRounding = 0.0f;
			style.WindowRounding = 4.0f;
		}

		private static ImGuiKey KeyboardKeysToImGuiKey(SDL.SDL_Keycode keycode) {
			switch (keycode) {
				case SDL.SDL_Keycode.SDLK_TAB: return ImGuiKey.Tab;
				case SDL.SDL_Keycode.SDLK_LEFT: return ImGuiKey.LeftArrow;
				case SDL.SDL_Keycode.SDLK_RIGHT: return ImGuiKey.RightArrow;
				case SDL.SDL_Keycode.SDLK_UP: return ImGuiKey.UpArrow;
				case SDL.SDL_Keycode.SDLK_DOWN: return ImGuiKey.DownArrow;
				case SDL.SDL_Keycode.SDLK_PAGEUP: return ImGuiKey.PageUp;
				case SDL.SDL_Keycode.SDLK_PAGEDOWN: return ImGuiKey.PageDown;
				case SDL.SDL_Keycode.SDLK_HOME: return ImGuiKey.Home;
				case SDL.SDL_Keycode.SDLK_END: return ImGuiKey.End;
				case SDL.SDL_Keycode.SDLK_INSERT: return ImGuiKey.Insert;
				case SDL.SDL_Keycode.SDLK_DELETE: return ImGuiKey.Delete;
				case SDL.SDL_Keycode.SDLK_BACKSPACE: return ImGuiKey.Backspace;
				case SDL.SDL_Keycode.SDLK_SPACE: return ImGuiKey.Space;
				case SDL.SDL_Keycode.SDLK_RETURN: return ImGuiKey.Enter;
				case SDL.SDL_Keycode.SDLK_ESCAPE: return ImGuiKey.Escape;
				case SDL.SDL_Keycode.SDLK_QUOTE: return ImGuiKey.Apostrophe;
				case SDL.SDL_Keycode.SDLK_COMMA: return ImGuiKey.Comma;
				case SDL.SDL_Keycode.SDLK_MINUS: return ImGuiKey.Minus;
				case SDL.SDL_Keycode.SDLK_PERIOD: return ImGuiKey.Period;
				case SDL.SDL_Keycode.SDLK_SLASH: return ImGuiKey.Slash;
				case SDL.SDL_Keycode.SDLK_SEMICOLON: return ImGuiKey.Semicolon;
				case SDL.SDL_Keycode.SDLK_EQUALS: return ImGuiKey.Equal;
				case SDL.SDL_Keycode.SDLK_LEFTBRACKET: return ImGuiKey.LeftBracket;
				case SDL.SDL_Keycode.SDLK_BACKSLASH: return ImGuiKey.Backslash;
				case SDL.SDL_Keycode.SDLK_RIGHTBRACKET: return ImGuiKey.RightBracket;
				case SDL.SDL_Keycode.SDLK_BACKQUOTE: return ImGuiKey.GraveAccent;
				case SDL.SDL_Keycode.SDLK_CAPSLOCK: return ImGuiKey.CapsLock;
				case SDL.SDL_Keycode.SDLK_SCROLLLOCK: return ImGuiKey.ScrollLock;
				case SDL.SDL_Keycode.SDLK_NUMLOCKCLEAR: return ImGuiKey.NumLock;
				case SDL.SDL_Keycode.SDLK_PRINTSCREEN: return ImGuiKey.PrintScreen;
				case SDL.SDL_Keycode.SDLK_PAUSE: return ImGuiKey.Pause;
				case SDL.SDL_Keycode.SDLK_KP_0: return ImGuiKey.Keypad0;
				case SDL.SDL_Keycode.SDLK_KP_1: return ImGuiKey.Keypad1;
				case SDL.SDL_Keycode.SDLK_KP_2: return ImGuiKey.Keypad2;
				case SDL.SDL_Keycode.SDLK_KP_3: return ImGuiKey.Keypad3;
				case SDL.SDL_Keycode.SDLK_KP_4: return ImGuiKey.Keypad4;
				case SDL.SDL_Keycode.SDLK_KP_5: return ImGuiKey.Keypad5;
				case SDL.SDL_Keycode.SDLK_KP_6: return ImGuiKey.Keypad6;
				case SDL.SDL_Keycode.SDLK_KP_7: return ImGuiKey.Keypad7;
				case SDL.SDL_Keycode.SDLK_KP_8: return ImGuiKey.Keypad8;
				case SDL.SDL_Keycode.SDLK_KP_9: return ImGuiKey.Keypad9;
				case SDL.SDL_Keycode.SDLK_KP_PERIOD: return ImGuiKey.KeypadDecimal;
				case SDL.SDL_Keycode.SDLK_KP_DIVIDE: return ImGuiKey.KeypadDivide;
				case SDL.SDL_Keycode.SDLK_KP_MULTIPLY: return ImGuiKey.KeypadMultiply;
				case SDL.SDL_Keycode.SDLK_KP_MINUS: return ImGuiKey.KeypadSubtract;
				case SDL.SDL_Keycode.SDLK_KP_PLUS: return ImGuiKey.KeypadAdd;
				case SDL.SDL_Keycode.SDLK_KP_ENTER: return ImGuiKey.KeypadEnter;
				case SDL.SDL_Keycode.SDLK_KP_EQUALS: return ImGuiKey.KeypadEqual;
				case SDL.SDL_Keycode.SDLK_LCTRL: return ImGuiKey.LeftCtrl;
				case SDL.SDL_Keycode.SDLK_LSHIFT: return ImGuiKey.LeftShift;
				case SDL.SDL_Keycode.SDLK_LALT: return ImGuiKey.LeftAlt;
				case SDL.SDL_Keycode.SDLK_LGUI: return ImGuiKey.LeftSuper;
				case SDL.SDL_Keycode.SDLK_RCTRL: return ImGuiKey.RightCtrl;
				case SDL.SDL_Keycode.SDLK_RSHIFT: return ImGuiKey.RightShift;
				case SDL.SDL_Keycode.SDLK_RALT: return ImGuiKey.RightAlt;
				case SDL.SDL_Keycode.SDLK_RGUI: return ImGuiKey.RightSuper;
				case SDL.SDL_Keycode.SDLK_APPLICATION: return ImGuiKey.Menu;
				case SDL.SDL_Keycode.SDLK_0: return ImGuiKey._0;
				case SDL.SDL_Keycode.SDLK_1: return ImGuiKey._1;
				case SDL.SDL_Keycode.SDLK_2: return ImGuiKey._2;
				case SDL.SDL_Keycode.SDLK_3: return ImGuiKey._3;
				case SDL.SDL_Keycode.SDLK_4: return ImGuiKey._4;
				case SDL.SDL_Keycode.SDLK_5: return ImGuiKey._5;
				case SDL.SDL_Keycode.SDLK_6: return ImGuiKey._6;
				case SDL.SDL_Keycode.SDLK_7: return ImGuiKey._7;
				case SDL.SDL_Keycode.SDLK_8: return ImGuiKey._8;
				case SDL.SDL_Keycode.SDLK_9: return ImGuiKey._9;
				case SDL.SDL_Keycode.SDLK_a: return ImGuiKey.A;
				case SDL.SDL_Keycode.SDLK_b: return ImGuiKey.B;
				case SDL.SDL_Keycode.SDLK_c: return ImGuiKey.C;
				case SDL.SDL_Keycode.SDLK_d: return ImGuiKey.D;
				case SDL.SDL_Keycode.SDLK_e: return ImGuiKey.E;
				case SDL.SDL_Keycode.SDLK_f: return ImGuiKey.F;
				case SDL.SDL_Keycode.SDLK_g: return ImGuiKey.G;
				case SDL.SDL_Keycode.SDLK_h: return ImGuiKey.H;
				case SDL.SDL_Keycode.SDLK_i: return ImGuiKey.I;
				case SDL.SDL_Keycode.SDLK_j: return ImGuiKey.J;
				case SDL.SDL_Keycode.SDLK_k: return ImGuiKey.K;
				case SDL.SDL_Keycode.SDLK_l: return ImGuiKey.L;
				case SDL.SDL_Keycode.SDLK_m: return ImGuiKey.M;
				case SDL.SDL_Keycode.SDLK_n: return ImGuiKey.N;
				case SDL.SDL_Keycode.SDLK_o: return ImGuiKey.O;
				case SDL.SDL_Keycode.SDLK_p: return ImGuiKey.P;
				case SDL.SDL_Keycode.SDLK_q: return ImGuiKey.Q;
				case SDL.SDL_Keycode.SDLK_r: return ImGuiKey.R;
				case SDL.SDL_Keycode.SDLK_s: return ImGuiKey.S;
				case SDL.SDL_Keycode.SDLK_t: return ImGuiKey.T;
				case SDL.SDL_Keycode.SDLK_u: return ImGuiKey.U;
				case SDL.SDL_Keycode.SDLK_v: return ImGuiKey.V;
				case SDL.SDL_Keycode.SDLK_w: return ImGuiKey.W;
				case SDL.SDL_Keycode.SDLK_x: return ImGuiKey.X;
				case SDL.SDL_Keycode.SDLK_y: return ImGuiKey.Y;
				case SDL.SDL_Keycode.SDLK_z: return ImGuiKey.Z;
				case SDL.SDL_Keycode.SDLK_F1: return ImGuiKey.F1;
				case SDL.SDL_Keycode.SDLK_F2: return ImGuiKey.F2;
				case SDL.SDL_Keycode.SDLK_F3: return ImGuiKey.F3;
				case SDL.SDL_Keycode.SDLK_F4: return ImGuiKey.F4;
				case SDL.SDL_Keycode.SDLK_F5: return ImGuiKey.F5;
				case SDL.SDL_Keycode.SDLK_F6: return ImGuiKey.F6;
				case SDL.SDL_Keycode.SDLK_F7: return ImGuiKey.F7;
				case SDL.SDL_Keycode.SDLK_F8: return ImGuiKey.F8;
				case SDL.SDL_Keycode.SDLK_F9: return ImGuiKey.F9;
				case SDL.SDL_Keycode.SDLK_F10: return ImGuiKey.F10;
				case SDL.SDL_Keycode.SDLK_F11: return ImGuiKey.F11;
				case SDL.SDL_Keycode.SDLK_F12: return ImGuiKey.F12;
			}
			
			return ImGuiKey.None;
		}

		#endregion
		
	}
}