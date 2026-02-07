using UnityEngine;

public class AnimeGirlController : MonoBehaviour
{
    [SerializeField] private ParticleSystem jumpEffect;
    [SerializeField] private ParticleSystem landingEffect;

    public void PlayJumpEffect()
    {
        jumpEffect.Play();
    }
 
    public void PlayLandingEffect()
    {
        landingEffect.Play();
    }
}
