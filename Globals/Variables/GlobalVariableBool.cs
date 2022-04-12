namespace Morpeh.Globals {
    using System.Globalization;
    using UnityEngine;
    using Unity.IL2CPP.CompilerServices;

    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
    [CreateAssetMenu(menuName = "ECS/Globals/Variables/Variable Bool")]
    public class GlobalVariableBool : BaseGlobalVariable<bool> {
        public override bool Deserialize(string serializedData) => bool.Parse(serializedData);

        public override string Serialize(bool data) => data.ToString(CultureInfo.InvariantCulture);
    }
}