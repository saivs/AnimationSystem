using System;
using Unity.Mathematics;
using UnityEngine.Assertions;

namespace Saivs.Extensions
{
    public static class MathematicsExtensions
    {
        public static int FloorToInt(this float v) => (int)math.floor(v);

        public static int2 FloorToInt(this float2 f2) => (int2)math.floor(f2);

        public static int3 FloorToInt(this float3 f3) => (int3)math.floor(f3);

        public static float3 Right(this quaternion q) => new float3x3(q).c0;

        public static float3 Up(this quaternion q) => new float3x3(q).c1;

        public static float3 Forward(this quaternion q) => new float3x3(q).c2;

        public static float3 GetPosition(this float4x4 matrix) => matrix.c3.xyz;

        public static quaternion GetRotation(this float4x4 matrix)
        {
            float3 scale = GetScale(matrix);
            float3 inverted = Invert(scale);
            matrix = matrix.ScaleBy(inverted);
            return new quaternion(matrix);
        }

        public static float3 GetScale(this float4x4 matrix) => new float3(
            math.length(matrix.c0.xyz),
            math.length(matrix.c1.xyz),
            math.length(matrix.c2.xyz));

        public static float3 GetScaleSqr(this float4x4 matrix) => new float3(
            math.lengthsq(matrix.c0.xyz),
            math.lengthsq(matrix.c1.xyz),
            math.lengthsq(matrix.c2.xyz));

        public static float4x4 ScaleBy(this float4x4 matrix, float3 scale)
        {
            return math.mul(matrix, float4x4.Scale(scale));
        }

        public static float LargestScale(this float4x4 matrix)
        {
            float3 scaleSqr = matrix.GetScaleSqr();
            float largestScaleSqr = math.cmax(scaleSqr);
            return math.sqrt(largestScaleSqr);
        }

        public static float3 MultiplyPoint3x4(this float4x4 matrix, float3 point)
        {
            return math.mul(matrix, new float4(point, 1.0f)).xyz;
        }

        public static float3 InvertSafe(float3 f)
        {
            float x = Math.Abs(f.x) < float.Epsilon ? 0 : 1 / f.x;
            float y = Math.Abs(f.y) < float.Epsilon ? 0 : 1 / f.y;
            float z = Math.Abs(f.z) < float.Epsilon ? 0 : 1 / f.z;

            return new float3(x, y, z);
        }

        public static float3 Invert(float3 f)
        {
            Assert.IsTrue(Math.Abs(f.x) > float.Epsilon);
            Assert.IsTrue(Math.Abs(f.y) > float.Epsilon);
            Assert.IsTrue(Math.Abs(f.z) > float.Epsilon);

            return new float3(1 / f.x, 1 / f.y, 1 / f.z);
        }
    }
}
