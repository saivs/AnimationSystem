using UnityEngine;

namespace Saivs.Animation
{
    public struct BoneTransform
    {
        public bool IsValid;
        public Vector3 Position;
        public Quaternion Rotation;
        public Vector3 Scale;

        public static BoneTransform Null => new BoneTransform()
        {
            IsValid = false,
            Position = Vector3.zero,
            Rotation = Quaternion.identity,
            Scale = Vector3.one,
        };

        public static BoneTransform Lerp(BoneTransform a, BoneTransform b, float t)
        {
            if (!a.IsValid || !b.IsValid)
                return a;

            BoneTransform result = a;

            result.Position = Vector3.Lerp(a.Position, b.Position, t);
            result.Rotation = Quaternion.Lerp(a.Rotation, b.Rotation, t);
            result.Scale = Vector3.Lerp(a.Scale, b.Scale, t);

            return result;
        }
    }
}