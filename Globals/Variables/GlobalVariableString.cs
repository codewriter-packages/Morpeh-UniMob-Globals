namespace Morpeh.Globals {
    using UnityEngine;
    using Unity.IL2CPP.CompilerServices;

    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
    [CreateAssetMenu(menuName = "ECS/Globals/Variables/Variable String")]
    public class GlobalVariableString : BaseGlobalVariable<string> {
        public override string Deserialize(string serializedData) => serializedData;

        public override string Serialize(string data) => data;
    }
}