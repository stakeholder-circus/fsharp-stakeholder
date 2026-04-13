namespace FsharpStakeholder

open System.Collections.Generic
open System.Text.Json
open System.Text.Json.Serialization

type FamilyGroup =
    | ClassicSix
    | ModernCore
    | AiGovernance
    | SecurityBlockchain
    | HealthProtocol
    | OverlayQuantum

type GeneratorFamilyDefinition =
    { Id: string
      Label: string
      Group: FamilyGroup
      Summary: string
      RendererKey: string
      Smoke: bool }

type SessionConfig =
    { DevType: string
      Complexity: string
      Jargon: string
      OutputFormat: string
      Seed: string
      Project: string
      Framework: string
      FocusFamily: string option
      Alerts: bool
      Team: bool
      Minimal: bool
      Trace: bool }

type ExperimentalConfig =
    { Provider: string option
      Model: string option
      Profile: string option
      Prompt: string option
      AdapterMode: string
      HasAnyFlag: bool }

[<CLIMutable>]
type GeneratorFamilyListItem =
    { Id: string
      Label: string
      Group: string
      Summary: string
      RendererKey: string
      Renderer: string
      Smoke: bool }

[<CLIMutable>]
type ListValuesPayload =
    { DevTypes: string array
      JargonLevels: string array
      Complexities: string array
      OutputFormats: string array
      GeneratorFamilies: GeneratorFamilyListItem array
      ExperimentalProviders: string array
      ExperimentalAdapterModes: string array }

[<CLIMutable>]
type SessionConfigDto =
    { DevType: string
      Complexity: string
      Jargon: string
      OutputFormat: string
      Seed: string
      Project: string
      Framework: string
      FocusFamily: string
      Alerts: bool
      Team: bool
      Minimal: bool
      Trace: bool }

[<CLIMutable>]
type SessionEvent =
    { EventType: string
      Sequence: int
      Message: string
      Timestamp: string
      Context: IDictionary<string, obj>
      Provenance: IDictionary<string, obj>
      Terminal: string }

[<CLIMutable>]
type SessionResult =
    { SessionId: string
      Mode: string
      Config: SessionConfigDto
      SelectedFamilies: string array
      Events: SessionEvent array }

exception CommandLineException of string
exception ExperimentalProviderNotImplementedException of string

module SessionJson =
    let options =
        let value = JsonSerializerOptions(PropertyNamingPolicy = JsonNamingPolicy.CamelCase, WriteIndented = true)
        value.DefaultIgnoreCondition <- JsonIgnoreCondition.WhenWritingNull
        value
