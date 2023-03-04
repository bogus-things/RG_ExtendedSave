using BepInEx;
using BepInEx.Bootstrap;
using BepInEx.Preloader.Core.Patching;
using Mono.Cecil;
using Mono.Cecil.Cil;
using System;

namespace RGExtendedSavePatcher
{
    [PatcherPluginInfo(GUID, PluginName, Version)]
    public class Patcher : BasePatcher
    {
        public const string PluginName = "RG ExtendedSave Patcher";
        public const string GUID = "com.bogus.RGExtendedSavePatcher";
        public const string Version = "0.0.1";

        private const string ExtendedSaveDataPropertyName = "ExtendedSaveData";

        private bool patched = false;

        [TargetAssembly("Assembly-CSharp.dll")]
        public void Patch(AssemblyDefinition ass)
        {
            TypeDefinition messagePackObject;

            //Body
            messagePackObject = ass.MainModule.GetType("Chara.ChaFileBody");
            PropertyInject(ass, messagePackObject, ExtendedSaveDataPropertyName, typeof(object));

            //Face
            messagePackObject = ass.MainModule.GetType("Chara.ChaFileFace");
            PropertyInject(ass, messagePackObject, ExtendedSaveDataPropertyName, typeof(object));
            messagePackObject = ass.MainModule.GetType("Chara.ChaFileFace/EyesInfo");
            PropertyInject(ass, messagePackObject, ExtendedSaveDataPropertyName, typeof(object));
            messagePackObject = ass.MainModule.GetType("Chara.ChaFileFace/MakeupInfo");
            PropertyInject(ass, messagePackObject, ExtendedSaveDataPropertyName, typeof(object));

            //Hair
            messagePackObject = ass.MainModule.GetType("Chara.ChaFileHair");
            PropertyInject(ass, messagePackObject, ExtendedSaveDataPropertyName, typeof(object));
            messagePackObject = ass.MainModule.GetType("Chara.ChaFileHair/PartsInfo");
            PropertyInject(ass, messagePackObject, ExtendedSaveDataPropertyName, typeof(object));
            messagePackObject = ass.MainModule.GetType("Chara.ChaFileHair/PartsInfo/BundleInfo");
            PropertyInject(ass, messagePackObject, ExtendedSaveDataPropertyName, typeof(object));
            messagePackObject = ass.MainModule.GetType("Chara.ChaFileHair/PartsInfo/ColorInfo");
            PropertyInject(ass, messagePackObject, ExtendedSaveDataPropertyName, typeof(object));

            //Clothes
            messagePackObject = ass.MainModule.GetType("Chara.ChaFileClothes");
            PropertyInject(ass, messagePackObject, ExtendedSaveDataPropertyName, typeof(object));
            messagePackObject = ass.MainModule.GetType("Chara.ChaFileClothes/PartsInfo");
            PropertyInject(ass, messagePackObject, ExtendedSaveDataPropertyName, typeof(object));
            messagePackObject = ass.MainModule.GetType("Chara.ChaFileClothes/PartsInfo/ColorInfo");
            PropertyInject(ass, messagePackObject, ExtendedSaveDataPropertyName, typeof(object));

            //Accessory
            messagePackObject = ass.MainModule.GetType("Chara.ChaFileAccessory");
            PropertyInject(ass, messagePackObject, ExtendedSaveDataPropertyName, typeof(object));
            messagePackObject = ass.MainModule.GetType("Chara.ChaFileAccessory/PartsInfo");
            PropertyInject(ass, messagePackObject, ExtendedSaveDataPropertyName, typeof(object));
            messagePackObject = ass.MainModule.GetType("Chara.ChaFileAccessory/PartsInfo/ColorInfo");
            PropertyInject(ass, messagePackObject, ExtendedSaveDataPropertyName, typeof(object));

            messagePackObject = ass.MainModule.GetType("Chara.ChaFileGameInfo");
            PropertyInject(ass, messagePackObject, ExtendedSaveDataPropertyName, typeof(object));

            messagePackObject = ass.MainModule.GetType("Chara.ChaFileParameter");
            PropertyInject(ass, messagePackObject, ExtendedSaveDataPropertyName, typeof(object));

            messagePackObject = ass.MainModule.GetType("Chara.ChaFileStatus");
            PropertyInject(ass, messagePackObject, ExtendedSaveDataPropertyName, typeof(object));

            patched = true;
        }

        public override void Initialize()
        {
            DefaultAssemblyResolver resolver = (DefaultAssemblyResolver) TypeLoader.ReaderParameters.AssemblyResolver;
            resolver.AddSearchDirectory($"{Paths.BepInExRootPath}\\Unhollowed\\RoomGirl\\unhollowed");
            resolver.AddSearchDirectory($"{Paths.BepInExRootPath}\\Unhollowed\\RoomGirl\\unity-libs");

            base.Initialize();
        }

        public override void Finalizer()
        {
            if (patched)
            {
                Log.LogMessage("done done done done done");
            }
            else
            {
                Log.LogWarning("oops oops oops");
            }
        }

        private void PropertyInject(AssemblyDefinition assembly, TypeDefinition assemblyTypes, string propertyName, Type returnType)
        {
            //Import the return type
            var propertyType = assembly.MainModule.ImportReference(returnType);

            //define the field we store the value in
            var field = new FieldDefinition(ConvertToFieldName(propertyName), FieldAttributes.Private, propertyType);
            assemblyTypes.Fields.Add(field);

            //Create the get method
            var get = new MethodDefinition("get_" + propertyName, MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig, propertyType);
            var getProcessor = get.Body.GetILProcessor();
            getProcessor.Append(getProcessor.Create(OpCodes.Ldarg_0));
            getProcessor.Append(getProcessor.Create(OpCodes.Ldfld, field));
            getProcessor.Append(getProcessor.Create(OpCodes.Stloc_0));
            var inst = getProcessor.Create(OpCodes.Ldloc_0);
            getProcessor.Append(getProcessor.Create(OpCodes.Br_S, inst));
            getProcessor.Append(inst);
            getProcessor.Append(getProcessor.Create(OpCodes.Ret));
            get.Body.Variables.Add(new VariableDefinition(propertyType));
            get.Body.InitLocals = true;
            get.SemanticsAttributes = MethodSemanticsAttributes.Getter;
            assemblyTypes.Methods.Add(get);

            //Create the set method
            var set = new MethodDefinition("set_" + propertyName, MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig, assembly.MainModule.ImportReference(typeof(void)));
            var setProcessor = set.Body.GetILProcessor();
            setProcessor.Append(setProcessor.Create(OpCodes.Ldarg_0));
            setProcessor.Append(setProcessor.Create(OpCodes.Ldarg_1));
            setProcessor.Append(setProcessor.Create(OpCodes.Stfld, field));
            setProcessor.Append(setProcessor.Create(OpCodes.Ret));
            set.Parameters.Add(new ParameterDefinition(propertyType) { Name = "value" });
            set.SemanticsAttributes = MethodSemanticsAttributes.Setter;
            assemblyTypes.Methods.Add(set);

            //create the property
            var propertyDefinition = new PropertyDefinition(propertyName, PropertyAttributes.None, propertyType) { GetMethod = get, SetMethod = set };

            //add the property to the type.
            assemblyTypes.Properties.Add(propertyDefinition);
        }

        private string ConvertToFieldName(string propertyName)
        {
            var fieldName = new System.Text.StringBuilder();
            fieldName.Append("_");
            fieldName.Append(propertyName[0].ToString().ToLower());
            if (propertyName.Length > 1)
                fieldName.Append(propertyName.Substring(1));

            return fieldName.ToString();
        }
    }
}
