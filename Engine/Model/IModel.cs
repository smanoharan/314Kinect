using System;
using System.Collections.Generic;
using Engine.FeatureExtraction;

namespace Engine.Model
{
    /// <summary>
    /// The model for a single sign
    /// </summary>
    public interface IModel
    {
        /// <summary>
        /// Record a sequence, then evaluate how well it satisfies the model.
        /// 
        /// Skeleton and Hand scores are in the interval [0,100)
        /// </summary>
        /// <param name="sequenceFinished">The function to call with the scores</param>
        /// <param name="lowEntropy">The function to call when (partial or temporary) silence is detected</param>
        /// <param name="highEntropy">The function to call when the user is moving</param>
        void EvaluateSequence(Action<int, int> sequenceFinished, Action<int> lowEntropy, Action highEntropy);
        
        /// <summary>
        /// Abandon the recording, throwing away the partially recorded sequence.
        /// </summary>
        void CancelRecording();

        /// <summary>
        /// Train a model using the training sequence
        /// </summary>
        /// <param name="seqList">A set of training sequences</param>
        void TrainWith(IList<Sequence> seqList);
    }
}
