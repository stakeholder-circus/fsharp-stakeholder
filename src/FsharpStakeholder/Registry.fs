namespace FsharpStakeholder

open System
open System.Collections.Generic

module Registry =
    let private allFamilies =
        [|
            { Id = "code_analyzer"; Label = "code_analyzer"; Group = ClassicSix; Summary = "code review, build graph, SDK drift"; RendererKey = "classic-six.code_analyzer"; Smoke = true }
            { Id = "data_processing"; Label = "data_processing"; Group = ClassicSix; Summary = "fixtures, pipelines, transforms"; RendererKey = "classic-six.data_processing"; Smoke = true }
            { Id = "jargon"; Label = "jargon"; Group = ClassicSix; Summary = "credible domain language"; RendererKey = "classic-six.jargon"; Smoke = true }
            { Id = "metrics"; Label = "metrics"; Group = ClassicSix; Summary = "token cost, burn, queue depth"; RendererKey = "classic-six.metrics"; Smoke = true }
            { Id = "network_activity"; Label = "network_activity"; Group = ClassicSix; Summary = "API, SSE, and transport events"; RendererKey = "classic-six.network_activity"; Smoke = true }
            { Id = "system_monitoring"; Label = "system_monitoring"; Group = ClassicSix; Summary = "health, backpressure, saturation"; RendererKey = "classic-six.system_monitoring"; Smoke = true }
            { Id = "agent_workflows"; Label = "agent_workflows"; Group = ModernCore; Summary = "delegation, retries, approvals"; RendererKey = "modern-core.agent_workflows"; Smoke = true }
            { Id = "platform_engineering"; Label = "platform_engineering"; Group = ModernCore; Summary = "golden paths, identity, queues"; RendererKey = "modern-core.platform_engineering"; Smoke = true }
            { Id = "observability_ai_runtime"; Label = "observability_ai_runtime"; Group = ModernCore; Summary = "tracing, burn rate, GPU pressure"; RendererKey = "modern-core.observability_ai_runtime"; Smoke = true }
            { Id = "delivery_preview_ops"; Label = "delivery_preview_ops"; Group = ModernCore; Summary = "preview deploys, canaries, flags"; RendererKey = "modern-core.delivery_preview_ops"; Smoke = true }
            { Id = "supply_chain_security"; Label = "supply_chain_security"; Group = ModernCore; Summary = "provenance, attestations, secrets"; RendererKey = "modern-core.supply_chain_security"; Smoke = true }
            { Id = "ai_inference_ops"; Label = "ai_inference_ops"; Group = AiGovernance; Summary = "model routing, fallback, cache"; RendererKey = "ai-governance.fallback"; Smoke = false }
            { Id = "knowledge_retrieval"; Label = "knowledge_retrieval"; Group = AiGovernance; Summary = "stale embeddings, recall, citations"; RendererKey = "ai-governance.fallback"; Smoke = false }
            { Id = "evaluation_and_guardrails"; Label = "evaluation_and_guardrails"; Group = AiGovernance; Summary = "eval drift, guardrail failures"; RendererKey = "ai-governance.fallback"; Smoke = false }
            { Id = "aibom_provenance"; Label = "aibom_provenance"; Group = AiGovernance; Summary = "model lineage and AI bills of materials"; RendererKey = "ai-governance.fallback"; Smoke = false }
            { Id = "data_governance_compliance"; Label = "data_governance_compliance"; Group = AiGovernance; Summary = "consent, retention, audit"; RendererKey = "ai-governance.fallback"; Smoke = false }
            { Id = "finops_capacity"; Label = "finops_capacity"; Group = AiGovernance; Summary = "budget, quota, resource burn"; RendererKey = "ai-governance.fallback"; Smoke = false }
            { Id = "identity_and_trust"; Label = "identity_and_trust"; Group = SecurityBlockchain; Summary = "keys, delegation, trust boundaries"; RendererKey = "security-blockchain.fallback"; Smoke = false }
            { Id = "agent_boundary_security"; Label = "agent_boundary_security"; Group = SecurityBlockchain; Summary = "tool, prompt, and auth boundaries"; RendererKey = "security-blockchain.fallback"; Smoke = false }
            { Id = "blockchain_protocol_ops"; Label = "blockchain_protocol_ops"; Group = SecurityBlockchain; Summary = "rollups, validators, account abstraction"; RendererKey = "security-blockchain.fallback"; Smoke = false }
            { Id = "cross_chain_interop"; Label = "cross_chain_interop"; Group = SecurityBlockchain; Summary = "chain abstraction and transfers"; RendererKey = "security-blockchain.fallback"; Smoke = false }
            { Id = "proof_and_sequencer_ops"; Label = "proof_and_sequencer_ops"; Group = SecurityBlockchain; Summary = "proof queues, ordering, MEV"; RendererKey = "security-blockchain.fallback"; Smoke = false }
            { Id = "fhir_profile_generator"; Label = "fhir_profile_generator"; Group = HealthProtocol; Summary = "FHIR resource generation"; RendererKey = "health-protocol.fallback"; Smoke = false }
            { Id = "smart_launch_oauth"; Label = "smart_launch_oauth"; Group = HealthProtocol; Summary = "SMART launch and OAuth context"; RendererKey = "health-protocol.fallback"; Smoke = false }
            { Id = "bulk_fhir_population_ops"; Label = "bulk_fhir_population_ops"; Group = HealthProtocol; Summary = "bulk export and analytics"; RendererKey = "health-protocol.fallback"; Smoke = false }
            { Id = "hl7v2_feed_ops"; Label = "hl7v2_feed_ops"; Group = HealthProtocol; Summary = "ADT/ORU feed handling"; RendererKey = "health-protocol.fallback"; Smoke = false }
            { Id = "clinical_workflow_events"; Label = "clinical_workflow_events"; Group = HealthProtocol; Summary = "hooks, subscriptions, workflow events"; RendererKey = "health-protocol.fallback"; Smoke = false }
            { Id = "dicomweb_imaging_ops"; Label = "dicomweb_imaging_ops"; Group = HealthProtocol; Summary = "QIDO/WADO/STOW imaging flows"; RendererKey = "health-protocol.fallback"; Smoke = false }
            { Id = "openehr_semantic_record_ops"; Label = "openehr_semantic_record_ops"; Group = HealthProtocol; Summary = "archetypes, templates, AQL"; RendererKey = "health-protocol.fallback"; Smoke = false }
            { Id = "device_telemetry_clinical"; Label = "device_telemetry_clinical"; Group = HealthProtocol; Summary = "bedside telemetry and alerts"; RendererKey = "health-protocol.fallback"; Smoke = false }
            { Id = "emr_vendor_adapter"; Label = "emr_vendor_adapter"; Group = HealthProtocol; Summary = "EMR vendor adapter flows"; RendererKey = "health-protocol.fallback"; Smoke = false }
            { Id = "ocpp_chargepoint_ops"; Label = "ocpp_chargepoint_ops"; Group = HealthProtocol; Summary = "OCPP 1.6 and 2.x chargepoint ops"; RendererKey = "health-protocol.fallback"; Smoke = false }
            { Id = "ocpi_roaming_ops"; Label = "ocpi_roaming_ops"; Group = HealthProtocol; Summary = "roaming, sessions, tariffs"; RendererKey = "health-protocol.fallback"; Smoke = false }
            { Id = "mcp_a2a_ops"; Label = "mcp_a2a_ops"; Group = HealthProtocol; Summary = "MCP and A2A tool calls"; RendererKey = "health-protocol.fallback"; Smoke = false }
            { Id = "streaming_bus_ops"; Label = "streaming_bus_ops"; Group = HealthProtocol; Summary = "Kafka, NATS, MQTT, event buses"; RendererKey = "health-protocol.fallback"; Smoke = false }
            { Id = "service_mesh_rpc_ops"; Label = "service_mesh_rpc_ops"; Group = HealthProtocol; Summary = "gRPC and GraphQL federation"; RendererKey = "health-protocol.fallback"; Smoke = false }
            { Id = "edge_client_runtime"; Label = "edge_client_runtime"; Group = HealthProtocol; Summary = "edge UI, hydration, offline sync"; RendererKey = "health-protocol.fallback"; Smoke = false }
            { Id = "embedded_agentic_pipeline"; Label = "embedded_agentic_pipeline"; Group = HealthProtocol; Summary = "deterministic control loops"; RendererKey = "health-protocol.fallback"; Smoke = false }
            { Id = "multilingual_security_packs"; Label = "multilingual_security_packs"; Group = OverlayQuantum; Summary = "localized security/operator tone"; RendererKey = "overlay-quantum.fallback"; Smoke = false }
            { Id = "security_persona_packs"; Label = "security_persona_packs"; Group = OverlayQuantum; Summary = "SOC, CTI, reverse-engineering personas"; RendererKey = "overlay-quantum.fallback"; Smoke = false }
            { Id = "hybrid_runtime_ops"; Label = "hybrid_runtime_ops"; Group = OverlayQuantum; Summary = "quantum jobs, sessions, batches"; RendererKey = "overlay-quantum.fallback"; Smoke = false }
            { Id = "capacity_cost_controller"; Label = "capacity_cost_controller"; Group = OverlayQuantum; Summary = "queues, reservations, spend controls"; RendererKey = "overlay-quantum.fallback"; Smoke = false }
            { Id = "batch_execution_tuner"; Label = "batch_execution_tuner"; Group = OverlayQuantum; Summary = "batch throughput and benchmarks"; RendererKey = "overlay-quantum.fallback"; Smoke = false }
            { Id = "compiler_maintainer"; Label = "compiler_maintainer"; Group = OverlayQuantum; Summary = "transpiler and plugin maintenance"; RendererKey = "overlay-quantum.fallback"; Smoke = false }
            { Id = "interop_adapter_engineer"; Label = "interop_adapter_engineer"; Group = OverlayQuantum; Summary = "OpenQASM and QIR adaptation"; RendererKey = "overlay-quantum.fallback"; Smoke = false }
            { Id = "preflight_capacity_planner"; Label = "preflight_capacity_planner"; Group = OverlayQuantum; Summary = "resource estimation and gating"; RendererKey = "overlay-quantum.fallback"; Smoke = false }
            { Id = "simulator_performance_engineer"; Label = "simulator_performance_engineer"; Group = OverlayQuantum; Summary = "simulators, GPU, local mode"; RendererKey = "overlay-quantum.fallback"; Smoke = false }
        |]

    let private byId =
        let value = Dictionary<string, GeneratorFamilyDefinition>(StringComparer.OrdinalIgnoreCase)
        allFamilies |> Array.iter (fun family -> value[family.Id] <- family)
        value

    let all = allFamilies
    let devTypes = [| "backend"; "blockchain"; "data-science"; "dev-ops"; "frontend"; "fullstack"; "game-development"; "machine-learning"; "security"; "systems-programming" |]
    let jargonLevels = [| "low"; "normal"; "high"; "extreme" |]
    let complexities = [| "low"; "medium"; "high"; "extreme" |]
    let outputFormats = [| "text"; "json" |]

    let requireFamily familyId =
        match byId.TryGetValue familyId with
        | true, family -> family
        | _ -> raise (CommandLineException(sprintf "Unknown family '%s'." familyId))
