public record CalculationRequest(
    string Model,
    double Lambda,
    double Mu,
    string Unit,
    double? Variance,
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
    string Unit
);
