//#define DEBUG_THIS
#define ONLY_REPAINT
using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Reflection;



namespace EMX.HierarchyPlugin.Editor
{

	class GlDrawer
	{
		PluginInstance p;
		internal GlDrawer(PluginInstance p)
		{
			this.p = p;
			drawCalls = new DrawCall[2] { new DrawCall(this), new DrawCall(this) };
		}




		public void DrawStackItem(DrawStack drawStack)
		{
			////old draer

			//  wasRest = null;
			dc = drawStack.drawCallIndex;
			BAKE_DRAWER = !disableBatching &&
#if ONLY_REPAINT
			p.EVENT.type == EventType.Repaint &&
#endif
				p.par_e.USE_DINAMIC_BATCHING;
			for (int i = 0; i < drawStack.Count; i++)
			{
				drawStack.DrawSIngleItem(drawStack.stack[i]);
			}
			BAKE_DRAWER = false;


			/*var o = drawStack.current_object;
			for ( int i = 0 ; i <drawStack.Count ; i++ )
			{
				drawStack.stack[ i ].type
				///DrawSIngleItem( stack[ i ], o );
			}

			drawStack
			throw new NotImplementedException();*/
		}
		bool BAKE_DRAWER = false;
		public bool disableBatching = false;
		internal void ReleaseStack()
		{
			BAKE_DRAWER = false;
			drawCalls[0].ReleaseStack();
			if (drawFade)
			{
				drawFade = false;
				EditorGUI.DrawRect(fadeRect, fadeColor);
			}
			drawCalls[1].ReleaseStack();
		}
		class DrawCall
		{
			GlDrawer drawer;
			internal DrawCall(GlDrawer drawer)
			{
				this.drawer = drawer;
			}
			internal void ReleaseStack()
			{
#if ONLY_REPAINT
				if (drawer.p.EVENT.type != EventType.Repaint) return;
#endif
				for (int i = 0; i < style_stack.glCount; i++) drawer._DrawStyleWithText(ref style_stack.glStack[i]);
				style_stack.glCount = 0;
				//
				if (unsorted_glTexture_stacks.Count > temp_stack_a.Length) Array.Resize(ref temp_stack_a, unsorted_glTexture_stacks.Count);
				foreach (var item in unsorted_glTexture_stacks) temp_stack_a[item.Value.tex_sorted_index] = item.Value;

				var nulltexture = -1;
				for (int i = 0; i < unsorted_glTexture_stacks.Count; i++) if (temp_stack_a[i].emptyTexture == -1) nulltexture = i;
				if (nulltexture > 0)
				{
					var temp = temp_stack_a[nulltexture];
					for (int i = nulltexture; i > 0; i--) temp_stack_a[i] = temp_stack_a[i - 1];
					temp_stack_a[0] = temp;
				}

				//int count  = 0;
				for (int i = 0, L = unsorted_glTexture_stacks.Count; i < L; i++)
				{
					var S = temp_stack_a[i];
					//Debug.Log(S.glStack.Count);
					if (S.glStack.Count > temp_stack_b.Length) Array.Resize(ref temp_stack_b, S.glStack.Count);
					foreach (var item in S.glStack) temp_stack_b[item.Value.mat_sorted_index] = item.Value;
					for (int x = 0, L2 = S.glStack.Count; x < L2; x++)
					{
						var R = temp_stack_b[x];
						var t = R.tex != null ? R.tex.texture : null;
						if (x == 0)
						{
							if (!R.mat)
							{
								//  Debug.LogWarning("No material " + R.tex);
								ClearStacks();
								Root.p[0].RESET_DRAWSTACK(Root.p[0].pluginID);
								Root.p[0].RepaintWindow(Root.p[0].pluginID, true);
								return;
							}
						}
						/*if (i == 0)
						{
							additionalMaterial().SetTexture(_MainTex, t);
							additionalMaterial().SetPass(0);
							//break;
						}
						else
						{
							R.mat.SetTexture(_MainTex, t);
							R.mat.SetPass(0);
						}*/
						R.mat.SetTexture(_MainTex, t);
						R.mat.SetPass(0);

						GL.PushMatrix();
						GL.Begin(GL.QUADS);

						for (int z = 0; z < R.glCount; z++)
						{

							if (R.glStack[z].border <= 0 || !t) drawer.draw_simple_quad_fast(R.glStack[z].rect, ref R.glStack[z].col, R.glStack[z].tex);
							else drawer.__DrawTexture(ref R.glStack[z].rect, R.glStack[z].tex, ref R.glStack[z].col, R.glStack[z].border, R.mat, ref drawer.nullClip, fast: true);
						}
						R.glCount = 0;
						//  count++;
						GL.End();
						GL.PopMatrix();
						//GL.Clear(false, true,Color.clear);

					}
				}
				// Debug.Log(count);
				//
				for (int i = 0; i < label_stack.glCount; i++) drawer._DrawLabel(ref label_stack.glStack[i]);
				label_stack.glCount = 0;
				for (int i = 0; i < button_stack.glCount; i++) drawer._DrawButton(ref button_stack.glStack[i]);
				button_stack.glCount = 0;
			}

			internal GLTextureStack[] temp_stack_a = new GLTextureStack[20];
			internal GLTextureStackAndMaterial[] temp_stack_b = new GLTextureStackAndMaterial[20];
			internal Dictionary<int, GLTextureStack> unsorted_glTexture_stacks = new Dictionary<int, GLTextureStack>() { { -1, new GLTextureStack(0) } };
			internal GLLabelStack label_stack = new GLLabelStack();
			internal GLButtonStack button_stack = new GLButtonStack();
			internal GLStyleStack style_stack = new GLStyleStack();


			internal void ClearStacks()
			{
				foreach (var item in unsorted_glTexture_stacks)
					item.Value.glStack.Clear();
				style_stack.glCount = 0;
				label_stack.glCount = 0;
				button_stack.glCount = 0;

			}
		}
		internal void ClearStacks()
		{
			foreach (var item in drawCalls)
			{
				if (item == null) continue;
				item.ClearStacks();
			}
		}

		int dc = 0;
		/*  int dc = -1;
          internal void SET_DRAW_CALL()
          {
              dc++;
              if (dc >= drawCalls.Length) throw new Exception("dc");
          }*/
		DrawCall[] drawCalls;

		bool drawFade = false;
		Rect fadeRect;
		Color fadeColor;
		internal void DrawFade(Rect r, Color c)
		{
			drawFade = true;
			fadeRect = r;
			fadeColor = c;
		}


		static int _MainTex = Shader.PropertyToID("_MainTex");
		static int colorProperty = Shader.PropertyToID("_Color");
		Material _defaultMaterial;
		Material defaultMaterial()
		{
			if (!_defaultMaterial)
				_defaultMaterial = new Material(p.DEFAULT_SHADER_SHADER.ExternalMaterialReference);
			return _defaultMaterial;
		}
		static Material _additionalMaterial;
		static Material additionalMaterial()
		{
			if (!_additionalMaterial)
				_additionalMaterial = new Material(Root.p[0].DEFAULT_SHADER_SHADER.ExternalMaterialReference);
			return _additionalMaterial;
		}
		Vector3 tv3_a;
		Vector3 tv3_b;
		Vector3 tv3_c;
		Vector3 tv3_d;

		void DoClip(ref Rect rect, ref Rect? clipRect)
		{
			var oX = rect.x;
			var oY = rect.y;
			if (rect.x < clipRect.Value.x) rect.x = clipRect.Value.x;
			if (rect.x > clipRect.Value.x + clipRect.Value.width) rect.x = clipRect.Value.x + clipRect.Value.width;
			if (rect.y < clipRect.Value.y) rect.y = clipRect.Value.y;
			if (rect.y > clipRect.Value.y + clipRect.Value.height) rect.y = clipRect.Value.y + clipRect.Value.height;
			rect.width = Mathf.Max(0, rect.width - (rect.x - oX));
			rect.height = Mathf.Max(0, rect.height - (rect.y - oY));
			if (rect.width + rect.x > clipRect.Value.x + clipRect.Value.width) rect.width = Mathf.Max(0, clipRect.Value.x + clipRect.Value.width - rect.x);
			if (rect.height + rect.y > clipRect.Value.y + clipRect.Value.height) rect.height = Mathf.Max(0, clipRect.Value.y + clipRect.Value.height - rect.y);

		}

		void DoClip(ref Rect rect, Vector2 uv_start, Vector2 uv_end, ref Rect? clipRect)
		{

			var or = rect;
			DoClip(ref rect, ref clipRect);

			if (or.xMax != rect.xMax || or.xMin != rect.xMin)
			{
				var d = or.xMax - or.xMin;
				var l_A = (rect.xMin - or.xMin) / d;
				var l_B = (rect.xMax - or.xMin) / d;
				uv_start.x = Mathf.LerpUnclamped(uv_start.x, uv_end.x, l_A);
				uv_end.x = Mathf.LerpUnclamped(uv_start.x, uv_end.x, l_B);
			}
			if (or.yMax != rect.yMax || or.yMin != rect.yMin)
			{
				var d = or.yMax - or.yMin;
				var l_A = (rect.yMin - or.yMin) / d;
				var l_B = (rect.yMax - or.yMin) / d;
				uv_start.y = Mathf.LerpUnclamped(uv_start.y, uv_end.y, l_A);
				uv_end.y = Mathf.LerpUnclamped(uv_start.y, uv_end.y, l_B);
			}

			tv3_a.Set(uv_start.x, uv_start.y, 0);
			tv3_b.Set(uv_start.x, uv_end.y, 0);
			tv3_c.Set(uv_end.x, uv_end.y, 0);
			tv3_d.Set(uv_end.x, uv_start.y, 0);

		}


		void draw_simple_quad(Rect rect, ref Color c, IconData tex = null, Rect? clipRect = null)
		{
			if (rect.width <= 0 || rect.height <= 0) return;


			//  p.par_e.DEFAULT_SHADER_SHADER.ExternalMaterialReference.SetPass( 0 );
			GL.PushMatrix();
			///  GL.Clear(true, false, Color.black);
			GL.Begin(GL.QUADS);

			GL.Color(c);






			if (tex != null)
			{

				if (clipRect.HasValue)
				{
					DoClip(ref rect, tex.uv_start, tex.uv_end, ref clipRect);
				}
				else
				{
					tv3_a.Set(tex.uv_start.x, tex.uv_start.y, 0);
					tv3_b.Set(tex.uv_start.x, tex.uv_end.y, 0);
					tv3_c.Set(tex.uv_end.x, tex.uv_end.y, 0);
					tv3_d.Set(tex.uv_end.x, tex.uv_start.y, 0);
				}


				GL.TexCoord(tv3_a);
				GL.Vertex3(rect.x, rect.y, 0);
				GL.TexCoord(tv3_b);
				GL.Vertex3(rect.x, rect.y + rect.height, 0);
				GL.TexCoord(tv3_c);
				GL.Vertex3(rect.x + rect.width, rect.y + rect.height, 0);
				GL.TexCoord(tv3_d);
				GL.Vertex3(rect.x + rect.width, rect.y, 0);
			}
			else
			{
				if (clipRect.HasValue) DoClip(ref rect, ref clipRect);

				GL.Vertex3(rect.x, rect.y, 0);
				GL.Vertex3(rect.x, rect.y + rect.height, 0);
				GL.Vertex3(rect.x + rect.width, rect.y + rect.height, 0);
				GL.Vertex3(rect.x + rect.width, rect.y, 0);
			}


			GL.End();
			GL.PopMatrix();
		}

		void draw_simple_quad_fast(Rect rect, ref Color c, IconData tex = null, Rect? clipRect = null)
		{
			if (rect.width <= 0 || rect.height <= 0) return;
			GL.Color(c);





			if (tex != null)
			{

				if (clipRect.HasValue)
				{
					//var oX = rect.x;
					//var oY = rect.y;
					//if (rect.x < clipRect.Value.x) rect.x = clipRect.Value.x;
					//if (rect.x > clipRect.Value.x + clipRect.Value.width) rect.x = clipRect.Value.x + clipRect.Value.width;
					//if (rect.y < clipRect.Value.y) rect.y = clipRect.Value.y;
					//if (rect.y > clipRect.Value.y + clipRect.Value.height) rect.y = clipRect.Value.y + clipRect.Value.height;
					//rect.width = Mathf.Max(0, rect.width - (rect.x - oX));
					//rect.height = Mathf.Max(0, rect.height - (rect.y - oY));
					//if (rect.width + rect.x > clipRect.Value.x + clipRect.Value.width) rect.width = Mathf.Max(0, clipRect.Value.x + clipRect.Value.width - rect.x);
					//if (rect.height + rect.y > clipRect.Value.y + clipRect.Value.height) rect.height = Mathf.Max(0, clipRect.Value.y + clipRect.Value.height - rect.y);
					DoClip(ref rect, tex.uv_start, tex.uv_end, ref clipRect);
				}
				else
				{
					tv3_a.Set(tex.uv_start.x, tex.uv_start.y, 0);
					tv3_b.Set(tex.uv_start.x, tex.uv_end.y, 0);
					tv3_c.Set(tex.uv_end.x, tex.uv_end.y, 0);
					tv3_d.Set(tex.uv_end.x, tex.uv_start.y, 0);
				}

				//Debug.Log(c);

				//	tv3.Set(tex.uv_start.x, tex.uv_start.y, 0);
				GL.TexCoord(tv3_a);
				GL.Vertex3(rect.x, rect.y, 0);
				//tv3.Set(tex.uv_start.x, tex.uv_end.y, 0);
				GL.TexCoord(tv3_b);
				GL.Vertex3(rect.x, rect.y + rect.height, 0);
				//tv3.Set(tex.uv_end.x, tex.uv_end.y, 0);
				GL.TexCoord(tv3_c);
				GL.Vertex3(rect.x + rect.width, rect.y + rect.height, 0);
				//tv3.Set(tex.uv_end.x, tex.uv_start.y, 0);
				GL.TexCoord(tv3_d);
				GL.Vertex3(rect.x + rect.width, rect.y, 0);
			}
			else
			{
				if (clipRect.HasValue) DoClip(ref rect, ref clipRect);

				GL.Vertex3(rect.x, rect.y, 0);
				GL.Vertex3(rect.x, rect.y + rect.height, 0);
				GL.Vertex3(rect.x + rect.width, rect.y + rect.height, 0);
				GL.Vertex3(rect.x + rect.width, rect.y, 0);
			}
		}


		void draw_uvved_quad(float start_x, float start_y, float rect_width_x, float rect_height_y, float _v1x, float _v1y, float uv_width, float uv_height, ref Rect? clipRect)
		{

		

			uv_height = -uv_height;
			var end_x = start_x + rect_width_x;
			var end_y = start_y + rect_height_y;
			var _v2x = _v1x + uv_width;
			var _v2y = _v1y + uv_height;
			start_x = Mathf.RoundToInt(start_x);
			start_y = Mathf.RoundToInt(start_y);
			end_x = Mathf.RoundToInt(end_x);
			end_y = Mathf.RoundToInt(end_y);

			if (0 == rect_height_y) return;
			if (0 == rect_width_x) return;

			/*_v1x = Mathf.RoundToInt( _v1x );
			_v1y = Mathf.RoundToInt( _v1y );
			_v2x = Mathf.RoundToInt( _v2x );
			_v2y = Mathf.RoundToInt( _v2y );*/

			/// YOU MAY COMMENT IT, ITS SIMPLY JUST A VISUAL DEBUGING
			if (_v1x < 0 || _v1y < 0 || _v2x < 0 || _v2y < 0 ||
				_v1x > 1 || _v1y > 1 || _v2x > 1 || _v2y > 1 || rect_width_x < 0 || rect_height_y < 0) throw new Exception(
					_v1x + " " + _v1y + " " + _v2x + " " + _v2y + " " +
				_v1x + " " + _v1y + " " + _v2x + " " + _v2y + " " + rect_width_x + " " + rect_height_y);

			if (clipRect.HasValue)
			{
				//if (start_x < clipRect.Value.x) start_x = clipRect.Value.x;
				//if (start_x > clipRect.Value.x + clipRect.Value.width) start_x = clipRect.Value.x + clipRect.Value.width;
				//if (start_y < clipRect.Value.y) start_y = clipRect.Value.y;
				//if (start_y > clipRect.Value.y + clipRect.Value.height) start_y = clipRect.Value.y + clipRect.Value.height;
				//if (end_x < clipRect.Value.x) end_x = clipRect.Value.x;
				//if (end_x > clipRect.Value.x + clipRect.Value.width) end_x = clipRect.Value.x + clipRect.Value.width;
				//if (end_y < clipRect.Value.y) end_y = clipRect.Value.y;
				//if (end_y > clipRect.Value.y + clipRect.Value.height) end_y = clipRect.Value.y + clipRect.Value.height;
				//Debug.Log(start_x + " " + start_y + " " + end_x + " " + end_y + " --- " + clipRect);

				var r = new Rect(start_x, start_y, rect_width_x, rect_height_y);
				DoClip(ref r, new Vector2(_v1x, _v1y), new Vector2(_v2x, _v2y), ref clipRect);
				start_x = r.x;
				start_y = r.y;
				end_x = start_x + r.width;
				end_y = start_y + r.height;

				{
					//	Debug.Log(start_x + " " + start_y + " " + end_x + " " + end_y + " ||| " + clipRect);

				}
			}
			else
			{
				tv3_a.Set(_v1x, _v1y, 0);
				tv3_b.Set(_v1x, _v2y, 0);
				tv3_c.Set(_v2x, _v2y, 0);
				tv3_d.Set(_v2x, _v1y, 0);


				//tv3_a.Set(tex.uv_start.x, tex.uv_start.y, 0);
				//tv3_b.Set(tex.uv_start.x, tex.uv_end.y, 0);
				//tv3_c.Set(tex.uv_end.x, tex.uv_end.y, 0);
				//tv3_d.Set(tex.uv_end.x, tex.uv_start.y, 0);
			}


			GL.TexCoord(tv3_a);
			GL.Vertex3(start_x, start_y, 0);

			GL.TexCoord(tv3_b);
			GL.Vertex3(start_x, end_y, 0);

			GL.TexCoord(tv3_c);
			GL.Vertex3(end_x, end_y, 0);

			GL.TexCoord(tv3_d);
			GL.Vertex3(end_x, start_y, 0);
		}


		Rect? nullClip = null;
		void __DrawTexture(ref Rect rect, IconData tex, ref Color col, float border, Material mat, ref Rect? clipRect, bool fast = false)
		{


			/*	var asd = rect;
                asd.x -= 20;
                asd.width = asd.height;
                defaultMaterial().SetTexture( _MainTex, tex.texture );
                defaultMaterial().SetPass( 0 );
                draw_simple_quad( ref asd, ref col, tex );*/
			if (!fast)
			{
				//Debug.Log(mat.shader);
				mat.SetTexture(_MainTex, tex.texture);
				mat.SetPass(0);
				GL.PushMatrix();
				GL.Begin(GL.QUADS);
			}

			GL.Color(col);



			if (border == 0)
			{
				//Debug.Log(clipRect);
				//defaultMaterial().SetTexture(_MainTex, tex.texture);
				//defaultMaterial().SetPass(0);
				draw_simple_quad(rect, ref col, tex, clipRect);
			}
			else
			{
				////	var max_w = (tex.uv_end.x - tex.uv_start.x) * tex.texture.width / 2;
				var max_w = Mathf.FloorToInt(tex.width / 2);
				var border_x = Math.Min(border, max_w);
				max_w = Mathf.FloorToInt(rect.width / 2);
				border_x = Math.Min(border_x, max_w);

				var max_h = Mathf.FloorToInt(tex.height / 2);
				var border_y = Math.Min(border, max_h);
				max_h = Mathf.FloorToInt(rect.height / 2);
				border_y = Math.Min(border_y, max_h);
				var n_border = Math.Min(border_x / tex.texture.width, border_y / tex.texture.height);


				//border_x = border_y = 0;
				////n_border = 0f / tex.texture.width;

				var rect_border_x = border_x;// n_border * rect.width;
				var rect_border_y = border_y;// n_border * rect.height;
				var uv_width = tex.uv_end.x - tex.uv_start.x;
				var uv_height = tex.uv_start.y - tex.uv_end.y;


				//if ( !d )
				//{
				//	Debug.Log(rect + " " + clipRect);
				//	Debug.Log( border );
				//	Debug.Log( n_border + " " +border_x + " " +tex.width + " " +tex.texture.width );
				//	Debug.Log( tex.uv_start + " " +tex.uv_end );
				//	Debug.Log( tex.uv_start.x + n_border );
				//	Debug.Log( tex.uv_start.y - n_border );
				//	Debug.Log( uv_width - n_border * 2 );
				//	Debug.Log( uv_height - n_border * 2 );
				//
				//}

				draw_uvved_quad(
						rect.x, rect.y, rect_border_x, rect_border_y,
						tex.uv_start.x, tex.uv_start.y, n_border, n_border, ref clipRect);
				draw_uvved_quad(
					rect.x + rect.width - rect_border_x, rect.y, rect_border_x, rect_border_y,
					tex.uv_end.x - n_border, tex.uv_start.y, n_border, n_border, ref clipRect);
				draw_uvved_quad(
					rect.x + rect.width - rect_border_x, rect.y + rect.height - rect_border_y, rect_border_x, rect_border_y,
					tex.uv_end.x - n_border, tex.uv_end.y + n_border, n_border, n_border, ref clipRect);
				draw_uvved_quad(
					rect.x, rect.y + rect.height - rect_border_y, rect_border_x, rect_border_y,
					tex.uv_start.x, tex.uv_end.y + n_border, n_border, n_border, ref clipRect);


				draw_uvved_quad(
					rect.x + rect_border_x, rect.y, rect.width - rect_border_x * 2, rect_border_y,
					tex.uv_start.x + n_border, tex.uv_start.y, uv_width - n_border * 2, n_border, ref clipRect);
				draw_uvved_quad(
					rect.x + rect_border_x, rect.y + rect.height - rect_border_y, rect.width - rect_border_x * 2, rect_border_y,
					tex.uv_start.x + n_border, tex.uv_end.y + n_border, uv_width - n_border * 2, n_border, ref clipRect);
				draw_uvved_quad(
					rect.x, rect.y + rect_border_y, rect_border_x, rect.height - rect_border_y * 2,
					tex.uv_start.x, tex.uv_start.y - n_border, n_border, uv_height - n_border * 2, ref clipRect);
				draw_uvved_quad(
					rect.x + rect.width - rect_border_x, rect.y + rect_border_y, rect_border_x, rect.height - rect_border_y * 2,
					tex.uv_end.x - n_border, tex.uv_start.y - n_border, n_border, uv_height - n_border * 2, ref clipRect);

				draw_uvved_quad(
					rect.x + rect_border_x, rect.y + rect_border_y, rect.width - rect_border_x * 2, rect.height - rect_border_y * 2,
					tex.uv_start.x + n_border, tex.uv_start.y - n_border, uv_width - n_border * 2, uv_height - n_border * 2, ref clipRect);


			}
			if (!fast)
			{
				GL.End();
				GL.PopMatrix();
			}

		}







		class GLTextureStackAndMaterial
		{
			internal GLTextureStackAndMaterial(int sorted, IconData tex, Material mat)
			{
				mat_sorted_index = sorted;
				this.mat = mat;
				this.tex = tex;
			}
			internal int mat_sorted_index;
			internal Material mat;
			internal IconData tex;
			internal int glCount = 0;
			internal GLTextureElement[] glStack = new GLTextureElement[2000];
			internal void _put_stack(ref Rect rect, IconData tex, ref Color col, float border)
			{
				if (glCount >= glStack.Length) Array.Resize(ref glStack, glCount * 2);

				glStack[glCount].rect = rect;
				glStack[glCount].clipRect = null;
				glStack[glCount].tex = tex;
				glStack[glCount].col = col;
				glStack[glCount].border = border;
				glCount++;
			}
			internal void _put_stack(ref Rect rect, IconData tex, ref Color col, float border, ref Rect? clipRect)
			{
				if (glCount >= glStack.Length) Array.Resize(ref glStack, glCount * 2);

				glStack[glCount].rect = rect;
				glStack[glCount].clipRect = clipRect;
				glStack[glCount].tex = tex;
				glStack[glCount].col = col;
				glStack[glCount].border = border;
				glCount++;
			}
		}
		// DRAW SWITHCER
		class GLTextureStack
		{
			internal GLTextureStack(int sorted)
			{
				tex_sorted_index = sorted;
			}
			internal int tex_sorted_index;
			internal Dictionary<int, GLTextureStackAndMaterial> glStack = new Dictionary<int, GLTextureStackAndMaterial>();
			internal int emptyTexture = 0;
			internal void _put_stack(ref Rect rect, IconData tex, ref Color col, float border, Material mat)
			{
				var matID = mat.GetInstanceID();
				if (tex == null) emptyTexture = -1;
				if (!glStack.ContainsKey(matID))
				{
					glStack.Add(matID, new GLTextureStackAndMaterial(glStack.Count, tex, mat));
					if (!glStack[matID].mat) throw new Exception("2");
				}
				if (!glStack[matID].mat)
				{
					glStack[matID] = new GLTextureStackAndMaterial(glStack.Count, tex, mat);
					if (!glStack[matID].mat) throw new Exception("2");
				}

				glStack[matID]._put_stack(ref rect, tex, ref col, border);
#if DEBUG_THIS
                if (glStack[mat.GetInstanceID()].mat != mat) throw new Exception("mat");
                if (glStack[mat.GetInstanceID()].tex?.texture != tex?.texture) throw new Exception("tex " + (tex?.texture) + " + " + glStack[mat.GetInstanceID()].tex?.texture);
#endif
			}
			internal void _put_stack(ref Rect rect, IconData tex, ref Color col, float border, Material mat, ref Rect? clipRect)
			{
				var matID = mat.GetInstanceID();
				if (tex == null) emptyTexture = -1;
				if (!glStack.ContainsKey(matID))
				{
					glStack.Add(matID, new GLTextureStackAndMaterial(glStack.Count, tex, mat));
					if (!glStack[matID].mat) throw new Exception("2");
				}
				if (!glStack[matID].mat)
				{
					glStack[matID] = new GLTextureStackAndMaterial(glStack.Count, tex, mat);
					if (!glStack[matID].mat) throw new Exception("2");
				}

				glStack[matID]._put_stack(ref rect, tex, ref col, border, ref clipRect);
#if DEBUG_THIS
                if (glStack[mat.GetInstanceID()].mat != mat) throw new Exception("mat");
                if (glStack[mat.GetInstanceID()].tex?.texture != tex?.texture) throw new Exception("tex " + (tex?.texture) + " + " + glStack[mat.GetInstanceID()].tex?.texture);
#endif
			}

		}
		internal struct GLTextureElement
		{
			internal IconData tex;
			internal Rect rect;
			internal Rect? clipRect;
			internal Color col;
			internal float border;
		}
		class GLLabelStack
		{
			internal int glCount = 0;
			internal GLLabelElement[] glStack = new GLLabelElement[200];
			internal void _put_stack(Rect rect, GUIContent self_ContentInstance, TextAnchor alignment, RectOffset padding,
			 int fontSize,
			 Font font,
			  Color textColor,
			 TextClipping clipping)
			{
				if (glCount >= glStack.Length) Array.Resize(ref glStack, glCount * 2);
				//   if (glStack.Any(g => g.rect == rect)) throw new Exception(glStack.First(g => g.rect == rect).self_ContentInstance.text + " - " + self_ContentInstance.text);
				/* int count = 0;
                 for (int i = 0; i < glCount; i++)
                 {
                     if (glStack[i].rect == rect) count++;
                 }
                 if (self_ContentInstance.text == "MainCamera") Debug.Log(count + " " + glCount);*/
				glStack[glCount].rect = rect;
				glStack[glCount].self_ContentInstance = self_ContentInstance;
				glStack[glCount].alignment = alignment;
				glStack[glCount].padding = padding;
				glStack[glCount].fontSize = fontSize;
				glStack[glCount].font = font;
				glStack[glCount].textColor = textColor;
				glStack[glCount].clipping = clipping;
				//  glStack[glCount].guiColor = guiColor;
				glCount++;
			}
		}
		internal struct GLLabelElement
		{
			internal Rect rect;
			internal GUIContent self_ContentInstance;
			internal TextAnchor alignment;
			internal RectOffset padding;
			internal int fontSize;
			internal Font font;
			internal Color textColor;
			//internal Color guiColor;
			internal TextClipping clipping;
			/* labelStyle.alignment = style.alignment;
                     labelStyle.fontSize = style.fontSize;
                     labelStyle.font = style.font;
                     labelStyle.normal.textColor = style.normal.textColor;
                     labelStyle.clipping = clip;
                     labelStyle.Draw(rect, self_ContentInstance, false, false, false, false);*/
		}
		class GLButtonStack
		{
			internal int glCount = 0;
			internal GLButtonElement[] glStack = new GLButtonElement[100];
			internal void _put_stack(ref Rect rect, GUIContent self_ContentInstance, GUIStyle style, int controlId, bool contains, Color guiColor, bool guiEnabled, int controlID)
			{
				if (glCount >= glStack.Length) Array.Resize(ref glStack, glCount * 2);
				glStack[glCount].rect = rect;
				glStack[glCount].self_ContentInstance = self_ContentInstance;
				glStack[glCount].controlId = controlId;
				glStack[glCount].contains = contains;
				glStack[glCount].style = style;
				glStack[glCount].guiColor = guiColor;
				glStack[glCount].guiEnabled = guiEnabled;
				glStack[glCount].controlId = controlID;
				glCount++;
			}
		}
		internal struct GLButtonElement
		{
			internal Rect rect;
			internal GUIContent self_ContentInstance;
			internal int controlId;
			internal bool contains;
			internal GUIStyle style;
			internal Color guiColor;
			internal bool guiEnabled;
		}
		class GLStyleStack
		{
			internal int glCount = 0;
			internal GLStyleElement[] glStack = new GLStyleElement[50];
			internal void _put_stack(ref Rect rect, GUIContent content, GUIStyle style, bool hover, TextClipping clip)
			{
				if (glCount >= glStack.Length) Array.Resize(ref glStack, glCount * 2);
				glStack[glCount].rect = rect;
				glStack[glCount].content = content;
				glStack[glCount].style = style;
				glStack[glCount].hover = hover;
				glStack[glCount].clip = clip;
				glCount++;
			}
		}
		internal struct GLStyleElement
		{
			internal Rect rect;
			internal GUIContent content; internal GUIStyle style; internal bool hover; internal TextClipping clip;
		}

		void _put_stack(ref Rect rect, IconData tex, ref Color col, float border, Material mat)
		{
#if DEBUG_THIS
            if (tex != null && tex.id != tex.texture.GetInstanceID()) throw new Exception("id !");
#endif
			var id = tex != null ? tex.id : -1;
			if (!drawCalls[dc].unsorted_glTexture_stacks.ContainsKey(id)) drawCalls[dc].unsorted_glTexture_stacks.Add(id, new GLTextureStack(drawCalls[dc].unsorted_glTexture_stacks.Count));
			// if (!defaultMaterial()) throw new Exception("ASD");
			//  if (!mat && mat != null) throw new Exception("ASD");
			drawCalls[dc].unsorted_glTexture_stacks[id]._put_stack(ref rect, tex, ref col, border, mat ?? defaultMaterial());
		}
		void _put_stack(ref Rect rect, IconData tex, ref Color col, float border, Material mat, ref Rect? clipRect)
		{
#if DEBUG_THIS
            if (tex != null && tex.id != tex.texture.GetInstanceID()) throw new Exception("id !");
#endif
			var id = tex != null ? tex.id : -1;
			if (!drawCalls[dc].unsorted_glTexture_stacks.ContainsKey(id)) drawCalls[dc].unsorted_glTexture_stacks.Add(id, new GLTextureStack(drawCalls[dc].unsorted_glTexture_stacks.Count));
			// if (!defaultMaterial()) throw new Exception("ASD");
			//  if (!mat && mat != null) throw new Exception("ASD");
			drawCalls[dc].unsorted_glTexture_stacks[id]._put_stack(ref rect, tex, ref col, border, mat ?? defaultMaterial(), ref clipRect);
		}


		internal void _DrawTexture(Rect rect, IconData tex, ref Color col, float border)
		{
			if (p.EVENT.type != EventType.Repaint || rect.width <= 0 || rect.height <= 0) return;
			if (!BAKE_DRAWER) __DrawTexture(ref rect, tex, ref col, border, defaultMaterial(), ref nullClip);
			else _put_stack(ref rect, tex, ref col, border, defaultMaterial(), ref nullClip);
		}
		internal void _DrawTexture(Rect rect, IconData tex, ref Color col, float border, Rect? clipRect)
		{
			if (p.EVENT.type != EventType.Repaint || rect.width <= 0 || rect.height <= 0) return;
			if (!BAKE_DRAWER) __DrawTexture(ref rect, tex, ref col, border, defaultMaterial(), ref clipRect);
			else _put_stack(ref rect, tex, ref col, border, defaultMaterial(), ref clipRect);
		}
		//bool d;
		internal void _DrawTexture(Rect rect, IconData tex, ref Color col, float border, Material mat)
		{
			if (p.EVENT.type != EventType.Repaint || rect.width <= 0 || rect.height <= 0) return;
			if (!BAKE_DRAWER) __DrawTexture(ref rect, tex, ref col, border, mat ?? defaultMaterial(), ref nullClip);
			else _put_stack(ref rect, tex, ref col, border, mat ?? defaultMaterial(), ref nullClip);
		}
		internal void _DrawTexture(Rect rect, IconData tex, ref Color col, float border, Material mat, Rect? clipRect)
		{
			if (p.EVENT.type != EventType.Repaint || rect.width <= 0 || rect.height <= 0) return;
			if (!BAKE_DRAWER) __DrawTexture(ref rect, tex, ref col, border, mat ?? defaultMaterial(), ref clipRect);
			else _put_stack(ref rect, tex, ref col, border, mat ?? defaultMaterial(), ref clipRect);
		}

		internal void _DrawTexture(Rect rect, ref Color col, Event ev = null) //  EditorGUI.DrawRect( rect, col );
		{
			if (ev == null && p.EVENT.type != EventType.Repaint) return;
			if (ev != null && ev.type != EventType.Repaint) return;
			if (!BAKE_DRAWER)
			{
				defaultMaterial().SetTexture(_MainTex, null);
				defaultMaterial().SetPass(0);
				draw_simple_quad(rect, ref col);
			}
			else
			{
				_put_stack(ref rect, null, ref col, 0, null);
			}

		}
		internal void _DrawTexture_ForExternalWindow(Rect rect, IconData tex, ref Color col)
		{
			p.EVENT = Event.current;
			_DrawTexture(rect, tex, ref col);
		}
		internal void _DrawTexture_ForExternalWindow(Rect rect, IconData tex, ref Color col, float border, Material mat)
		{
			p.EVENT = Event.current;
			_DrawTexture(rect, tex, ref col, border, mat);
		}
		internal void _DrawTexture(Rect rect, IconData tex, ref Color col)
		{
			if (p.EVENT.type != EventType.Repaint) return;
			if (!BAKE_DRAWER)
			{
				defaultMaterial().SetTexture(_MainTex, tex.texture);
				defaultMaterial().SetPass(0);
				draw_simple_quad(rect, ref col, tex);
			}
			else
			{
				_put_stack(ref rect, tex, ref col, 0, null);
			}
		}


		GUIStyle labelStyle = new GUIStyle();
		internal void _DrawLabel(Rect rect, GUIContent self_ContentInstance, GUIStyle style, TextClipping clip = TextClipping.Clip)
		{
			if (p.EVENT.type != EventType.Repaint) return;
			if (!BAKE_DRAWER)
			{
				//if (self_ContentInstance.text == null)
				{
					Root.SetMouseTooltip(self_ContentInstance, rect);
				}
				if (self_ContentInstance.text != null && self_ContentInstance.text != "")
				{

					labelStyle.alignment = style.alignment;
					labelStyle.fontSize = style.fontSize;
					labelStyle.font = style.font;
					labelStyle.normal.textColor = style.normal.textColor;
					labelStyle.clipping = clip;
					labelStyle.padding = style.padding;
					labelStyle.Draw(rect, self_ContentInstance, false, false, false, false);
				}
			}
			else
			{
				drawCalls[dc].label_stack._put_stack(rect, self_ContentInstance, style.alignment, style.padding
				, style.fontSize
				, style.font
				, style.normal.textColor * GUI.color
				, clip);
			}
			//GUI tooltip if not null
		}
		//  Rect? wasRest;
		void _DrawLabel(ref GLLabelElement e)
		{
			Root.SetMouseTooltip(e.self_ContentInstance, e.rect);
			if (e.self_ContentInstance.text != null && e.self_ContentInstance.text != "")
			{
				labelStyle.alignment = e.alignment;
				labelStyle.padding = e.padding;
				labelStyle.fontSize = e.fontSize;
				labelStyle.font = e.font;
				labelStyle.normal.textColor = e.textColor;
				labelStyle.clipping = e.clipping;
				// var c= GUI.color;
				// GUI.color=  e.guiColor;
				labelStyle.Draw(e.rect, e.self_ContentInstance, false, false, false, false);
				// if (wasRest == e.rect) throw new Exception("ASD");
				// wasRest = e.rect;
				// GUI.color = c;
			}
		}
		Color tempColor;
		Color alpha = new Color(1, 1, 1, 0.5f);
		//int capturedControlID = -1;


		internal bool _DrawButton(Rect rect, GUIContent self_ContentInstance, GUIStyle style)
		{
			//if (Event.current.type == EventType.Repaint)

			bool res = false;
			if (p.par_e.ONDOWN_ACTION_INSTEAD_ONUP && p.EVENT.type == EventType.MouseDown && rect.Contains(p.EVENT.mousePosition))
			{
				res = true;
			}

			/* var texture = Icons.GetIconDataFromTexture(style.normal.background ?? style.normal.scaledBackgrounds[0]);
           _DrawTexture(rect, texture, ref white, style.border.left);
            if (self_ContentInstance != null && self_ContentInstance.text != null && self_ContentInstance.text != "")
                _DrawLabel(rect, self_ContentInstance, style, style.clipping);
            else if (self_ContentInstance != null && self_ContentInstance.tooltip != null && self_ContentInstance.tooltip != "")
                Root.SetMouseTooltip(self_ContentInstance, rect);*/




			/* if (p.EVENT.type == EventType.MouseDown && !p.par_e.ONDOWN_ACTION_INSTEAD_ONUP && rect.Contains(p.EVENT.mousePosition))
             {
                 capturedControlID = controlId;
                 p.EVENT.Use();
             }
             if (p.EVENT.type == EventType.MouseUp && !p.par_e.ONDOWN_ACTION_INSTEAD_ONUP && rect.Contains(p.EVENT.mousePosition) && capturedControlID == controlId)
             {
                 res = true;
             }*/
			//  if (
#if ONLY_REPAINT
#endif
			// p.EVENT.type != EventType.Repaint ||
			// !BAKE_DRAWER)
			//   if (Event.current.isMouse) Debug.Log(EditorGUIUtility.GetControlID(FocusType.Passive) + " " + EditorGUIUtility.hotControl);
			//  var h = EditorGUIUtility.hotControl;
			{
				if (!p.par_e.ONDOWN_ACTION_INSTEAD_ONUP) res = GUI.Button(rect, self_ContentInstance, style);
				else GUI.Button(rect, self_ContentInstance, style);
			}
			/*  if (EditorGUIUtility.hotControl != h)
                 Debug.Log(EditorGUIUtility.hotControl + " " + Event.
                     current.type +  "  " + Event.
                     current.button);*/
			//  if (res) Event.current.Use();

			if (!BAKE_DRAWER)
			{
				/*  if (p.EVENT.type == EventType.Repaint)
                  {
                      int controlId = GUIUtility.GetControlID(FocusType.Passive, rect);
                      if (!GUI.enabled)
                      {
                          tempColor = GUI.color;
                          GUI.color *= alpha;
                      }
                      // GUI.GrabMouseControl(id);
                      style.Draw(rect, self_ContentInstance, controlId, false, rect.Contains(p.EVENT.mousePosition));
                      if (capturedControlID == controlId) Debug.Log("ASD");
                      // style.Draw(rect, self_ContentInstance, rect.Contains(p.EVENT.mousePosition), false, capturedControlID == controlId, false);
                      if (!GUI.enabled) GUI.color = tempColor;
                  }*/
			}
			else
			{
				drawCalls[dc].button_stack._put_stack(ref rect, self_ContentInstance, style, 0, false, GUI.color, GUI.enabled, 0); //EditorGUIUtility.hotControl != h ? 1 : 0
			}


			return res;
		}
		void _DrawButton(ref GLButtonElement el)
		{
			var c = GUI.color;
			GUI.color = el.guiColor;
			var e = GUI.enabled;
			GUI.enabled = el.guiEnabled;
			// if (el.controlId != 0) EditorGUI.DrawRect(el.rect, hoverColor);
			//EditorGUIUtility.hotControl = EditorGUIUtility.GetControlID(FocusType.Passive);
			// GUI.Button(el.rect, el.self_ContentInstance, el.style);
			GUI.color = c;
			GUI.enabled = e;

			//  el.style.clipping = el.clip;
			// el.style.Draw(el.rect, el.content, el.hover, false, false, false);
		}
		internal void _DrawStyleWithText(Rect rect, GUIStyle style, GUIContent content, TextClipping clip, bool enabled)
		{

			if (p.EVENT.type != EventType.Repaint) return;
			if (!BAKE_DRAWER)
			{
				// var texture = Icons.GetIconDataFromTexture(style.normal.background ?? style.normal.scaledBackgrounds[0]);
				//  _DrawTexture(rect, texture, ref white, style.border.left);
				//  _DrawLabel(rect, content, style, clip);
				style.clipping = clip;
				// int controlId = enabled ? GUIUtility.GetControlID(FocusType.Passive, rect) : 0;
				//  style.Draw(rect, content, controlId, false, enabled && rect.Contains(Event.current.mousePosition));
				style.Draw(rect, content, enabled && rect.Contains(p.EVENT.mousePosition), false, false, false);
			}
			else
			{
				drawCalls[dc].style_stack._put_stack(ref rect, content, style, enabled && rect.Contains(p.EVENT.mousePosition), clip);
			}

		}
		void _DrawStyleWithText(ref GLStyleElement el)
		{
			el.style.clipping = el.clip;
			el.style.Draw(el.rect, el.content, el.hover, false, false, false);
		}



		///////////////////////////
		//TAP GLOW INPORT
		////  static Texture2D hoverTexture;
		static Color hoverColor = new Color32(50, 225, 255, 255 / 6);
		internal void DRAW_TAP_GLOW(Rect rect, float alpha)
		{
			Color c = hoverColor;
			c.a = alpha;
			_DrawTexture(rect, ref c);
			//    EditorGUI.DrawRect(rect, c);
		}
		internal void DRAW_TAP_GLOW(Rect rect)
		{
			//// if ( !hoverTexture ) hoverTexture = Root.p[ 0 ].GetIcon( "TAP_GLOW" );
			/// GUI.DrawTexture( rect, hoverTexture );
			_DrawTexture(rect, ref hoverColor, Event.current);
			//  EditorGUI.DrawRect(rect, hoverColor);
		}
		internal void DRAW_TAP_GLOW(Rect rect, Color color)
		{
			//// if ( !hoverTexture ) hoverTexture = Root.p[ 0 ].GetIcon( "TAP_GLOW" );
			/// GUI.DrawTexture( rect, hoverTexture );
			_DrawTexture(rect, ref color, Event.current);
			// EditorGUI.DrawRect(rect, color);
		}
		//TAP GLOW INPORT
		///////////////////////////

		// DRAW SWITHCER
		/* internal void _DrawTexture(Rect rect, ref Color col, float A)
        {
          if (p.EVENT.type != EventType.Repaint) return;
          col.a *= A;
          defaultMaterial().SetTexture(_MainTex, null);
          defaultMaterial().SetPass(0);
          draw_simple_quad(ref rect, ref col);
        }*/


		internal void _DrawTexture(Rect rect, IconData tex)
		{
			if (p.EVENT.type != EventType.Repaint) return;
			var c = Color.white;
			_DrawTexture(rect, tex, ref c);
		}
		internal void _DrawRect(ref Rect rect, ref Color color)
		{
			_DrawTexture(rect, ref color);
		}
		internal void _DrawRect(ref Rect rect, Color color)
		{
			_DrawTexture(rect, ref color);
		}

		internal void GL_BEGIN()
		{
			GL.PushMatrix();
			// Set black as background color
			//GL.LoadPixelMatrix();
			GL.Clear(true, false, Color.black);
			// var mat = adapter.DEFAULT_SHADER_SHADER.HIghlighterExternalMaterial;
			//mat = adapter.HIghlighterExternalMaterialNormal;

			defaultMaterial().SetTexture(_MainTex, null);
			defaultMaterial().SetPass(0);
			//	GL.Begin(GL.LINES);
			GL.Begin(GL.LINES);
			//  mat.SetTexture(adapter._MainTex, Texture2D.whiteTexture);
		}

		internal void GL_END()
		{
			GL.End();

			GL.PopMatrix();
		}


		/*	internal void _DrawTexture_PlusGUIColor( Rect rect, IconData tex ) 
		{
			if ( p.EVENT.type != EventType.Repaint ) return;
			var c = GUI.color;
			_DrawTexture( rect, tex, ref c );
		}*/
		/*internal void _DrawTexture_StretchToFill( Rect rect, Texture tex ) 
		{ 
			if ( p.EVENT.type != EventType.Repaint ) return;
			GUI.DrawTexture( rect, tex, ScaleMode.StretchToFill, true, 1, GUI.color, 0, 0 );
		}*/
		/*internal void _DrawRect_PlusGUIColor( ref Rect rect, Color c )
		{
			c *= GUI.color;
			_DrawTexture( rect, ref c );
		}*/
	}
}
