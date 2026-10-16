namespace SocialWebDemo.Core;

public static class SnapshotFactory
{
    public const string AllTerritories = "Todos os territórios";
    public static readonly string[] Territories = ["Território Aurora", "Território Horizonte", "Território Vale"];
    public static readonly string[] Months = ["2026-04", "2026-05", "2026-06", "2026-07", "2026-08", "2026-09"];

    public static Snapshot Build()
    {
        var random = new Random(202610);
        var households = new List<Household>();
        var attendances = new List<Attendance>();
        var benefits = new List<BenefitGrant>();
        string[] incomeBands = ["Até 1 salário mínimo", "De 1 a 2 salários mínimos", "Acima de 2 salários mínimos"];
        string[] kinds = ["Orientação", "Acompanhamento", "Encaminhamento"];
        string[] benefitTypes = ["Auxílio eventual", "Apoio alimentar", "Transporte"];

        for (var number = 1; number <= 180; number++)
        {
            var id = $"FAM-{number:0000}";
            var territory = Territories[(number - 1) % Territories.Length];
            var complete = random.NextDouble() < 0.88;
            households.Add(new Household(
                id,
                territory,
                random.Next(1, 7),
                incomeBands[random.Next(incomeBands.Length)],
                complete,
                random.NextDouble() < 0.25 ? "Em acompanhamento" : "Cadastro ativo"));

            var attendanceCount = random.Next(0, 4);
            for (var item = 0; item < attendanceCount; item++)
            {
                attendances.Add(new Attendance(
                    $"ATD-{attendances.Count + 1:0000}",
                    id,
                    Months[random.Next(Months.Length)],
                    kinds[random.Next(kinds.Length)],
                    random.NextDouble() < 0.24 ? "Em aberto" : "Concluído"));
            }

            if (random.NextDouble() < 0.36)
            {
                benefits.Add(new BenefitGrant(
                    $"BEN-{benefits.Count + 1:0000}",
                    id,
                    benefitTypes[random.Next(benefitTypes.Length)],
                    random.NextDouble() < 0.7 ? "Ativo" : "Concluído"));
            }
        }

        Validate(households, attendances, benefits);
        var views = new Dictionary<string, TerritoryView>();
        foreach (var territory in new[] { AllTerritories }.Concat(Territories))
        {
            var selected = territory == AllTerritories
                ? households
                : households.Where(h => h.Territory == territory).ToList();
            var ids = selected.Select(h => h.Id).ToHashSet();
            var selectedAttendances = attendances.Where(a => ids.Contains(a.HouseholdId)).ToList();
            var selectedBenefits = benefits.Where(b => ids.Contains(b.HouseholdId)).ToList();
            var incomplete = selected.Count(h => !h.RegistrationComplete);
            views.Add(territory, new TerritoryView(
                territory,
                new MetricSet(
                    selected.Count,
                    selected.Sum(h => h.Members),
                    selectedAttendances.Count(a => a.Status == "Em aberto"),
                    selectedBenefits.Count(b => b.Status == "Ativo"),
                    incomplete,
                    Math.Round((selected.Count - incomplete) * 100m / selected.Count, 1)),
                Months.Select(month => new MonthlyActivity(month, selectedAttendances.Count(a => a.Month == month))).ToArray()));
        }

        return new Snapshot(1, "2026-09", "synthetic", "illustrative", [AllTerritories, .. Territories], households, attendances, benefits, views);
    }

    public static void Validate(
        IReadOnlyCollection<Household> households,
        IReadOnlyCollection<Attendance> attendances,
        IReadOnlyCollection<BenefitGrant> benefits)
    {
        if (households.Count == 0 || households.Select(h => h.Id).Distinct().Count() != households.Count)
            throw new InvalidOperationException("Famílias vazias ou com identificadores duplicados.");
        if (households.Any(h => !Territories.Contains(h.Territory) || h.Members < 1))
            throw new InvalidOperationException("Família com território ou tamanho inválido.");
        var ids = households.Select(h => h.Id).ToHashSet();
        if (attendances.Any(a => !ids.Contains(a.HouseholdId) || !Months.Contains(a.Month)) ||
            benefits.Any(b => !ids.Contains(b.HouseholdId)))
            throw new InvalidOperationException("Atendimento ou benefício sem família válida.");
    }
}
