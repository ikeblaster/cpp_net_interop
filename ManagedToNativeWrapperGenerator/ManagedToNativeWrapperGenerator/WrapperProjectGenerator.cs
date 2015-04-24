using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.IO;

namespace ManagedToNativeWrapperGenerator
{
    public class WrapperProjectGenerator : TypeGenerator
    {

        List<TypeGenerator> generatorChain;
        HashSet<Assembly> loadedAssemblies = new HashSet<Assembly>();


        public WrapperProjectGenerator(String outputFolder, List<TypeGenerator> generatorChain)
            : base(outputFolder)
        {
            this.generatorChain = generatorChain;
        }


        public override void AssemblyLoad(Assembly assembly)
        {
            loadedAssemblies.Add(assembly);
        }


        public override void GeneratorFinalize()
        {
            StringBuilder builder = new StringBuilder();
            List<string> filesCpp = new List<string>();
            List<string> filesH = new List<string>();

            foreach (var gen in this.generatorChain)
            {
                filesCpp.AddRange(gen.GetGeneratedFiles().FindAll(f => f.EndsWith(".cpp")));
                filesH.AddRange(gen.GetGeneratedFiles().FindAll(f => f.EndsWith(".h")));
            }

            builder.AppendLine(
@"<?xml version=""1.0"" encoding=""utf-8""?>
<Project DefaultTargets=""Build"" ToolsVersion=""4.0"" xmlns=""http://schemas.microsoft.com/developer/msbuild/2003"">
  <ItemGroup>
    <ProjectConfiguration Include=""Debug|Win32"">
      <Configuration>Debug</Configuration>
      <Platform>Win32</Platform>
    </ProjectConfiguration>
    <ProjectConfiguration Include=""Release|Win32"">
      <Configuration>Release</Configuration>
      <Platform>Win32</Platform>
    </ProjectConfiguration>
  </ItemGroup>
  <Import Project=""$(VCTargetsPath)\Microsoft.Cpp.default.props"" />
  <PropertyGroup>
    <ConfigurationType>DynamicLibrary</ConfigurationType>
    <CLRSupport>true</CLRSupport>
    <RuntimeLibrary>MD</RuntimeLibrary>
    <OutDir>$(OutputPath)\..\</OutDir>
  </PropertyGroup>
  <Import Project=""$(VCTargetsPath)\Microsoft.Cpp.props"" />
  <ItemGroup>");

            foreach (var file in filesCpp)
            {
                builder.AppendFormat(@"    <ClCompile Include=""{0}"" />", file);
                builder.AppendLine();
            }

            builder.AppendLine(
@"  </ItemGroup>
  <ItemGroup>");

            foreach (var file in filesH)
            {
                builder.AppendFormat(@"    <ClInclude Include=""{0}"" />", file);
                builder.AppendLine();
            }

            builder.AppendLine(
@"  </ItemGroup>
  <ItemGroup>");

            foreach (var assembly in this.loadedAssemblies)
            {
                builder.AppendFormat(
@"    <Reference Include=""{0}"">
      <HintPath>{1}</HintPath>
    </Reference>", assembly.FullName, assembly.ManifestModule.Name);
                builder.AppendLine();
            }

            builder.AppendLine(
@"  </ItemGroup>
  <Import Project=""$(VCTargetsPath)\Microsoft.Cpp.Targets"" />
</Project>");


            

            this.WriteToFile(builder.ToString(), GetFileName());

            // TODO: checkbox, copy assemblies
            foreach (var assembly in this.loadedAssemblies)
            {
                File.Copy(assembly.Location, Path.Combine(outputFolder, assembly.ManifestModule.Name), true);
            }

            try
            {
                File.WriteAllText(Path.Combine(outputFolder, "marshaler_ext.h"), ManagedToNativeWrapperGenerator.Properties.Resources.marshaler_ext);
            }
            catch (System.Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message);     	
            }
        }


        public static string GetFileName()
        {
            return Utils.GetWrapperProjectName() + ".vcxproj";
        }

        public override string GetFileNameFor(Type type)
        {
            return GetFileName();
        }

    }
}
