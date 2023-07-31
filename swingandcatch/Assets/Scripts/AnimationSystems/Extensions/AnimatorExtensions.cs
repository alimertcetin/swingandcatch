using UnityEngine;

namespace TheGame.AnimationSystems.Extensions
{
    public static class AnimatorExtensions
    {
        public static float GetClipDuration(this Animator animator, string clipName)
        {
            var clips = animator.runtimeAnimatorController.animationClips;
            int length = clips.Length;
            for (var i = 0; i < length; i++)
            {
                var clip = clips[i];
                if (clip.name == clipName)
                {
                    return clip.length;
                }
            }

            return -1f;
        }
    }
}