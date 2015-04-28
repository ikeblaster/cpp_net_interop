using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using ManagedToNativeWrapperGenerator.Properties;

namespace ManagedToNativeWrapperGenerator
{
    public class WrapperProjectGenerator : TypeGenerator
    {
        private readonly List<TypeGenerator> generatorChain;
        private readonly HashSet<Assembly> loadedAssemblies = new HashSet<Assembly>();

        public WrapperProjectGenerator(string outputFolder, List<TypeGenerator> generatorChain)
            : base(outputFolder)
        {
            this.generatorChain = generatorChain;
        }

        public override void AssemblyLoad(Assembly assembly)
        {
            this.loadedAssemblies.Add(assembly);
        }

        public override void GeneratorFinalize()
        {
            var builder = new StringBuilder();
            var filesCpp = new List<string>();
            var filesH = new List<string>();

            foreach (var gen in this.generatorChain)
            {
                filesCpp.AddRange(gen.GetGeneratedFiles().FindAll(f => f.EndsWith(".cpp")).Distinct());
                filesH.AddRange(gen.GetGeneratedFiles().FindAll(f => f.EndsWith(".h")).Distinct());
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




            WriteToFile(builder.ToString(), GetFileName());

            // TODO: checkbox, copy assemblies
            foreach (var assembly in this.loadedAssemblies)
            {
                string outFile = Path.Combine(this.OutputFolder, assembly.ManifestModule.Name);

                if(File.Exists(assembly.Location) && !File.Exists(outFile))
                    File.Copy(assembly.Location, outFile, true);
            }

            try
            {
                File.WriteAllText(Path.Combine(this.OutputFolder, "marshaler_ext.h"), Resources.marshaler_ext);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
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