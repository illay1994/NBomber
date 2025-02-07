[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module internal NBomber.Domain.Step

open System
open System.Threading.Tasks
open NBomber.Contracts
open NBomber.Contracts.Internal
open NBomber.Domain.ScenarioContext
open NBomber.Domain.Stats.ScenarioStatsActor

let inline measure (name: string) (ctx: ScenarioContext) (run: unit -> Task<Response<'T>>) = backgroundTask {
    let startTime = ctx.Timer.Elapsed.TotalMilliseconds
    try
        let! response = run()
        let endTime = ctx.Timer.Elapsed.TotalMilliseconds
        let latency = endTime - startTime

        let result = { Name = name; ClientResponse = response; EndTimeMs = endTime; LatencyMs = latency }
        ctx.StatsActor.Publish(AddMeasurement result)
        return response
    with
    | :? OperationCanceledException as ex ->
        let endTime = ctx.Timer.Elapsed.TotalMilliseconds
        let latency = endTime - startTime

        let response = ResponseInternal.failTimeout
        let result = { Name = name; ClientResponse = response; EndTimeMs = endTime; LatencyMs = latency }

        ctx.StatsActor.Publish(AddMeasurement result)
        return response

    | ex ->
        let endTime = ctx.Timer.Elapsed.TotalMilliseconds
        let latency = endTime - startTime

        let context = ctx :> IScenarioContext
        context.Logger.Fatal(ex, $"Unhandled exception for Scenario: {0}, Step: {1}", context.ScenarioInfo.ScenarioName, name)

        let response = ResponseInternal.failUnhandled ex
        let result = { Name = name; ClientResponse = response; EndTimeMs = endTime; LatencyMs = latency }

        ctx.StatsActor.Publish(AddMeasurement result)
        return response
}
