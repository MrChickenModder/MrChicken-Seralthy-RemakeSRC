using GorillaExtensions;
using Seralyth.Menu;
using TMPro;
using UnityEngine;
using UnityEngine.Video;

namespace Seralyth.Classes.Mods;

public class VirtualStumpAd : MonoBehaviour
{
	private bool hasSetupFeaturedMapVideo;

	private VideoPlayer videoPlayer;

	public static GameObject LoadingText;

	public static GameObject MapInfoText;

	public static GameObject FeaturedMaps;

	public static GameObject DisplayTextObj;

	private Vector3 oldLocalScale = Vector3.zero;

	private string oldText = "";

	private SpriteRendererData cachedSpriteRendererData;

	public static VirtualStumpAd Instance { get; private set; }

	public static SpriteRenderer SpriteRenderer { get; private set; }

	private void Awake()
	{
		Instance = this;
	}

	private void OnDisable()
	{
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Expected O, but got Unknown
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		hasSetupFeaturedMapVideo = false;
		TextMeshPro component = MapInfoText.GetComponent<TextMeshPro>();
		((TMP_Text)component).text = oldText;
		MapInfoText.SetActive(false);
		LoadingText.SetActive(true);
		foreach (Transform item in DisplayTextObj.transform)
		{
			Transform val = item;
			if (((Object)val).name.ToLower().EndsWith("tmp"))
			{
				((Component)val).gameObject.SetActive(!((Component)val).gameObject.activeSelf);
			}
		}
		Transform obj = FeaturedMaps.transform.Find("FeaturedMapImage");
		GameObject val2 = ((obj != null) ? ((Component)obj).gameObject : null);
		if (!((Object)(object)val2 == (Object)null))
		{
			Object.Destroy((Object)(object)GTExt.GetOrAddComponent<MeshFilter>(val2));
			Object.Destroy((Object)(object)GTExt.GetOrAddComponent<MeshRenderer>(val2));
			val2.transform.localScale = oldLocalScale;
			Object.Destroy((Object)(object)GTExt.GetOrAddComponent<VideoPlayer>(val2));
			ApplySpriteRenderer(val2);
		}
	}

	private void Update()
	{
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Expected O, but got Unknown
		//IL_0212: Unknown result type (might be due to invalid IL or missing references)
		//IL_0219: Expected O, but got Unknown
		//IL_0259: Unknown result type (might be due to invalid IL or missing references)
		//IL_0260: Expected O, but got Unknown
		//IL_027e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0283: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_029b: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a0: Unknown result type (might be due to invalid IL or missing references)
		if (hasSetupFeaturedMapVideo && !videoPlayer.isPlaying && ((Behaviour)videoPlayer).enabled)
		{
			if (!videoPlayer.isLooping)
			{
				videoPlayer.isLooping = true;
			}
			videoPlayer.Play();
		}
		if (hasSetupFeaturedMapVideo)
		{
			return;
		}
		LoadingText = Main.GetObject("Environment Objects/LocalObjects_Prefab/TreeRoom/LoadingText");
		MapInfoText = Main.GetObject("Environment Objects/LocalObjects_Prefab/TreeRoom/MapInfo_TMP");
		FeaturedMaps = Main.GetObject("Environment Objects/LocalObjects_Prefab/TreeRoom/ModIOFeaturedMapsDisplay");
		DisplayTextObj = Main.GetObject("Environment Objects/LocalObjects_Prefab/TreeRoom/ModIOFeaturedMapsDisplay/DisplayText");
		if ((Object)(object)DisplayTextObj != (Object)null)
		{
			foreach (Transform item in DisplayTextObj.transform)
			{
				Transform val = item;
				if (((Object)val).name.ToLower().EndsWith("tmp"))
				{
					((Component)val).gameObject.SetActive(!((Component)val).gameObject.activeSelf);
				}
			}
		}
		if ((Object)(object)MapInfoText == (Object)null || (Object)(object)FeaturedMaps == (Object)null)
		{
			return;
		}
		try
		{
			TextMeshPro component = MapInfoText.GetComponent<TextMeshPro>();
			if ((Object)(object)component != (Object)null)
			{
				oldText = ((TMP_Text)component).text;
				((TMP_Text)component).text = "<b><color=#7C00FA>MrChicken Menu</color></b>";
				MapInfoText.SetActive(true);
			}
			GameObject loadingText = LoadingText;
			if (loadingText != null)
			{
				loadingText.SetActive(false);
			}
			Transform obj = FeaturedMaps.transform.Find("FeaturedMapImage");
			GameObject val2 = ((obj != null) ? ((Component)obj).gameObject : null);
			if (!((Object)(object)val2 == (Object)null))
			{
				CacheAndRemoveSpriteRenderer(val2);
				MeshFilter orAddComponent = GTExt.GetOrAddComponent<MeshFilter>(val2);
				orAddComponent.mesh = Resources.GetBuiltinResource<Mesh>("Quad.fbx");
				MeshRenderer orAddComponent2 = GTExt.GetOrAddComponent<MeshRenderer>(val2);
				Material material = new Material(Shader.Find("Unlit/Texture"));
				((Renderer)orAddComponent2).material = material;
				videoPlayer = GTExt.GetOrAddComponent<VideoPlayer>(val2);
				videoPlayer.audioOutputMode = (VideoAudioOutputMode)0;
				videoPlayer.url = "https://raw.githubusercontent.com/Seralyth/Seralyth-Menu/master/Resources/Server/Videos/vstump-video.mp4";
				RenderTexture val3 = new RenderTexture(512, 512, 0);
				videoPlayer.targetTexture = val3;
				((Renderer)orAddComponent2).material.mainTexture = (Texture)(object)val3;
				if (oldLocalScale == Vector3.zero)
				{
					oldLocalScale = val2.transform.localScale;
				}
				val2.transform.localScale = new Vector3(0.845f, 0.445f, 1f);
				videoPlayer.isLooping = true;
				videoPlayer.Play();
				val2.SetActive(true);
				hasSetupFeaturedMapVideo = true;
			}
		}
		catch
		{
		}
	}

	private void CacheAndRemoveSpriteRenderer(GameObject target)
	{
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		SpriteRenderer = target.GetComponent<SpriteRenderer>();
		if (!((Object)(object)SpriteRenderer == (Object)null))
		{
			cachedSpriteRendererData = new SpriteRendererData
			{
				Sprite = SpriteRenderer.sprite,
				Material = ((Renderer)SpriteRenderer).material,
				Color = SpriteRenderer.color,
				SortingLayerID = ((Renderer)SpriteRenderer).sortingLayerID,
				SortingOrder = ((Renderer)SpriteRenderer).sortingOrder,
				FlipX = SpriteRenderer.flipX,
				FlipY = SpriteRenderer.flipY,
				DrawMode = SpriteRenderer.drawMode,
				Size = SpriteRenderer.size
			};
			Object.Destroy((Object)(object)SpriteRenderer);
		}
	}

	private void ApplySpriteRenderer(GameObject target)
	{
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		if (cachedSpriteRendererData != null)
		{
			SpriteRenderer = target.AddComponent<SpriteRenderer>();
			SpriteRenderer.sprite = cachedSpriteRendererData.Sprite;
			((Renderer)SpriteRenderer).material = cachedSpriteRendererData.Material;
			SpriteRenderer.color = cachedSpriteRendererData.Color;
			((Renderer)SpriteRenderer).sortingLayerID = cachedSpriteRendererData.SortingLayerID;
			((Renderer)SpriteRenderer).sortingOrder = cachedSpriteRendererData.SortingOrder;
			SpriteRenderer.flipX = cachedSpriteRendererData.FlipX;
			SpriteRenderer.flipY = cachedSpriteRendererData.FlipY;
			SpriteRenderer.drawMode = cachedSpriteRendererData.DrawMode;
			SpriteRenderer.size = cachedSpriteRendererData.Size;
		}
	}
}
