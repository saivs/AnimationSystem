using System;

namespace Saivs.Animation
{
    [Serializable]
    public struct StringHash : IEquatable<StringHash>
    {
        public uint Id;

        public StringHash(string str) =>
            Id = GetStableHashCode(str);

        public static implicit operator StringHash(string str) =>
            new StringHash(str);

        public static implicit operator StringHash(uint id) =>
            new StringHash { Id = id };

        public static bool IsNullOrEmpty(StringHash strHash) =>
            strHash.Id == 0;

        public static bool operator ==(StringHash lhs, StringHash rhs) =>
            lhs.Id == rhs.Id;

        public static bool operator !=(StringHash lhs, StringHash rhs) =>
            lhs.Id != rhs.Id;

        public bool Equals(StringHash other) =>
            Id == other.Id;

        public override int GetHashCode() =>
            (int)Id;

        public override bool Equals(object other)
        {
            if (other == null || !(other is StringHash))
                return false;

            return Id == ((StringHash)other).Id;
        }

        // string.GetHashCode is not guaranteed to be the same on all machines, but
        // we need one that is the same on all machines. simple and stupid:
        private static uint GetStableHashCode(string text)
        {
            unchecked
            {
                uint hash = 0;

                if (string.IsNullOrEmpty(text))
                    return hash;

                hash = 23;

                foreach (char c in text)
                    hash = hash * 31 + c;

                return hash;
            }
        }
    }
}