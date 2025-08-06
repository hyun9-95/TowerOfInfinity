using Cysharp.Threading.Tasks;
using UnityEngine;

public class CharacterAnimationEffect : AddressableMono
{
    public enum EffectType
	{
		Run,
		Dash,
		Brake,
		Jump,
		Fall,
		FireMuzzleS,
		FireMuzzleM,
	}
	#region Property
	#endregion

	#region Value
	[SerializeField]
	private Animator animator;

	[SerializeField]
	private SpriteRenderer spriteRenderer;
    #endregion

    #region Function
    private void Awake()
    {
		spriteRenderer.enabled = false;
    }

    public void Play(EffectType type, Vector3 pos, bool isFlipX)
	{
		PlayAsync(type, pos, isFlipX).Forget();
	}

	private async UniTask PlayAsync(EffectType type, Vector3 pos, bool isFlipX)
    {
		transform.position = pos;
		spriteRenderer.enabled = true;
        spriteRenderer.flipX = isFlipX;
        
        animator.Play(type.ToString(), 0, 0f);

		await UniTaskUtils.DelaySeconds(0.25f);

        spriteRenderer.enabled = false;
    }
    #endregion
}
