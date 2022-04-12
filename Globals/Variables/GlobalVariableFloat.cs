namespace Morpeh.Globals {
    using System.Globalization;
    using UnityEngine;
    using Unity.IL2CPP.CompilerServices;

    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
    [CreateAssetMenu(menuName = "ECS/Globals/Variables/Variable Float")]
    public class GlobalVariableFloat : BaseGlobalVariable<float> {
        public override float Deserialize(string serializedData) => float.Parse(serializedData, CultureInfo.InvariantCulture);

        public override string Serialize(float data) => data.ToString(CultureInfo.InvariantCulture);
    }
}