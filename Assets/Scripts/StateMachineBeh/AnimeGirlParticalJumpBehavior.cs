using UnityEngine;
using UnityEngine.Animations;

public class AnimeGirlParticalJumpBehavior : StateMachineBehaviour
{
    private AnimeGirlController _girlController;


    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex,
        AnimatorControllerPlayable controller)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex, controller);
        if (!_girlController)
            AnimeGirlController.TryGetByAnimator(animator, out _girlController);

        if (_girlController)
            _girlController.PlayJumpEffect();
    }
    
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex,
        AnimatorControllerPlayable controller)
    {
        base.OnStateExit(animator, stateInfo, layerIndex, controller);
        if (!_girlController)
            AnimeGirlController.TryGetByAnimator(animator, out _girlController);

        if (_girlController)
            _girlController.PlayLandingEffect();
    }

    
}
