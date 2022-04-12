namespace Morpeh.Globals {
    using System.Globalization;
    using UnityEngine;
    using Unity.IL2CPP.CompilerServices;

    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
    [CreateAssetMenu(menuName = "ECS/Globals/Variables/Variable Int")]
    public class GlobalVariableInt : BaseGlobalVariable<int> {
        public override int Deserialize(string serializedData) => int.Parse(serializedData, CultureInfo.InvariantCulture);

        public override string Serialize(int data) => data.ToString(CultureInfo.InvariantCulture);
    }
}