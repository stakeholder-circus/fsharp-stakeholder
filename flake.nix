{
  description = "stakeholder-circus fsharp-stakeholder foundation";

  inputs.nixpkgs.url = "github:NixOS/nixpkgs/nixos-unstable";

  outputs = { self, nixpkgs }:
    let
      systems = [ "x86_64-linux" "aarch64-darwin" "x86_64-darwin" ];
      forAllSystems = nixpkgs.lib.genAttrs systems;
    in {
      devShells = forAllSystems (system:
        let pkgs = import nixpkgs { inherit system; };
        in {
          default = pkgs.mkShell {
            packages = with pkgs; [ git jq python312 dotnet-sdk_8 ];
          };
        });
      apps = forAllSystems (system:
        let pkgs = import nixpkgs { inherit system; };
            mk = name: text: {
              type = "app";
              program = "${pkgs.writeShellScript name text}";
            };
        in {
          build = mk "build" ''
            python3 scripts/validate_scaffold.py
            dotnet build src/FsharpStakeholder/FsharpStakeholder.fsproj
            dotnet build tests/FsharpStakeholder.Tests/FsharpStakeholder.Tests.fsproj
          '';
          test = mk "test" ''
            dotnet test tests/FsharpStakeholder.Tests/FsharpStakeholder.Tests.fsproj
          '';
          check = mk "check" ''
            python3 scripts/validate_scaffold.py
            dotnet build src/FsharpStakeholder/FsharpStakeholder.fsproj
            dotnet build tests/FsharpStakeholder.Tests/FsharpStakeholder.Tests.fsproj
            dotnet test tests/FsharpStakeholder.Tests/FsharpStakeholder.Tests.fsproj
          '';
          format = mk "format" ''
            dotnet format src/FsharpStakeholder/FsharpStakeholder.fsproj
            dotnet format tests/FsharpStakeholder.Tests/FsharpStakeholder.Tests.fsproj
          '';
        });
    };
}
