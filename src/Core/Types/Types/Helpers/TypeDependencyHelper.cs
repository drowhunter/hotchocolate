using System;
using System.Collections.Generic;
using System.Linq;
using HotChocolate.Configuration;
using HotChocolate.Language;
using HotChocolate.Types.Descriptors.Definitions;

namespace HotChocolate.Types
{
    internal static class TypeDependencyHelper
    {
        public static void RegisterDependencies(
            this IInitializationContext context,
            ObjectTypeDefinition definition)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (definition == null)
            {
                throw new ArgumentNullException(nameof(definition));
            }

            context.RegisterDependencyRange(
                definition.Interfaces,
                TypeDependencyKind.Default);

            RegisterDirectiveDependencies(context, definition);
            RegisterFieldDependencies(context, definition.Fields);

            foreach (ObjectFieldDefinition field in definition.Fields)
            {
                if (field.Member != null && field.Resolver == null)
                {
                    context.RegisterResolver(
                        field.Name,
                        field.Member,
                        definition.ClrType,
                        field.ResolverType);
                }
            }
        }

        public static void RegisterDependencies(
            this IInitializationContext context,
            InterfaceTypeDefinition definition)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (definition == null)
            {
                throw new ArgumentNullException(nameof(definition));
            }

            RegisterDirectiveDependencies(context, definition);
            RegisterFieldDependencies(context, definition.Fields);
        }

        public static void RegisterDependencies(
            this IInitializationContext context,
            EnumTypeDefinition definition)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (definition == null)
            {
                throw new ArgumentNullException(nameof(definition));
            }

            RegisterDirectiveDependencies(context, definition);
        }

        public static void RegisterDependencies(
            this IInitializationContext context,
            InputObjectTypeDefinition definition)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (definition == null)
            {
                throw new ArgumentNullException(nameof(definition));
            }

            RegisterDirectiveDependencies(context, definition);

            foreach (InputFieldDefinition field in definition.Fields)
            {
                if (field.Type != null)
                {
                    context.RegisterDependency(field.Type,
                        TypeDependencyKind.Default);
                }

                context.RegisterDependencyRange(
                    field.Directives.Select(t => t.TypeReference),
                    TypeDependencyKind.Completed);
            }
        }

        private static void RegisterDirectiveDependencies<T>(
            this IInitializationContext context,
            TypeDefinitionBase<T> definition)
            where T : class, ISyntaxNode
        {
            context.RegisterDependencyRange(
                definition.Directives.Select(t => t.TypeReference),
                TypeDependencyKind.Completed);
        }

        private static void RegisterFieldDependencies(
            this IInitializationContext context,
            IEnumerable<OutputFieldDefinitionBase> fields)
        {
            foreach (OutputFieldDefinitionBase field in fields)
            {
                if (field.Type != null)
                {
                    context.RegisterDependency(field.Type,
                        TypeDependencyKind.Default);
                }

                context.RegisterDependencyRange(
                    field.Directives.Select(t => t.TypeReference),
                    TypeDependencyKind.Completed);

                RegisterFieldDependencies(context,
                    fields.SelectMany(t => t.Arguments).ToList());
            }
        }

        private static void RegisterFieldDependencies(
            this IInitializationContext context,
            IEnumerable<ArgumentDefinition> fields)
        {
            foreach (ArgumentDefinition field in fields)
            {
                if (field.Type != null)
                {
                    context.RegisterDependency(field.Type,
                        TypeDependencyKind.Completed);
                }

                context.RegisterDependencyRange(
                    field.Directives.Select(t => t.TypeReference),
                    TypeDependencyKind.Completed);
            }
        }
    }
}
