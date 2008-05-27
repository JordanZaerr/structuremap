using System;
using System.Collections.Generic;

namespace StructureMap.Pipeline
{
    public class ExplicitArguments
    {
        private readonly Dictionary<string, object> _args;
        private readonly Dictionary<Type, object> _children = new Dictionary<Type, object>();

        public ExplicitArguments(Dictionary<string, object> args)
        {
            _args = args;
        }

        public ExplicitArguments() : this(new Dictionary<string, object>())
        {
        }

        public T Get<T>() where T : class
        {
            return (T) Get(typeof (T));
        }

        public object Get(Type type)
        {
            return _children.ContainsKey(type) ? _children[type] : null;
        }

        public void Set<T>(T arg)
        {
            _children.Add(typeof (T), arg);
        }

        public void SetArg(string key, object argValue)
        {
            _args.Add(key, argValue);
        }

        public object GetArg(string key)
        {
            return _args.ContainsKey(key) ? _args[key] : null;
        }

        public void Configure(ConfiguredInstance instance)
        {
            foreach (KeyValuePair<string, object> arg in _args)
            {
                instance.SetProperty(arg.Key, arg.Value.ToString());
                instance.Child(arg.Key).Is(arg.Value);
            }
        }

        public bool Has(Type type)
        {
            return _children.ContainsKey(type);
        }

        public bool Has(string propertyName)
        {
            return _args.ContainsKey(propertyName);
        }
    }

    public class ExplicitInstance<PLUGINTYPE> : ConfiguredInstance
    {
        private readonly ExplicitArguments _args;

        public ExplicitInstance(ExplicitArguments args, Instance defaultInstance)
        {
            args.Configure(this);
            _args = args;

            ConfiguredInstance defaultConfiguration = defaultInstance as ConfiguredInstance;
            if (defaultConfiguration != null)
            {
                mergeIntoThis(defaultConfiguration);
            }
            else
            {
                setPluggedType(typeof(PLUGINTYPE));
            }
        }


        protected override object getChild(string propertyName, Type pluginType, IBuildSession buildSession)
        {
            if (_args.Has(pluginType))
            {
                return _args.Get(pluginType);
            }

            if (_args.Has(propertyName))
            {
                return _args.GetArg(propertyName);
            }

            return base.getChild(propertyName, pluginType, buildSession);
        }
    }
}