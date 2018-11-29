using System;
using System.Collections.Generic;
using HotChocolate.Utilities;
using HotChocolate.Types;
using System.Linq;

namespace HotChocolate.Configuration
{
    internal partial class SchemaConfiguration
    {
        #region RegisterType - Type
        public void RegisterTypes<TQuery>(){
                RegisterTypes(typeof(TQuery));
        }
        public void RegisterTypes<TQuery, TMutation>(){
                RegisterTypes(typeof(TQuery),typeof(TMutation));
        }
        public void RegisterTypes<TQuery, TMutation, TSubscription>(){
                RegisterTypes(typeof(TQuery),typeof(TMutation),typeof(TSubscription));
        }

        public void RegisterTypes(Type queryType, Type mutationType = null, Type subscriptionType = null)
        {
            IEnumerable<Type> graphQlTypes = System.Reflection.Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(x => !x.IsAbstract &&
                            (typeof(INamedType).IsAssignableFrom(x)
                            ));


            foreach (Type type in graphQlTypes)
            {
                INamedType namedType = RegisterObjectType(type);

                if(type.FullName == queryType.FullName)
                    Options.QueryTypeName = namedType.Name;
                else if(type.FullName == mutationType?.FullName)
                    Options.MutationTypeName = namedType.Name;
                else if(type.FullName == subscriptionType?.FullName)
                    Options.SubscriptionTypeName = namedType.Name;
            }
            
        }

        public void RegisterType<T>()
        {
            RegisterType(typeof(T));
        }

        public void RegisterType(Type type)
        {
            if (type.IsDefined(typeof(GraphQLResolverOfAttribute), false))
            {
                _typeRegistry.RegisterResolverType(type);
            }
            else
            {
                CreateAndRegisterType(type);
            }
        }

        public void RegisterQueryType<T>() where T : class
        {
            RegisterQueryType(typeof(T));
        }
        public void RegisterQueryType(Type type)
        {
            INamedType namedType = RegisterObjectType(type);
            Options.QueryTypeName = namedType.Name;
        }


        public void RegisterMutationType<T>() where T : class
        {
            RegisterMutationType(typeof(T));
        }
        public void RegisterMutationType(Type type)
        {
            INamedType namedType = RegisterObjectType(type);
            Options.MutationTypeName = namedType.Name;
        }


        public void RegisterSubscriptionType<T>() where T : class
        {
            RegisterSubscriptionType(typeof(T));
        }
        public void RegisterSubscriptionType(Type type)
        {
            INamedType namedType = RegisterObjectType(type);
            Options.SubscriptionTypeName = namedType.Name;
        }

        private INamedType RegisterObjectType<T>() where T : class
        {
            return RegisterObjectType(typeof(T));
        }
        private INamedType RegisterObjectType(Type type)
        {
            return typeof(ObjectType).IsAssignableFrom(type)
                ? CreateAndRegisterType(type)
                : CreateAndRegisterType(typeof(ObjectType<>).MakeGenericType(type));
        }

        

        private INamedType CreateAndRegisterType(Type type)
        {
            if (BaseTypes.IsNonGenericBaseType(type))
            {
                throw new SchemaException(new SchemaError(
                    "You cannot add a type without specifing its " +
                    "name and attributes."));
            }

            TypeReference typeReference = type.GetOutputType();
            _typeRegistry.RegisterType(typeReference);
            return _typeRegistry.GetType<INamedType>(typeReference);
        }

        #endregion

        #region RegisterType - Instance

        public void RegisterType<T>(T namedType)
            where T : class, INamedType
        {
            _typeRegistry.RegisterType(namedType);
        }

        public void RegisterQueryType<T>(T objectType)
            where T : ObjectType
        {
            Options.QueryTypeName = objectType.Name;
            _typeRegistry.RegisterType(objectType);
        }

        public void RegisterMutationType<T>(T objectType)
            where T : ObjectType
        {
            Options.MutationTypeName = objectType.Name;
            _typeRegistry.RegisterType(objectType);
        }

        public void RegisterSubscriptionType<T>(T objectType)
            where T : ObjectType
        {
            Options.SubscriptionTypeName = objectType.Name;
            _typeRegistry.RegisterType(objectType);
        }
        #endregion

        #region Directives

        public void RegisterDirective<T>() where T : DirectiveType, new()
        {
            RegisterDirective(typeof(T));
        }

        public void RegisterDirective(Type type)
        {
            _directiveRegistry.RegisterDirectiveType(type);
        }


        public void RegisterDirective<T>(T directive) where T : DirectiveType
        {
            _directiveRegistry.RegisterDirectiveType(directive);
        }

        #endregion
    }
}
