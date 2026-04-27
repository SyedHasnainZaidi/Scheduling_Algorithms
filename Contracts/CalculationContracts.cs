public record CalculationRequest(
    string Model,
    double Lambda,
    double Mu,
    string Unit,
    double? Variance,
    double? MinServiceTime,
    double? MaxServiceTime,
    string? Mg1Distribution,
    double? Mg1MeanServiceTime,
    double? Mg1Variance,
    double? Mg1MinServiceTime,
    double? Mg1MaxServiceTime,
    double? Ca,
    double? Cs,
    int? Servers,
    double? Cs2
);

public record CalculationResponse(
    string Model,
    double Rho,
    double? P0,
    double L,
    double Lq,
    double W,
    double Wq,
    double? MeanServiceTime,
    double? Variance,
    string Unit
);
