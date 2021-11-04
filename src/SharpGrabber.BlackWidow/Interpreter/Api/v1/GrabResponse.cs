using Esprima.Ast;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Reflection;
using System.Text;

namespace DotNetTools.SharpGrabber.BlackWidow.Interpreter.Api.v1
{
    public class GrabResponse
    {
        private readonly GrabResult _grabResult;
        private readonly IGrabberServices _grabberServices;
        private readonly IGrabbedTypeCollection _grabbedTypeCollection;
        private readonly IApiTypeConverter _typeConverter;
        private readonly ICollection<IGrabbed> _grabbedCollection;

        public GrabResponse(GrabResult grabResult, ICollection<IGrabbed> grabbedCollection, IGrabberServices grabberServices,
            IGrabbedTypeCollection grabbedTypeCollection, IApiTypeConverter typeConverter)
        {
            _grabResult = grabResult;
            _grabberServices = grabberServices;
            _typeConverter = typeConverter;
            _grabbedTypeCollection = grabbedTypeCollection;
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
            var grabbedType = _grabbedTypeCollection.GetGrabbed(grabbedTypeId);
            if (grabbedType == null)
                throw new NotSupportedException($"Grabbed type '{grabbedTypeId}' is not registered.");

            var grabbed = (IGrabbed)Activator.CreateInstance(grabbedType);
            SetProperties(grabbed, values);
            _grabbedCollection.Add(grabbed);
        }

        private void SetProperties(object obj, IDictionary<string, object> values)
        {
            if (obj == null)
                return;

            var type = obj.GetType();
            foreach (var pair in values)
            {
                if (pair.Value == null)
                    continue;

                var prop = type.GetProperty(pair.Key, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                if (prop == null)
                    continue;

                if (pair.Value is IDictionary<string, object> map)
                {
                    if (map.Count == 0)
                        return;
                    var innerObject = prop.GetValue(obj);
                    if (innerObject == null)
                    {
                        innerObject = Activator.CreateInstance(prop.PropertyType);
                        prop.SetValue(obj, innerObject);
                    }
                    SetProperties(innerObject, map);
                    return;
                }

                var value = _typeConverter.ChangeType(pair.Value, prop.PropertyType);
                prop.SetValue(obj, value);
            }
        }
    }
}
