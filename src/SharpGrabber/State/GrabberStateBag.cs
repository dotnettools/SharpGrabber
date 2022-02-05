using System;
using System.Collections.Generic;
using System.Text;

namespace DotNetTools.SharpGrabber
{
    internal class GrabberStateBag : IGrabberStateBag
    {
        private readonly Dictionary<object, object> _state = new();

        public void Delete(object key)
        {
            _state.Remove(key);
        }

        public object Get(object key, object @default = null)
        {
            return _state.TryGetValue(key, out var result) ? result : @default;
        }

        public void Set(object key, object value)
        {
            _state[key] = value;
        }
    }
}
