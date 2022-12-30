using NBomber.Contracts;
using NBomber.CSharp;
using NBomber.Runner.Abstraction;
using NBomber.Tests;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

[assembly: MarkAssemblyAttribure(typeof(Setup))]

namespace NBomber.Tests
{

    public class Setup : ISetup
    {
        IEnumerable<ScenarioProps> ISetup.Setup()
        {
            yield return Scenario.Create("scenario_1", async context =>
            {
                var step1 = await Step.Run("step_1", context, async () =>
                {
                    await Task.Delay(1000);
                    return Response.Ok(payload: "step_1 response", sizeBytes: 10);
                });

                var step2 = await Step.Run("step_2", context, async () =>
                {
                    await Task.Delay(1000);
                    return Response.Ok(payload: "step_2 response", sizeBytes: 10);
                });

                return step1.Payload.Value == "step_1 response" && step2.Payload.Value == "step_2 response"
                    ? Response.Ok(statusCode: "200")
                    : Response.Fail(statusCode: "500");
            })
.WithoutWarmUp()
.WithLoadSimulations(
    Simulation.Inject(rate: 10, interval: TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(20)),
    Simulation.KeepConstant(copies: 50, during: TimeSpan.FromSeconds(30))
);

         yield return Scenario.Create("scenario_2", async context =>
            {
                await Task.Delay(1000);
                return Response.Ok();
            })
        .WithoutWarmUp()
        .WithLoadSimulations(Simulation.KeepConstant(copies: 1, during: TimeSpan.FromSeconds(10)));
            
            yield return Scenario.Empty("sc_3")
                .WithInit(async context =>
            {
                // here we can populate our DB
                await Task.Delay(5000);
            })
            .WithClean(async context =>
            {
                // here we can do a cleanup for our DB
                await Task.Delay(5000);
            }); ;
        }
    }
}