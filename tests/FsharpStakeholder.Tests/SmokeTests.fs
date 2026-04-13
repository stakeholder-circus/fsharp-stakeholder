module FsharpStakeholder.Tests.SmokeTests

open System
open System.IO
open System.Linq
open System.Text.Json
open FsharpStakeholder
open Xunit

let private parseJson (text: string) =
    JsonSerializer.Deserialize<JsonElement>(text, JsonSerializerOptions(PropertyNameCaseInsensitive = true))

let private runAndCaptureStdout (args: string array) =
    let originalOut = Console.Out
    let originalErr = Console.Error
    use stdout = new StringWriter()
    use stderr = new StringWriter()
    Console.SetOut(stdout)
    Console.SetError(stderr)
    try
        let exitCode = FsharpStakeholderApp.run args
        Assert.Equal(0, exitCode)
        stdout.ToString()
    finally
        Console.SetOut(originalOut)
        Console.SetError(originalErr)

let private runAndCaptureError (args: string array) =
    let originalOut = Console.Out
    let originalErr = Console.Error
    use stdout = new StringWriter()
    use stderr = new StringWriter()
    Console.SetOut(stdout)
    Console.SetError(stderr)
    try
        let exitCode = FsharpStakeholderApp.run args
        exitCode, stderr.ToString()
    finally
        Console.SetOut(originalOut)
        Console.SetError(originalErr)

[<Fact>]
let ``list-values exposes the full registry and dedicated renderer keys`` () =
    let payload = runAndCaptureStdout [| "--list-values" |] |> parseJson
    let families = payload.GetProperty("generatorFamilies").EnumerateArray().ToArray()
    Assert.True(families.Length >= 30)

    let requiredDedicated =
        [| "code_analyzer", "classic-six.code_analyzer"
           "data_processing", "classic-six.data_processing"
           "jargon", "classic-six.jargon"
           "metrics", "classic-six.metrics"
           "network_activity", "classic-six.network_activity"
           "system_monitoring", "classic-six.system_monitoring"
           "agent_workflows", "modern-core.agent_workflows"
           "platform_engineering", "modern-core.platform_engineering"
           "observability_ai_runtime", "modern-core.observability_ai_runtime"
           "delivery_preview_ops", "modern-core.delivery_preview_ops"
           "supply_chain_security", "modern-core.supply_chain_security" |]

    requiredDedicated
    |> Array.iter (fun (familyId, rendererKey) ->
        let family = families.Single(fun item -> item.GetProperty("id").GetString() = familyId)
        Assert.Equal(rendererKey, family.GetProperty("rendererKey").GetString())
        Assert.True(family.GetProperty("smoke").GetBoolean()))

[<Theory>]
[<InlineData("code_analyzer", "classic-six.code_analyzer", "analysisFocus", "typed interfaces, agent-authored patches, and MCP assumptions")>]
[<InlineData("data_processing", "classic-six.data_processing", "dataWindow", "embeddings, semantic chunks, and batch transforms with deterministic ordering")>]
[<InlineData("jargon", "classic-six.jargon", "languagePolicy", "credible 2026 terminology instead of fake-deep phrasing")>]
[<InlineData("metrics", "classic-six.metrics", "signalBlend", "queue depth, token spend, and GPU occupancy in a single operations lane")>]
[<InlineData("network_activity", "classic-six.network_activity", "transportMix", "RPC, event-stream, and adapter traffic under deterministic retry rules")>]
[<InlineData("system_monitoring", "classic-six.system_monitoring", "telemetryScope", "collector pressure, runner health, and policy-denial signals across the stack")>]
[<InlineData("agent_workflows", "modern-core.agent_workflows", "coordinationMode", "delegated agent work, approval gates, and cross-repo handoff envelopes")>]
[<InlineData("platform_engineering", "modern-core.platform_engineering", "platformSurface", "golden paths, identity boundaries, and queue ownership in the shared platform lane")>]
[<InlineData("observability_ai_runtime", "modern-core.observability_ai_runtime", "runtimeSignals", "trace spans, token burn, GPU pressure, and policy denials in one runtime lane")>]
[<InlineData("delivery_preview_ops", "modern-core.delivery_preview_ops", "deliveryGuardrail", "preview deploys, canaries, release flags, and rollback checkpoints under seed control")>]
[<InlineData("supply_chain_security", "modern-core.supply_chain_security", "supplyChainPosture", "provenance, attestations, dependency drift, and secret exposure in one security lane")>]
let ``dedicated families emit expected metadata`` familyId rendererKey focusKey focusValue =
    let session =
        runAndCaptureStdout
            [| "--dev-type"; "backend"; "--complexity"; "medium"; "--seed"; familyId + "-seed"; "--focus-family"; familyId; "--output-format"; "json" |]
        |> parseJson

    let activity =
        session.GetProperty("events").EnumerateArray()
        |> Seq.find (fun item ->
            item.GetProperty("eventType").GetString() = "generator.activity"
            && item.GetProperty("context").GetProperty("family").GetString() = familyId)

    let message = activity.GetProperty("message").GetString()
    Assert.Equal(rendererKey, activity.GetProperty("context").GetProperty("renderer").GetString())
    Assert.Equal("dedicated first-push renderer", activity.GetProperty("context").GetProperty("detail").GetString())
    Assert.Equal(focusKey, activity.GetProperty("context").GetProperty("familyFocusKey").GetString())
    Assert.Equal(focusValue, activity.GetProperty("context").GetProperty(focusKey).GetString())
    Assert.Equal("rust-stakeholder", activity.GetProperty("context").GetProperty("traceabilitySourceRepo").GetString())
    Assert.Equal("java-stakeholder", activity.GetProperty("context").GetProperty("traceabilityJavaRepo").GetString())
    Assert.Equal("stakeholder-core", activity.GetProperty("context").GetProperty("traceabilityContractRepo").GetString())
    Assert.Equal("full-parity", activity.GetProperty("context").GetProperty("traceabilityParityClass").GetString())
    Assert.Contains("Java, Rust, and stakeholder-core", message, StringComparison.Ordinal)

[<Fact>]
let ``deterministic json stays stable for the same seed`` () =
    let args = [| "--dev-type"; "backend"; "--complexity"; "medium"; "--seed"; "first-push-stability"; "--focus-family"; "code_analyzer"; "--output-format"; "json" |]
    let first = runAndCaptureStdout args
    let second = runAndCaptureStdout args
    Assert.Equal(first, second)

[<Fact>]
let ``experimental provider flags fail fast`` () =
    let exitCode, stderr = runAndCaptureError [| "--experimental-provider"; "openai-compatible" |]
    Assert.Equal(2, exitCode)
    Assert.Contains("experimental-provider is not implemented yet in fsharp-stakeholder", stderr, StringComparison.OrdinalIgnoreCase)
