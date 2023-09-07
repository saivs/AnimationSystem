using System;

namespace Saivs.Animation
{
    [Serializable]
    public struct BoneBinding
    {
        public static BoneBinding Null => new BoneBinding()
        {
            Id = new StringHash(),
            PositionIndex = -1,
            RotationIndex = -1,
            ScaleIndex = -1,
        };

        public StringHash Id;
        public int PositionIndex;
        public int RotationIndex;
        public int ScaleIndex;
    }

    [Serializable]
    public struct CurveBinding
    {
        public StringHash Id;
        public int Index;
    }
}