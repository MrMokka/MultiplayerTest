#define RUNTIME_ALSO

#if RUNTIME_ALSO || UNITY_EDITOR
using System;
using UnityEngine;

/// <summary>
/// All attributes are automatically included in the build like all other attributes, like a [ContextMenu("")] menu, 
/// If you do not want to include the [DRAW_IN_HIER] attribute in the final build, 
/// You may just to comment RUNTIME_ALSO line, but if you will add DRAW_IN_HIER you need to write the condition #if UNITY_EDITOR for each your [SHOW_IN_HIER] attributes
/// You may also rename the name or attribute class [DRAW_IN_HIER] using ctrl+r+r if it visual studiom, for example [afgsdf], because obfuscator will skip any editor attributes like [ContextMenu("")]
/// So keep in mind, it is better if you also add #if UNITY_EDITOR for all attrbutes like [ContextMenu("")] 
/// </summary>

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = true, Inherited = false)]
public class DRAW_IN_HIER : Attribute
{
	public float? width = null;
	public Color? color = null;
	
	public DRAW_IN_HIER()
	{
	}

	public DRAW_IN_HIER(float width)
	{
		this.width = width;
	}

	public DRAW_IN_HIER(float[] color)
	{
		if (color.Length != 4) return;
		this.color = new Color(color[0], color[1], color[2], color[3]);
	}

	public DRAW_IN_HIER(float width, float[] color)
	{
		this.width = width;
		if (color.Length != 4) return;
		this.color = new Color(color[0], color[1], color[2], color[3]);
	}
}
#endif
