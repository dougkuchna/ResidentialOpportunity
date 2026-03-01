# Agent Rules — ResidentialOpportunity

The agent is allowed to run any and all commands to complete tasks without asking for permission. Document everything, and when finished commit and push. Do not progress beyond the current task scope without further instructions.

## Blazor Components

Use the code-behind pattern for all Blazor components. Place C# logic in `ComponentName.razor.cs` (partial class) rather than inline `@code` blocks. Move `@inject` directives to `[Inject]` properties and `@using` directives to `using` statements in the code-behind file.
