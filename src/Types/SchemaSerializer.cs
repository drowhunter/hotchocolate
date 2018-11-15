using System;
using System.Collections.Generic;
using System.Linq;
using HotChocolate.Language;
using HotChocolate.Types;

namespace HotChocolate
{
    public class SchemaSerializer
    {
        public static void Serialize(ISchema schema)
        {






        }

        public static ObjectTypeDefinitionNode SerializeObjectType(
            ObjectType type)
        {
            var name = new NameNode(type.Name);
            StringValueNode description =
                SerializeDescription(type.Description);
            List<NamedTypeNode> interfaces =
                SerializeNamedTypes(type.Interfaces.Values);
            List<DirectiveNode> directives =
                SerializeDirectives(type.Directives);
            List<FieldDefinitionNode> fields =
                SerializeFieldDefinitions(type.Fields);

            return new ObjectTypeDefinitionNode
            (
                null,
                name,
                description,
                directives,
                interfaces,
                fields
            );
        }

        private static List<FieldDefinitionNode> SerializeFieldDefinitions(
            IEnumerable<ObjectField> fields)
        {
            return fields.Select(SerializeFieldDefinition).ToList();
        }

        private static FieldDefinitionNode SerializeFieldDefinition(
            ObjectField field)
        {
            var name = new NameNode(field.Name);
            StringValueNode description =
                SerializeDescription(field.Description);
            List<InputValueDefinitionNode> arguments =
                ParseArgumentDefinitions(context);
            ITypeNode type = SerializeType(field.Type);
            List<DirectiveNode> directives =
                ParseDirectives(context, true);
            Location location = context.CreateLocation(start);

            return new FieldDefinitionNode
            (
                location,
                name,
                description,
                arguments,
                type,
                directives
            );
        }

        private static List<InputValueDefinitionNode> SerializeInputValueDefinitions(
            IEnumerable<InputField> inputField)
        {
            return inputField.Select(t => SerializeInputValueDefinition(t));
        }

        private static InputValueDefinitionNode SerializeInputValueDefinition(
            InputField inputField)
        {
            var name = new NameNode(inputField.Name);
            StringValueNode description =
                SerializeDescription(inputField.Description);
            ITypeNode type = SerializeType(inputField.Type);
            IValueNode defaultValue = inputField.DefaultValue;
            List<DirectiveNode> directives =
                SerializeDirectives(inputField.Directives);

            return new InputValueDefinitionNode
            (
                null,
                name,
                description,
                type,
                defaultValue,
                directives
            );
        }




        private static StringValueNode SerializeDescription(string description)
        {
            return string.IsNullOrEmpty(description)
                ? new StringValueNode(description)
                : null;
        }

        private static List<NamedTypeNode> SerializeNamedTypes(
            IEnumerable<INamedType> types)
        {
            return types.Select(t => new NamedTypeNode(t.Name)).ToList();
        }

        private static List<DirectiveNode> SerializeDirectives(
            IEnumerable<IDirective> directives)
        {
            return directives.Select(t => t.ToNode()).ToList();
        }

        private static ITypeNode SerializeType(IType type)
        {
            if (type is NonNullType nnt)
            {
                return new NonNullTypeNode(
                    (Language.INullableType)SerializeType(nnt.Type));
            }
‚
            if (type is ListType lt)
            {
                return new ListTypeNode(SerializeType(lt.ElementType));
            }

            if (type is INamedType nt)
            {
                return new NamedTypeNode(nt.Name);
            }

            throw new NotSupportedException();
        }




    }
}
