using UnityEngine;

namespace Seralyth.Classes.Mods;

public sealed class SpriteRendererData
{
	public Sprite Sprite;

	public Material Material;

	public Color Color;

	public int SortingLayerID;

	public int SortingOrder;

	public bool FlipX;

	public bool FlipY;

	public SpriteDrawMode DrawMode;

	public Vector2 Size;
}
