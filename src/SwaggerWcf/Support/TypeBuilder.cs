using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.Serialization;

namespace SwaggerWcf.Support
{
    public class TypeBuilder
    {
        public TypeBuilder(string typeName)
        {
            TypeName = typeName;
            Fields = new Dictionary<string, Tuple<Type,bool>>();
        }

        private string TypeName { get; set; }
        private Dictionary<string, Tuple<Type,bool>> Fields { get; set; }

        public Type Type
        {
            get
            {
                return CompileResultType();
            }
        }

        public void AddField(string fieldName, Type fieldType, bool required)
        {
            if (Fields.ContainsKey(fieldName))
                Fields[fieldName] = new Tuple<Type, bool>(fieldType, required);
            else
                Fields.Add(fieldName, new Tuple<Type, bool>(fieldType, required));
        }


        public Type CompileResultType()
        {
            System.Reflection.Emit.TypeBuilder tb = GetTypeBuilder();
            ConstructorBuilder constructor =
                tb.DefineDefaultConstructor(MethodAttributes.Public | MethodAttributes.SpecialName |
                                            MethodAttributes.RTSpecialName);
            
            // NOTE: assuming your list contains Field objects with fields FieldName(string) and FieldType(Type)
            foreach (KeyValuePair<string, Tuple<Type,bool>> field in Fields)
                CreateProperty(tb, field.Key, field.Value.Item1, field.Value.Item2);

            Type objectType = tb.CreateType();
            return objectType;
        }

        private System.Reflection.Emit.TypeBuilder GetTypeBuilder()
        {
            string typeSignature = TypeName;
            AssemblyName an = new AssemblyName(typeSignature);
            AssemblyBuilder assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(an, AssemblyBuilderAccess.Run);
            ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule("MainModule");
            System.Reflection.Emit.TypeBuilder tb = moduleBuilder.DefineType(typeSignature,
                                                                             TypeAttributes.Public |
                                                                             TypeAttributes.Class |
                                                                             TypeAttributes.AutoClass |
                                                                             TypeAttributes.AnsiClass |
                                                                             TypeAttributes.BeforeFieldInit |
                                                                             TypeAttributes.AutoLayout,
                                                                             null);
            return tb;
        }

        private static void CreateProperty(System.Reflection.Emit.TypeBuilder tb, string propertyName, Type propertyType, bool required)
        {
            FieldBuilder fieldBuilder = tb.DefineField("_" + propertyName, propertyType, FieldAttributes.Private);

            PropertyBuilder propertyBuilder = tb.DefineProperty(propertyName, PropertyAttributes.HasDefault, propertyType, null);
            MethodBuilder getPropMthdBldr = tb.DefineMethod("get_" + propertyName,
                                                            MethodAttributes.Public | MethodAttributes.SpecialName |
                                                            MethodAttributes.HideBySig, propertyType, Type.EmptyTypes);
            ILGenerator getIl = getPropMthdBldr.GetILGenerator();

            getIl.Emit(OpCodes.Ldarg_0);
            getIl.Emit(OpCodes.Ldfld, fieldBuilder);
            getIl.Emit(OpCodes.Ret);

            MethodBuilder setPropMthdBldr =
                tb.DefineMethod("set_" + propertyName,
                                MethodAttributes.Public |
                                MethodAttributes.SpecialName |
                                MethodAttributes.HideBySig,
                                null, new[] { propertyType });

            ILGenerator setIl = setPropMthdBldr.GetILGenerator();
            Label modifyProperty = setIl.DefineLabel();
            Label exitSet = setIl.DefineLabel();

            setIl.MarkLabel(modifyProperty);
            setIl.Emit(OpCodes.Ldarg_0);
            setIl.Emit(OpCodes.Ldarg_1);
            setIl.Emit(OpCodes.Stfld, fieldBuilder);

            setIl.Emit(OpCodes.Nop);
            setIl.MarkLabel(exitSet);
            setIl.Emit(OpCodes.Ret);

            Type[] ctorParams = { };
            ConstructorInfo contructorInfo = typeof(DataMemberAttribute).GetConstructor(ctorParams);

            CustomAttributeBuilder customAttrBuilder = new CustomAttributeBuilder(contructorInfo, new object[] { });
            //TODO: set IsRequired here ??

            propertyBuilder.SetCustomAttribute(customAttrBuilder);

            propertyBuilder.SetGetMethod(getPropMthdBldr);
            propertyBuilder.SetSetMethod(setPropMthdBldr);
        }
    }
}
