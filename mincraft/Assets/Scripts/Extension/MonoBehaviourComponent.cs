using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Extension {
    [Obsolete("I want generic.")]
    public class MonoEasyBehaviour : MonoBehaviour {

        private Dictionary<Type, Component> _components = null;

        protected Component this[Type targetType] {
            get {
                _components ??= GetType()
                    .GetCustomAttributes(typeof(RequireComponent), true)
                    .SelectMany(componentType => {
                        var req = (RequireComponent)componentType;
                        var result = new List<(Type, Component)>();
                        result.Add((req.m_Type0, GetComponent(req.m_Type0)));
                        if (req.m_Type1 != null)
                            result.Add((req.m_Type1, GetComponent(req.m_Type1)));
                        if (req.m_Type2 != null)
                            result.Add((req.m_Type2, GetComponent(req.m_Type2)));
                        return result;
                    })
                    .Distinct()
                    .ToDictionary(row => row.Item1, row => row.Item2);
                return _components.GetValueOrDefault(targetType);
            }
        }

        protected T Component<T>() where T : Component => this[typeof(T)] as T;
    }
}