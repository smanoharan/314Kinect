using System;
using System.Collections.Generic;
using Engine.FeatureExtraction;

namespace Engine.Model
{
    /// <summary>
    /// A model for testing without the Kinect. 
    /// Should not be used in production code.
    /// </summary>
    public class MockModel : IModel
    {
        public void EvaluateSequence(Action<int, int> sequenceFinished, Action<int> lowEntropy, Action highEntropy)
        {
            sequenceFinished(70 /*new Random().Next(100)*/, new Random().Next(100));
        }

        public void CancelRecording(){}

        public static void RecordSequence(Action<Sequence> sequenceFinished, Action<int> lowEntropy, Action highEntropy)
        {
            sequenceFinished(new Sequence(new double[][] { }, new double[][] { }));
        }

        public static void RecordSequenceWithVideo(Action<Sequence, byte[][]> sequenceFinished, Action<int> lowEntropy, Action highEntropy)
        {
            sequenceFinished(new Sequence(new double[][] { }, new double[][] { }), new byte[][] { });
        }

        public void TrainWith(IList<Sequence> seqList)
        {
            // do nothing.
        }
    }
}
