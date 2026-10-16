using System.Text.Json;
using SocialWebDemo.Core;

var snapshot = SnapshotFactory.Build();
var jsonOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web) { WriteIndented = true };
if (args.Contains("--export-snapshot"))
{
    var path = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "../../../../../site/data/snapshot.json"));
    Directory.CreateDirectory(Path.GetDirectoryName(path)!);
    File.WriteAllText(path, JsonSerializer.Serialize(snapshot, jsonOptions) + "\n");
    Console.WriteLine($"Snapshot sintético exportado: {path}");
    return;
}

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();
app.MapGet("/health", () => Results.Ok(new { status = "ok", source = "synthetic" }));
app.MapGet("/api/v1/context", () => Results.Ok(new { snapshot.SchemaVersion, snapshot.Period, snapshot.Source, snapshot.RuleStatus, snapshot.Territories }));
app.MapGet("/api/v1/overview", (string? territory) =>
    snapshot.Views.TryGetValue(territory ?? SnapshotFactory.AllTerritories, out var view)
        ? Results.Ok(view)
        : Results.BadRequest(new { error = "Território desconhecido." }));
app.MapGet("/api/v1/households", (string? territory) =>
    territory is null || territory == SnapshotFactory.AllTerritories
        ? Results.Ok(snapshot.Households)
        : SnapshotFactory.Territories.Contains(territory)
            ? Results.Ok(snapshot.Households.Where(h => h.Territory == territory))
            : Results.BadRequest(new { error = "Território desconhecido." }));
app.MapGet("/api/v1/households/{id}", (string id) =>
{
    var household = snapshot.Households.FirstOrDefault(h => h.Id.Equals(id, StringComparison.OrdinalIgnoreCase));
    return household is null
        ? Results.NotFound(new { error = "Família não encontrada." })
        : Results.Ok(new { household,
            attendances = snapshot.Attendances.Where(a => a.HouseholdId == household.Id),
            benefits = snapshot.Benefits.Where(b => b.HouseholdId == household.Id) });
});
app.Run();
