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
    [CreateAssetMenu(menuName = "ECS/Globals/Lists/Variable List Bool")]
    public class GlobalVariableListBool : BaseGlobalVariable<List<bool>> {
        public override List<bool> Deserialize(string serializedData) => JsonUtility.FromJson<ListBoolWrapper>(serializedData).list;

        public override string Serialize(List<bool> data) => JsonUtility.ToJson(new ListBoolWrapper {list = data});

        [Il2CppSetOption(Option.NullChecks, false)]
        [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
        [Il2CppSetOption(Option.DivideByZeroChecks, false)]
        [Serializable]
        private class ListBoolWrapper {
            public List<bool> list;
        }
    }
}