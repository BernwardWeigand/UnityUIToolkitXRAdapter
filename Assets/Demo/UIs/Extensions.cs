using JetBrains.Annotations;
using UnityEngine.XR;

namespace Demo.UIs {
    public static class Extensions {
        [Pure]
        public static bool? TryGetFeatureValue(this InputDevice device, InputFeatureUsage<bool> feature) =>
            device.TryGetFeatureValue(feature, out var value) ? value : (bool?) null;
    }
}