namespace Morpeh.Globals {
    using System.Collections.Generic;
    using UnityEngine;
    using System;
    using System.Globalization;
    using System.Linq;
    using Unity.IL2CPP.CompilerServices;

    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
    [CreateAssetMenu(menuName = "ECS/Globals/Lists/Variable List Int")]
    public class GlobalVariableListInt : BaseGlobalVariable<List<int>> {
        public override List<int> Deserialize(string serializedData) => JsonUtility.FromJson<ListIntWrapper>(serializedData).list;

        public override string Serialize(List<int> data) => JsonUtility.ToJson(new ListIntWrapper {list = data});

        [Il2CppSetOption(Option.NullChecks, false)]
        [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
        [Il2CppSetOption(Option.DivideByZeroChecks, false)]
        [Serializable]
        private class ListIntWrapper {
            public List<int> list;
        }
    }
}