using System;
using System.Collections.Generic;
using Engine.FeatureExtraction;
using Engine.Kinect;
using Engine.Model;

namespace Engine
{
    /// <summary>
    /// Default (Kinect-dependent) implementation of the IModel.
    /// Requires Kinect to be available.
    /// </summary>
    [Serializable()]
    public class EngineModel : IModel
    {
        public HMM SkelHMM { get; private set; }
        public HMM HandHMM { get; private set; }

        public EngineModel(HMM skelHMM, HMM handHMM)
        {
            SkelHMM = skelHMM;
            HandHMM = handHMM;
        }

        /// <summary>
        /// Record a sequence, then evaluate how well it satisfies the model.
        /// 
        /// Skeleton and Hand scores are in the interval [0,100)
        /// </summary>
        /// <param name="sequenceFinished">The function to call with the scores</param>
        /// <param name="lowEntropy">The function to call when (partial or temporary) silence is detected</param>
        /// <param name="highEntropy">The function to call when the user is moving</param>
        public void EvaluateSequence(Action<int, int> sequenceFinished, Action<int> lowEntropy, Action highEntropy)
        {
            SequenceRecorder.Get().RecordSequence(
                seq => sequenceFinished(EvaluteSkeleton(seq), EvaluateHand(seq)), 
                lowEntropy, highEntropy);
        }

        private int EvaluateHand(Sequence seq)
        {
            return HandHMM.Evaluate(seq, s => s.HandFeatures);
        }

        private int EvaluteSkeleton(Sequence seq)
        {
            return SkelHMM.Evaluate(seq, s => s.SkeletonFeatures);
        }

        /// <summary>
        /// Abandon the current recording and throw away the partial sequence.
        /// </summary>
        public void CancelRecording()
        {
            SequenceRecorder.Get().CancelRecording();
        }

        /// <summary>
        /// Train a model from the examples.
        /// </summary>
        /// <param name="sequences">The training examples</param>
        /// <returns></returns>
        public static EngineModel CreateFromTrainingSequences(IList<Sequence> sequences)
        {
            return new EngineModel(
                HMM.CreateFromTrainingExamples(sequences, s=>s.SkeletonFeatures),
                HMM.CreateFromTrainingExamples(sequences, s=>s.HandFeatures));
        }

        /// <summary>
        /// Record a sequence of skeleton and hand features.
        /// </summary>
        /// <param name="sequenceFinished">The function to call with the sequence</param>
        /// <param name="lowEntropy">The function to call when (partial or temporary) silence is detected</param>
        /// <param name="highEntropy">The function to call when the user is moving</param>
        public static void RecordSequence(Action<Sequence> sequenceFinished, Action<int> lowEntropy, Action highEntropy)
        {
            SequenceRecorder.Get().RecordSequence(sequenceFinished, lowEntropy, highEntropy);
        }

        /// <summary>
        /// Record a sequence of skeleton and hand features, 
        ///   along with a video of the user performing the sign.
        /// </summary>
        /// <param name="sequenceFinished">The function to call with the sequence & the video</param>
        /// <param name="lowEntropy">The function to call when (partial or temporary) silence is detected</param>
        /// <param name="highEntropy">The function to call when the user is moving</param>
        public static void RecordSequenceWithVideo(Action<Sequence, byte[][]> sequenceFinished, Action<int> lowEntropy, Action highEntropy)
        {
            SequenceRecorder.Get().RecordSequence(sequenceFinished, lowEntropy, highEntropy);
        }

        /// <summary>
        /// Train a model using the training sequence
        /// </summary>
        /// <param name="seqList">A set of training sequences</param>
        public void TrainWith(IList<Sequence> seqList)
        {
            SkelHMM.Train(seqList, seq => seq.SkeletonFeatures);
            HandHMM.Train(seqList, seq => seq.HandFeatures);
        }
    }
}
