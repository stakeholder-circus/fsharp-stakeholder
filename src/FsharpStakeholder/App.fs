namespace FsharpStakeholder

open System
open System.Collections.Generic
open System.IO
open System.Text.Json

type DedicatedRendererConfig =
    { RendererKey: string
      Accent: string
      FocusKey: string
      FocusValue: string
      RustPath: string
      JavaPath: string
      Phrases: string array }

module FsharpStakeholderApp =
    let private stableHash (value: string) =
        let mutable hash = 2166136261u
        for ch in value do
            hash <- hash ^^^ uint32 (int ch)
            hash <- hash * 16777619u
        int (hash &&& 0x7fffffffu)

    let private normalizeToken (value: string) =
        value.Replace("-", "", StringComparison.Ordinal).Replace("_", "", StringComparison.Ordinal)

    let private hasFlag (args: string array) (name: string) =
        args
        |> Array.exists (fun arg ->
            arg.Equals(name, StringComparison.OrdinalIgnoreCase)
            || arg.StartsWith(name + "=", StringComparison.OrdinalIgnoreCase))

    let private tryGetValue (args: string array) (name: string) =
        args
        |> Array.tryPick (fun arg ->
            if arg.StartsWith(name + "=", StringComparison.OrdinalIgnoreCase) then
                Some(arg.Substring(name.Length + 1))
            else
                None)
        |> function
            | Some value -> Some value
            | None ->
                let mutable result = None
                let mutable index = 0
                while index < args.Length && result.IsNone do
                    if args[index].Equals(name, StringComparison.OrdinalIgnoreCase) && index + 1 < args.Length then
                        result <- Some args[index + 1]
                    index <- index + 1
                result

    let private parseChoice fallback allowed args name =
        match tryGetValue args name with
        | None -> fallback
        | Some value ->
            let normalized = normalizeToken value
            allowed
            |> Array.tryFind (fun candidate -> normalizeToken candidate = normalized)
            |> function
                | Some candidate -> candidate
                | None -> raise (CommandLineException(sprintf "Invalid value '%s' for %s." value name))

    let private parseSessionConfig (args: string array) : SessionConfig =
        let minimal = hasFlag args "--minimal"
        { DevType = parseChoice "fullstack" Registry.devTypes args "--dev-type"
          Complexity = parseChoice "medium" Registry.complexities args "--complexity"
          Jargon = parseChoice "normal" Registry.jargonLevels args "--jargon"
          OutputFormat = parseChoice "text" Registry.outputFormats args "--output-format"
          Seed = defaultArg (tryGetValue args "--seed") "stakeholder-2026"
          Project = defaultArg (tryGetValue args "--project") "stakeholder"
          Framework = defaultArg (tryGetValue args "--framework") "fsharp-stakeholder"
          FocusFamily = tryGetValue args "--focus-family"
          Alerts = hasFlag args "--alerts"
          Team = hasFlag args "--team"
          Minimal = minimal
          Trace = (hasFlag args "--trace") || not minimal }

    let private parseExperimentalConfig (args: string array) =
        let provider = tryGetValue args "--experimental-provider"
        let model = tryGetValue args "--experimental-model"
        let profile = tryGetValue args "--experimental-profile"
        let prompt = tryGetValue args "--experimental-prompt"
        let adapterMode = parseChoice "api" [| "api"; "consumer" |] args "--experimental-adapter-mode"
        let any =
            [ "--experimental-provider"; "--experimental-model"; "--experimental-profile"; "--experimental-prompt"; "--experimental-adapter-mode" ]
            |> List.exists (hasFlag args)
            || provider.IsSome || model.IsSome || profile.IsSome || prompt.IsSome

        { Provider = provider
          Model = model
          Profile = profile
          Prompt = prompt
          AdapterMode = adapterMode
          HasAnyFlag = any }

    let private buildExperimentalProviderMessage (experimental: ExperimentalConfig) =
        let provider = defaultArg experimental.Provider "experimental-provider"
        let details =
            [ experimental.Provider |> Option.map (sprintf "provider=%s")
              experimental.Model |> Option.map (sprintf "model=%s")
              experimental.Profile |> Option.map (sprintf "profile=%s")
              experimental.Prompt |> Option.map (sprintf "prompt=%s")
              Some(sprintf "adapter-mode=%s" experimental.AdapterMode) ]
            |> List.choose id
            |> String.concat ", "

        sprintf "experimental-provider is not implemented yet in fsharp-stakeholder. Requested %s%s."
            provider
            (if String.IsNullOrWhiteSpace details then "" else sprintf " (%s)" details)

    let private buildSeed (config: SessionConfig) =
        String.concat ":" [| config.Seed; config.DevType; config.Complexity; config.Jargon; defaultArg config.FocusFamily ""; config.Project; config.Framework; string config.Alerts; string config.Team |]

    let private buildSessionId (config: SessionConfig) =
        sprintf "fsharp-%08x" (stableHash (buildSeed config))

    let private timestampFor (sequence: int) =
        DateTimeOffset.UnixEpoch.AddSeconds(float sequence).UtcDateTime.ToString("O")

    let private ansiLine ordinal accent message detail trace trailer =
        let reset = "\u001b[0m"
        let muted = "\u001b[2m"
        let seq = sprintf "%03d" ordinal
        sprintf "%s%s%s %s%s%s %s%s%s %s%s%s"
            muted seq reset
            accent message reset
            muted detail reset
            muted (if trace then "trace" else trailer) reset

    let private dictOf (pairs: (string * obj) list) =
        let value = Dictionary<string, obj>(StringComparer.Ordinal)
        pairs |> List.iter (fun (key, item) -> value[key] <- item)
        value :> IDictionary<string, obj>

    let private dedicatedConfigs =
        Map.ofList [
            "code_analyzer", { RendererKey = "classic-six.code_analyzer"; Accent = "\u001b[38;5;81m"; FocusKey = "analysisFocus"; FocusValue = "typed interfaces, agent-authored patches, and MCP assumptions"; RustPath = "src/generators/code_analyzer.rs"; JavaPath = "src/main/java/com/stakeholder/generators/CodeAnalyzerRenderer.java"; Phrases = [| "Typed interfaces, agent-authored patches, and MCP assumptions stayed explicit across the audit lane."; "Build-graph review findings stayed anchored to deterministic trace rows."; "Patch review output stayed concrete instead of collapsing into vague automation prose." |] }
            "data_processing", { RendererKey = "classic-six.data_processing"; Accent = "\u001b[38;5;45m"; FocusKey = "dataWindow"; FocusValue = "embeddings, semantic chunks, and batch transforms with deterministic ordering"; RustPath = "src/generators/data_processing.rs"; JavaPath = "src/main/java/com/stakeholder/generators/DataProcessingRenderer.java"; Phrases = [| "Dataset transforms stayed deterministic under seed control."; "Feature extraction remained traceable back to the input rows."; "Data pipeline handoffs stayed explicit in the trace." |] }
            "jargon", { RendererKey = "classic-six.jargon"; Accent = "\u001b[38;5;141m"; FocusKey = "languagePolicy"; FocusValue = "credible 2026 terminology instead of fake-deep phrasing"; RustPath = "src/generators/jargon.rs"; JavaPath = "src/main/java/com/stakeholder/generators/JargonRenderer.java"; Phrases = [| "Terminology drift was reduced to a readable glossary entry."; "Overheated wording stayed anchored to an explicit contract."; "Domain language remained precise across the renderer lane." |] }
            "metrics", { RendererKey = "classic-six.metrics"; Accent = "\u001b[38;5;87m"; FocusKey = "signalBlend"; FocusValue = "queue depth, token spend, and GPU occupancy in a single operations lane"; RustPath = "src/generators/metrics.rs"; JavaPath = "src/main/java/com/stakeholder/generators/MetricsRenderer.java"; Phrases = [| "Latency, throughput, and burn-rate values stayed visible."; "SLO context stayed attached to the generated line."; "The metrics lane stayed readable under seed control." |] }
            "network_activity", { RendererKey = "classic-six.network_activity"; Accent = "\u001b[38;5;214m"; FocusKey = "transportMix"; FocusValue = "RPC, event-stream, and adapter traffic under deterministic retry rules"; RustPath = "src/generators/network_activity.rs"; JavaPath = "src/main/java/com/stakeholder/generators/NetworkActivityRenderer.java"; Phrases = [| "Request flow stayed readable from client to endpoint."; "Transport edges stayed explicit in the generated trace."; "Network hops remained visible without losing the contract." |] }
            "system_monitoring", { RendererKey = "classic-six.system_monitoring"; Accent = "\u001b[38;5;82m"; FocusKey = "telemetryScope"; FocusValue = "collector pressure, runner health, and policy-denial signals across the stack"; RustPath = "src/generators/system_monitoring.rs"; JavaPath = "src/main/java/com/stakeholder/generators/SystemMonitoringRenderer.java"; Phrases = [| "Collector backpressure stayed visible in the output."; "Health checks remained attached to the terminal line."; "Monitoring noise stayed bounded by the renderer contract." |] }
            "agent_workflows", { RendererKey = "modern-core.agent_workflows"; Accent = "\u001b[38;5;213m"; FocusKey = "coordinationMode"; FocusValue = "delegated agent work, approval gates, and cross-repo handoff envelopes"; RustPath = "src/generators/agent_workflows.rs"; JavaPath = "src/main/java/com/stakeholder/generators/AgentWorkflowsRenderer.java"; Phrases = [| "Delegation planning, approval gates, and cross-repo handoff envelopes stayed readable from prompt to result."; "Delegated agent work remained explicit across queueing, approval, and retry steps."; "Agent workflow state kept delegation, approval, and handoff boundaries visible." |] }
            "platform_engineering", { RendererKey = "modern-core.platform_engineering"; Accent = "\u001b[38;5;117m"; FocusKey = "platformSurface"; FocusValue = "golden paths, identity boundaries, and queue ownership in the shared platform lane"; RustPath = "src/generators/platform_engineering.rs"; JavaPath = "src/main/java/com/stakeholder/generators/PlatformEngineeringRenderer.java"; Phrases = [| "Golden paths, identity federation, queue ownership, and paved-road rollouts stayed explicit."; "Platform contracts, tenancy edges, and queue ownership stayed aligned across the control plane."; "The shared platform lane kept ownership, queues, and defaults visible." |] }
            "observability_ai_runtime", { RendererKey = "modern-core.observability_ai_runtime"; Accent = "\u001b[38;5;111m"; FocusKey = "runtimeSignals"; FocusValue = "trace spans, token burn, GPU pressure, and policy denials in one runtime lane"; RustPath = "src/generators/observability_ai_runtime.rs"; JavaPath = "src/main/java/com/stakeholder/generators/ObservabilityAIRuntimeRenderer.java"; Phrases = [| "Inference spans, token burn, GPU saturation, and sandbox denials stayed correlated."; "Runtime traces, GPU pressure, and policy denials stayed visible in one lane."; "The runtime lane kept saturation, burn, and denials attached to the same trace." |] }
            "delivery_preview_ops", { RendererKey = "modern-core.delivery_preview_ops"; Accent = "\u001b[38;5;221m"; FocusKey = "deliveryGuardrail"; FocusValue = "preview deploys, canaries, release flags, and rollback checkpoints under seed control"; RustPath = "src/generators/delivery_preview_ops.rs"; JavaPath = "src/main/java/com/stakeholder/generators/DeliveryPreviewOpsRenderer.java"; Phrases = [| "Preview deploys, canary health, release flags, and rollback checkpoints stayed coordinated."; "Preview environments and rollback cues stayed visible before promotion."; "Release previews stayed explicit without hiding the rollback path." |] }
            "supply_chain_security", { RendererKey = "modern-core.supply_chain_security"; Accent = "\u001b[38;5;203m"; FocusKey = "supplyChainPosture"; FocusValue = "provenance, attestations, dependency drift, and secret exposure in one security lane"; RustPath = "src/generators/supply_chain_security.rs"; JavaPath = "src/main/java/com/stakeholder/generators/SupplyChainSecurityRenderer.java"; Phrases = [| "Attestations, dependency drift, key rotation, and registry trust signals stayed linked."; "Provenance, signing, and dependency drift stayed visible in one security lane."; "Dependency trust signals stayed readable without hiding provenance gaps." |] }
        ]

    let private dedicatedRenderer
        (family: GeneratorFamilyDefinition)
        (config: SessionConfig)
        ordinal
        (rendererConfig: DedicatedRendererConfig)
        : string * string * IDictionary<string, obj> =
        let message = rendererConfig.Phrases[(ordinal - 1) % rendererConfig.Phrases.Length] + " Traceability is anchored to Java, Rust, and stakeholder-core."
        let context =
            dictOf [
                "family", box family.Id
                "familyLabel", box family.Label
                "group", box (string family.Group)
                "renderer", box rendererConfig.RendererKey
                "devType", box config.DevType
                "smoke", box family.Smoke
                "seed", box config.Seed
                "trace", box config.Trace
                "alerts", box config.Alerts
                "team", box config.Team
                "ordinal", box ordinal
                "detail", box "dedicated first-push renderer"
                "familyFocusKey", box rendererConfig.FocusKey
                rendererConfig.FocusKey, box rendererConfig.FocusValue
                "traceabilitySourceRepo", box "rust-stakeholder"
                "traceabilitySourcePath", box rendererConfig.RustPath
                "traceabilityJavaRepo", box "java-stakeholder"
                "traceabilityJavaPath", box rendererConfig.JavaPath
                "traceabilityContractRepo", box "stakeholder-core"
                "traceabilityContractPath", box "docs/generator-families.md"
                "traceabilityParityClass", box "full-parity"
            ]
        message, ansiLine ordinal rendererConfig.Accent message family.Id config.Trace (string family.Group), context

    let private fallbackRenderer
        (family: GeneratorFamilyDefinition)
        (config: SessionConfig)
        ordinal
        : string * string * IDictionary<string, obj> =
        let message = sprintf "%s lane remains on grouped fallback while the F# push-bar tranche is still limited to classic-six and modern-core depth." family.Label
        let context =
            dictOf [
                "family", box family.Id
                "familyLabel", box family.Label
                "group", box (string family.Group)
                "renderer", box family.RendererKey
                "devType", box config.DevType
                "smoke", box family.Smoke
                "seed", box config.Seed
                "trace", box config.Trace
                "alerts", box config.Alerts
                "team", box config.Team
                "ordinal", box ordinal
                "detail", box "grouped fallback renderer"
                "summary", box family.Summary
            ]
        message, ansiLine ordinal "\u001b[38;5;177m" message family.Id config.Trace (string family.Group), context

    let private renderFamily
        (family: GeneratorFamilyDefinition)
        (config: SessionConfig)
        ordinal
        : string * string * IDictionary<string, obj> =
        match dedicatedConfigs.TryFind family.Id with
        | Some rendererConfig -> dedicatedRenderer family config ordinal rendererConfig
        | None -> fallbackRenderer family config ordinal

    let private addEvent (events: ResizeArray<SessionEvent>) eventType message context renderer terminal =
        let sequence = events.Count + 1
        let provenance =
            dictOf [
                "provider", box "local"
                "model", box "deterministic"
                "adapterMode", box "api"
                "promptVersion", box "first-push"
                "cache", box "n/a"
                "personalizationProfile", box "baseline"
                "renderer", box renderer
            ]
        events.Add(
            { EventType = eventType
              Sequence = sequence
              Message = message
              Timestamp = timestampFor sequence
              Context = context
              Provenance = provenance
              Terminal = terminal }
        )

    let private selectFamilies (config: SessionConfig) =
        match config.FocusFamily with
        | Some familyId -> [| Registry.requireFamily familyId |]
        | None when config.Minimal -> [| Registry.requireFamily "code_analyzer" |]
        | None -> [| Registry.requireFamily "code_analyzer"; Registry.requireFamily "agent_workflows" |]

    let buildListValues () : ListValuesPayload =
        { DevTypes = Registry.devTypes
          JargonLevels = Registry.jargonLevels
          Complexities = Registry.complexities
          OutputFormats = Registry.outputFormats
          GeneratorFamilies =
            Registry.all
            |> Array.map (fun (family: GeneratorFamilyDefinition) ->
                { Id = family.Id
                  Label = family.Label
                  Group = string family.Group
                  Summary = family.Summary
                  RendererKey = family.RendererKey
                  Renderer = family.RendererKey
                  Smoke = family.Smoke })
          ExperimentalProviders = [| "openai-compatible"; "anthropic"; "consumer-session" |]
          ExperimentalAdapterModes = [| "api"; "consumer" |] }

    let runSession (config: SessionConfig) : SessionResult =
        let selectedFamilies = selectFamilies config
        let sessionId = buildSessionId config
        let events = ResizeArray<SessionEvent>()

        addEvent events "session.start"
            (sprintf "session started for %s" config.DevType)
            (dictOf [ "sessionId", box sessionId; "devType", box config.DevType; "complexity", box config.Complexity; "jargon", box config.Jargon ])
            "baseline"
            (ansiLine 1 "\u001b[38;5;81m" (sprintf "session started for %s" config.DevType) "baseline" false "registry")

        addEvent events "session.plan"
            (sprintf "selected %d generator lanes" selectedFamilies.Length)
            (dictOf [ "families", box (selectedFamilies |> Array.map _.Id); "devType", box config.DevType ])
            "baseline"
            (ansiLine 2 "\u001b[38;5;45m" (sprintf "selected %d generator lanes" selectedFamilies.Length) "plan" false "registry")

        selectedFamilies
        |> Array.iteri (fun index family ->
            let ordinal = index + 3
            let message, terminal, context = renderFamily family config ordinal
            let renderer = context["renderer"] :?> string
            addEvent events "generator.activity" message context renderer terminal
            if config.Trace then
                addEvent events "generator.trace"
                    (sprintf "%s trace row" family.Id)
                    (dictOf [ "family", box family.Id; "group", box (string family.Group); "renderer", box renderer; "trace", box true ])
                    renderer
                    (ansiLine (events.Count + 1) "\u001b[38;5;111m" (sprintf "%s trace row" family.Id) family.Id true "trace"))

        addEvent events "session.end"
            "deterministic first-push candidate session complete"
            (dictOf [ "status", box "ok"; "durationEvents", box events.Count ])
            "baseline"
            (ansiLine (events.Count + 1) "\u001b[38;5;82m" "deterministic first-push candidate session complete" "end" false "registry")

        { SessionId = sessionId
          Mode = "static"
          Config =
            { DevType = config.DevType
              Complexity = config.Complexity
              Jargon = config.Jargon
              OutputFormat = config.OutputFormat
              Seed = config.Seed
              Project = config.Project
              Framework = config.Framework
              FocusFamily = defaultArg config.FocusFamily null
              Alerts = config.Alerts
              Team = config.Team
              Minimal = config.Minimal
              Trace = config.Trace }
          SelectedFamilies = selectedFamilies |> Array.map _.Id
          Events = events.ToArray() }

    let private printHelp () =
        Console.WriteLine("Usage: fsharp-stakeholder [options]")
        Console.WriteLine("  --list-values")
        Console.WriteLine("  --dev-type <backend|blockchain|data-science|dev-ops|frontend|fullstack|game-development|machine-learning|security|systems-programming>")
        Console.WriteLine("  --complexity <low|medium|high|extreme>")
        Console.WriteLine("  --jargon <low|normal|high|extreme>")
        Console.WriteLine("  --output-format <text|json>")
        Console.WriteLine("  --seed <value>")
        Console.WriteLine("  --focus-family <family-id>")
        Console.WriteLine("  --alerts")
        Console.WriteLine("  --team")
        Console.WriteLine("  --minimal")
        Console.WriteLine("  --trace")
        Console.WriteLine("  experimental provider flags are parsed but fail fast")

    let run (args: string array) =
        try
            let showHelp = hasFlag args "--help"
            let listValues = hasFlag args "--list-values"
            let experimental = parseExperimentalConfig args
            let sessionConfig = parseSessionConfig args

            if showHelp then
                printHelp ()
                0
            elif experimental.HasAnyFlag then
                raise (ExperimentalProviderNotImplementedException(buildExperimentalProviderMessage experimental))
            elif listValues then
                Console.WriteLine(JsonSerializer.Serialize(buildListValues (), SessionJson.options))
                0
            else
                let session = runSession sessionConfig
                if sessionConfig.OutputFormat = "json" then
                    Console.WriteLine(JsonSerializer.Serialize(session, SessionJson.options))
                else
                    let header = sprintf "%s \u001b[38;5;81m%s\u001b[0m \u001b[2m%s\u001b[0m" session.SessionId session.Mode session.Config.DevType
                    let body = session.Events |> Array.map _.Terminal |> String.concat Environment.NewLine
                    Console.WriteLine(header + Environment.NewLine + body)
                0
        with
        | ExperimentalProviderNotImplementedException message ->
            Console.Error.WriteLine(message)
            2
        | CommandLineException message ->
            Console.Error.WriteLine(message)
            2
        | ex ->
            Console.Error.WriteLine(ex.Message)
            2
