using Esprima.Ast;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace DotNetTools.SharpGrabber.BlackWidow.Interpreter.Api.v1
{
    public class GrabResponse
    {
        private readonly GrabResult _grabResult;
        private readonly IGrabberServices _grabberServices;
        private readonly ICollection<IGrabbed> _grabbedCollection;

        public GrabResponse(GrabResult grabResult, ICollection<IGrabbed> grabbedCollection, IGrabberServices grabberServices)
        {
            _grabResult = grabResult;
            _grabberServices = grabberServices;
            _grabbedCollection = grabbedCollection;
        }

        public string Title
        {
            get => _grabResult.Title;
            set => _grabResult.Title = value;
        }

        public string Description
        {
            get => _grabResult.Description;
            set => _grabResult.Description = value;
        }

        public DateTime? CreationDate
        {
            get => _grabResult.CreationDate;
            set => _grabResult.CreationDate = value;
        }

        public bool IsSecure
        {
            get => _grabResult.IsSecure;
            set => _grabResult.IsSecure = value;
        }

        public void Grab(string grabbedTypeId, IDictionary<string, object> values)
        {
            var grabbedType = _grabberServices.GetGrabbed(grabbedTypeId);
            if (grabbedType == null)
                throw new NotSupportedException($"Grabbed type '{grabbedType}' is not registered.");

            var grabbed = (IGrabbed)Activator.CreateInstance(grabbedType);
            SetProperties(grabbed, values);
            _grabbedCollection.Add(grabbed);
        }

        private void SetProperties(IGrabbed grabbed, IDictionary<string, object> values)
        {
            var type = grabbed.GetType();
            foreach (var pair in values)
            {
                if (pair.Value == null)
                    continue;

                var prop = type.GetProperty(pair.Key, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                if (prop == null)
                    continue;

                var value = _grabberServices.ChangeType(pair.Value, prop.PropertyType);
                prop.SetValue(grabbed, value);
            }
        }
    }
}
