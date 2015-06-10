using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using CppCliBridgeGenerator.Properties;

namespace CppCliBridgeGenerator
{
    /// <summary>
    /// Generator for vcxproj file.
    /// </summary>
    public class WrapperProjectGenerator : Generator
    {
        private readonly List<Generator> generatorChain;
        private readonly HashSet<Assembly> loadedAssemblies = new HashSet<Assembly>();

        /// <summary>
        /// Generator for vcxproj file - project file usable for bridge compilation.
        /// Gets informations about generated files from other generators.
        /// </summary>
        /// <param name="outputFolder">Output folder.</param>
        /// <param name="generatorChain">Chain of all generators.</param>
        public WrapperProjectGenerator(string outputFolder, List<Generator> generatorChain)
            : base(outputFolder)
        {
            this.generatorChain = generatorChain;
        }

        /// <summary>
        /// Adds assembly to collection for references and copying them to output folder
        /// </summary>
        /// <param name="assembly"></param>
        public override void AssemblyLoad(Assembly assembly)
        {
            this.loadedAssemblies.Add(assembly);
        }


        /// <summary>
        /// Finalization of generation - time to generate project file.
        /// </summary>
        public override void GeneratorFinalize()
        {
            var builder = new StringBuilder();
            var filesCpp = new List<string>();
            var filesH = new List<string>();

            // get all generated files
            foreach (var gen in this.generatorChain)
            {
                filesCpp.AddRange(gen.GetGeneratedFiles().FindAll(f => f.EndsWith(".cpp")).Distinct());
                filesH.AddRange(gen.GetGeneratedFiles().FindAll(f => f.EndsWith(".h")).Distinct());
            }

            // build vcxproj file
            builder.AppendLine(@"<?xml version=""1.0"" encoding=""utf-8""?>
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

            // add links to .cpp files
            foreach (var file in filesCpp)
            {
                builder.AppendFormat(@"    <ClCompile Include=""{0}"" />", file);
                builder.AppendLine();
            }

            builder.AppendLine(@"  </ItemGroup>
  <ItemGroup>");

            // add links to .h files
            foreach (var file in filesH)
            {
                builder.AppendFormat(@"    <ClInclude Include=""{0}"" />", file);
                builder.AppendLine();
            }

            builder.AppendLine(@"  </ItemGroup>
  <ItemGroup>");

            // add references to assemblies
            foreach (var assembly in this.loadedAssemblies)
            {
                if (assembly.ManifestModule.Name == "mscorlib.dll") continue;

                builder.AppendFormat(@"    <Reference Include=""{0}"">
      <HintPath>{1}</HintPath>
    </Reference>", assembly.FullName, assembly.ManifestModule.Name);
                builder.AppendLine();
            }

            builder.AppendLine(@"  </ItemGroup>
  <Import Project=""$(VCTargetsPath)\Microsoft.Cpp.Targets"" />
</Project>");



            // create vcxproj file
            WriteToFile(builder.ToString(), GetFileName());

            // INFO: copying assemblies could be controlled via checkbox (they needs to be there anyway)
            // copy assemblies to output folder
            foreach (var assembly in this.loadedAssemblies)
            {
                if (assembly.ManifestModule.Name == "mscorlib.dll") continue;

                string outFile = Path.Combine(this.OutputFolder, assembly.ManifestModule.Name);

                if(File.Exists(assembly.Location) && !File.Exists(outFile))
                    File.Copy(assembly.Location, outFile, true);
            }

            // copy marshaller to output folder
            try
            {
                File.WriteAllText(Path.Combine(this.OutputFolder, "marshaller_ext.h"), Resources.marshaller_ext);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        /// <summary>
        /// Gets output filename of this generator.
        /// </summary>
        /// <returns>Filename</returns>
        public static string GetFileName()
        {
            return Utils.GetWrapperProjectName() + ".vcxproj";
        }

    }
}