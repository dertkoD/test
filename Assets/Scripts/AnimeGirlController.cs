using UnityEngine;

public class AnimeGirlController : MonoBehaviour
{
    private static readonly System.Collections.Generic.Dictionary<Animator, AnimeGirlController> Registry = new();

    [SerializeField] private Animator animator;
    [SerializeField] private ParticleSystem jumpEffect;
    [SerializeField] private ParticleSystem landingEffect;

    public static bool TryGetByAnimator(Animator animator, out AnimeGirlController controller) =>
        Registry.TryGetValue(animator, out controller);

    private void OnEnable()
    {
        if (!animator) return;
        Registry[animator] = this;
    }

    private void OnDisable()
    {
        if (animator && Registry.TryGetValue(animator, out var existing) && existing == this)
            Registry.Remove(animator);
    }

    public void PlayJumpEffect()
    {
        jumpEffect.Play();
    }
 
    public void PlayLandingEffect()
    {
        landingEffect.Play();
    }
}
