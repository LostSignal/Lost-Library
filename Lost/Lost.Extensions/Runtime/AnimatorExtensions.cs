//-----------------------------------------------------------------------
// <copyright file="AnimatorExtensions.cs" company="Lost Signal LLC">
//     Copyright (c) Lost Signal LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Lost
{
    using UnityEngine;
    using UnityEngine.Animations;
    using UnityEngine.Playables;

    public static class AnimatorExtensions
    {
        public static PlayableGraph EnableAndPlayClip(this Animator animator, AnimationClip animationClip)
        {
            if (animator.enabled == false)
            {
                animator.enabled = true;
            }

            PlayableGraph playableGraph = PlayableGraph.Create();
            playableGraph.SetTimeUpdateMode(DirectorUpdateMode.GameTime);

            AnimationPlayableOutput playableOutput = AnimationPlayableOutput.Create(playableGraph, "Animation", animator);
            AnimationClipPlayable clipPlayable = AnimationClipPlayable.Create(playableGraph, animationClip);
            playableOutput.SetSourcePlayable(clipPlayable);

            playableGraph.Play();

            return playableGraph;
        }
    }
}
