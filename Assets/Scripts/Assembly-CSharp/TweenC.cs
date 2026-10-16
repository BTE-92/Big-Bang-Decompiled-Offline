using UnityEngine;

public class TweenC : BasicComponent
{
	public TransformC p_TC;

	public TweenedProperty tweenedProperty;

	public bool mirrored;

	public int currentRepeat;

	public int repeats;

	public TweenStyle currentTweenStyle;

	public TweenStyle mirroredTweenStyle;

	public Vector3 startValue;

	public Vector3 endValue;

	public Vector3 currentValue;

	public Vector3 control0;

	public Vector3 control1;

	public float delay;

	public float duration;

	public float startTime;

	public float currentTime;

	public bool useUnscaledDeltaTime;

	public TweenEventDelegate tweenEndEvent;

	public TweenEventDelegate tweenStartEvent;

	public bool removeEntityAtFinish;

	public bool removeComponentAtFinish;

	public bool hasFinished;

	public bool hasStarted;

	public bool curved;

	public bool alphaSprite;

	public bool alphaPrefabs;

	public bool alphaText;

	public Shader alphaShader;

	public object customObject;

	public TweenC()
		: base(ComponentType.Tween)
	{
	}

	public override void Reset()
	{
		base.Reset();
		p_TC = null;
		duration = 0f;
		currentRepeat = 0;
		repeats = 0;
		delay = 0f;
		mirrored = false;
		removeEntityAtFinish = false;
		removeComponentAtFinish = false;
		hasFinished = false;
		curved = false;
		alphaSprite = true;
		alphaPrefabs = false;
		alphaText = false;
		alphaShader = null;
		customObject = null;
		tweenEndEvent = null;
		tweenStartEvent = null;
		hasStarted = false;
	}
}
