namespace synthings.scalar

module curve =
    let linear (rise : float) (run : float) (intercept : float) (x : float) = (rise / run) * x + intercept
