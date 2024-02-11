namespace synthings.transmission

type Point =
    { x: float
      y: float
      z: float }

    static member two(x, y) = { x = x; y = y; z = 0.0 }
    static member three(x, y, z) = { x = x; y = y; z = z }