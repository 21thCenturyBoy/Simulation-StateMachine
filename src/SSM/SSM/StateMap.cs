using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace SSM
{
    /// <summary>
    /// 状态关系
    /// </summary>
    public class StateRelation
    {
        public int RelationVal;//权值
        public RelationInfo Info;//信息弧

        public StateRelation(int relationVal)
        {
            RelationVal = relationVal;
            Info = null;
        }
    }
    /// <summary>
    /// 关系信息
    /// </summary>
    public class RelationInfo
    {
        public List<Func<bool>> _condiFuncList;

        public RelationInfo()
        {
            _condiFuncList = new List<Func<bool>>();
        }

        public void AddRelationInfos(params Func<bool>[] condiFuncList)
        {
            _condiFuncList.AddRange(condiFuncList);
        }
        public void AddRelationInfo(Func<bool> condiFunc)
        {
            _condiFuncList.Add(condiFunc);
        }
    }

    public class StatePoint
    {
        public FiniteState State;

        private List<TagPair> _tagPairLsit;

        public List<TagPair> TagPairLsit => _tagPairLsit;

        public StatePoint(FiniteState state, params TagPair[] tagPairs)
        {
            State = state;
            if (tagPairs.Length != 0) _tagPairLsit = new List<TagPair>(tagPairs);
        }

        public bool isTagPairListEquals(List<TagPair> otherList, out List<TagPair> donthave)
        {
            donthave = new List<TagPair>();
            foreach (TagPair pair in _tagPairLsit)
            {
                if (!otherList.Contains(pair)) donthave.Add(pair);
            }
            return donthave.Count == 0;
        }
    }
    /// <summary>
    /// 状态图
    /// </summary>
    public class StateMap
    {
        public StateRelation[,] StateMatrix;
        public List<StatePoint> StatePointLsit;

        public int CurrentRelatCount;
        public int CurrentPointCount;

        private Dictionary<TagPair, Func<bool>> condDictionary;//key:TagInfoKeys的字符串，字典查询条件

        public StateMap(Dictionary<TagPair, Func<bool>> cond)
        {
            StatePointLsit = new List<StatePoint>();
            condDictionary = cond;
        }

        public void AddStatePoint(params StatePoint[] statePointLsit)
        {
            StatePointLsit.AddRange(statePointLsit);
            CurrentPointCount += statePointLsit.Length;
        }

        public bool CheckMapInfo()
        {
            foreach (StatePoint point in StatePointLsit)
            {
                foreach (TagPair pair in point.TagPairLsit)
                {
                    if (!condDictionary.ContainsKey(pair)) return false;
                }
            }
            if (CurrentPointCount != StatePointLsit.Count) return false;

            return true;
        }

        public bool CreateStateMap()
        {
            if (!CheckMapInfo()) return false;

            StateMatrix = new StateRelation[CurrentPointCount, CurrentPointCount];
            for (int i = 0; i < CurrentPointCount; i++)
            {
                for (int j = 0; j < CurrentPointCount; j++)
                {
                    if (i.Equals(j)) continue;
                    StateRelation relation = new StateRelation(0);
                    List<TagPair> tagPairs;

                    bool res = StatePointLsit[i].isTagPairListEquals(StatePointLsit[j].TagPairLsit, out tagPairs);
                    if (!res) //目前只关心结果不一样，不一样的就是跳转条件集合
                    {
                        relation.Info = new RelationInfo();
                        foreach (TagPair pair in tagPairs) relation.Info._condiFuncList.Add(condDictionary[pair]);
                    }

                    StateMatrix[i, j] = relation;
                }
            }
            return true;
        }

        public int CreateStateFSM()
        {
            List<FiniteState> states = new List<FiniteState>();
            int cou = 0;
            for (int i = 0; i < CurrentPointCount; i++)
            {
                for (int j = 0; j < CurrentPointCount; j++)
                {
                    if (i == j) continue;//自身状态==>自身状态

                    StatePointLsit[i].State.To(StatePointLsit[j].State, StateMatrix[j, i].Info._condiFuncList?.ToArray());
                    cou++;
                }
            }

            return cou;
        }
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
}
