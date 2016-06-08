using System.Reflection;
using System.Runtime.CompilerServices;

#if DEBUG
[assembly: AssemblyProduct("Artemis Tests (Debug)")]
[assembly: AssemblyConfiguration("Debug")]
#else
    [assembly: AssemblyProduct("Artemis Tests (Release)")]
    [assembly: AssemblyConfiguration("Release")]
#endif

[assembly: AssemblyVersion("1.0.0.*")]
[assembly: AssemblyFileVersion("1.0.0.1")]
[assembly: AssemblyInformationalVersion("Developer Build")]

// General Information about an assembly is controlled through the following
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("Artemis Tests")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyCompany("")]
[assembly: InternalsVisibleTo("artemis.tests")]
