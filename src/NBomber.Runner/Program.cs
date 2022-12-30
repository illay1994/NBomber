using NBomber.Contracts;
using NBomber.CSharp;
using NBomber.Runner.Abstraction;
using Spectre.Console;
using System;
using System.Reflection;

var scen = GetScenarioFromAssembly(args).ToArray();

if (scen.Length == 0)
{
    Console.Error.WriteLine("No scen found");
    return;
}
NBomberRunner
           .RegisterScenarios(scen)
           .Run();
IEnumerable<ScenarioProps> GetScenarioFromAssembly(params string[] args)
{
    foreach (var arg in args)
    {
        Assembly assembly;
        try
        {
            assembly = Assembly.LoadFile(arg);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
            continue;
        }
        foreach (var attribute in assembly.GetCustomAttributes<MarkAssemblyAttribure>())
        {
            var hostingStartup = (ISetup)Activator.CreateInstance(attribute.Type)!;
            foreach (var p in hostingStartup.Setup())
            {
                yield return p;
            }
        }

    }
}