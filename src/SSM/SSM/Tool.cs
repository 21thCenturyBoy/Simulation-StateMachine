using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace SSM
{
    public static class Tool
    {

    }


    [Serializable]
    public struct TagPair : IEqualityComparer, IEqualityComparer<TagPair>
    {
        private int key;
        private int value;

        public bool Equals(TagPair other)
        {
            return key == other.key && value == other.value;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is TagPair other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (key * 397) ^ value;
            }
        }

        public TagPair(int key, int value)
        {
            this.key = key;
            this.value = value;
        }

        public int Key
        {
            get { return this.key; }
        }

        public int Value
        {
            get { return this.value; }
        }

        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append('[');
            stringBuilder.Append(this.Key.ToString());
            stringBuilder.Append(", ");
            stringBuilder.Append(this.Value.ToString());
            stringBuilder.Append(']');
            return stringBuilder.ToString();
        }

        public bool Equals(object x, object y)
        {
            if (ReferenceEquals(null, x)) return false;
            if (ReferenceEquals(null, y)) return false;
            return ((TagPair)x).Equals(y);
        }

        public int GetHashCode(object obj)
        {
            if (obj == null) return 0;
            return obj.GetHashCode();
        }

        public bool Equals(TagPair x, TagPair y) => x.Equals(y);


        public int GetHashCode(TagPair obj) => obj.GetHashCode();
    }




    public class BatchTag
    {
        public FiniteState State;
        public List<TagPair> TagList;

        public BatchTag(FiniteState state, List<TagPair> tagList)
        {
            State = state;
            TagList = tagList;
        }


    }
    public class BatchOrder
    {
        private Dictionary<int, Dictionary<int, Func<bool>>> _batchsInfoDic;

        public BatchOrder()
        {
            _batchsInfoDic = new Dictionary<int, Dictionary<int, Func<bool>>>();
        }

    }
}
