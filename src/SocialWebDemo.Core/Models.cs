namespace SocialWebDemo.Core;

public sealed record Household(string Id, string Territory, int Members, string IncomeBand, bool RegistrationComplete, string FollowUpStatus);
public sealed record Attendance(string Id, string HouseholdId, string Month, string Kind, string Status);
public sealed record BenefitGrant(string Id, string HouseholdId, string Type, string Status);
public sealed record MetricSet(int Households, int People, int OpenAttendances, int ActiveBenefits, int IncompleteRegistrations, decimal RegistrationQuality);
public sealed record MonthlyActivity(string Month, int Attendances);
public sealed record TerritoryView(string Territory, MetricSet Metrics, IReadOnlyList<MonthlyActivity> Activity);
public sealed record Snapshot(
    int SchemaVersion,
    string Period,
    string Source,
    string RuleStatus,
    IReadOnlyList<string> Territories,
    IReadOnlyList<Household> Households,
    IReadOnlyList<Attendance> Attendances,
    IReadOnlyList<BenefitGrant> Benefits,
    IReadOnlyDictionary<string, TerritoryView> Views);
