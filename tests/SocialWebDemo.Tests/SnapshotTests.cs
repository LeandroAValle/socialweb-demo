using SocialWebDemo.Core;

namespace SocialWebDemo.Tests;

public class SnapshotTests
{
    [Fact]
    public void GeneratedSnapshotIsDeterministic()
    {
        var first = SnapshotFactory.Build();
        var second = SnapshotFactory.Build();
        Assert.Equal(first.Households, second.Households);
        Assert.Equal(first.Attendances, second.Attendances);
        Assert.Equal(first.Benefits, second.Benefits);
    }

    [Fact]
    public void TerritoryMetricsReconcileWithTotal()
    {
        var snapshot = SnapshotFactory.Build();
        var total = snapshot.Views[SnapshotFactory.AllTerritories].Metrics;
        var parts = SnapshotFactory.Territories.Select(t => snapshot.Views[t].Metrics).ToArray();
        Assert.Equal(total.Households, parts.Sum(p => p.Households));
        Assert.Equal(total.People, parts.Sum(p => p.People));
        Assert.Equal(total.OpenAttendances, parts.Sum(p => p.OpenAttendances));
        Assert.Equal(total.ActiveBenefits, parts.Sum(p => p.ActiveBenefits));
    }

    [Fact]
    public void OrphanAttendanceIsRejected()
    {
        Assert.Throws<InvalidOperationException>(() => SnapshotFactory.Validate(
            [new Household("FAM-0001", SnapshotFactory.Territories[0], 2, "Teste", true, "Cadastro ativo")],
            [new Attendance("ATD-0001", "FAM-X", SnapshotFactory.Months[0], "Orientação", "Em aberto")], []));
    }
}
