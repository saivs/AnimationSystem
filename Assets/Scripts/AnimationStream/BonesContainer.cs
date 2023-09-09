using System;

namespace Saivs.Animation
{
    public class BonesContainer
    {
        public int Lenght { get; private set; }
        public ref BoneTransform this[int index]
        {
            get
            {
                if (index >= Lenght)
                    throw new IndexOutOfRangeException();

                return ref _boneTransforms[index];
            }
        }

        private BoneTransform[] _boneTransforms;

        public BonesContainer()
        {
            _boneTransforms = new BoneTransform[50];
        }

        public void Init(int lenght)
        {
            Lenght = lenght;

            if (_boneTransforms.Length < lenght)
                Array.Resize(ref _boneTransforms, lenght);

            for(int i = 0; i < lenght; i++)
            {
                _boneTransforms[i] = BoneTransform.Null;
            }
        }
    }
}
