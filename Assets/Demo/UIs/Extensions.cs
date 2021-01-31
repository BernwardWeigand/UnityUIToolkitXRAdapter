using CoreLibrary;
using JetBrains.Annotations;
using LanguageExt;
using UnityEngine;
using UnityEngine.XR;

namespace Demo.UIs {
    public static class Extensions {
        [Pure]
        public static Option<bool> TryGetFeatureValue(this InputDevice device, InputFeatureUsage<bool> feature) =>
            device.TryGetFeatureValue(feature, out var value) ? value : Option<bool>.None;


        [Pure]
        internal static Option<T> AsOption<T>(this Component comp, Search where = Search.InObjectOnly) where T : class
            => comp.As<T>(where) ?? Option<T>.None;
    }
}