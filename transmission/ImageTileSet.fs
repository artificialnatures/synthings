namespace synthings.transmission

type PixelBoundary = {
    Top: int
    Left: int
    Bottom: int
    Right: int
}

module ImageTileSet =
    open SixLabors.ImageSharp

    type TilingMethod =
        | Grid
        | OverlappingCentered
        | OverlappingColumns
        | OverlappingRows

    let endPoint start tileDimension imageDimension =
        if start = 0
        then imageDimension - tileDimension
        else imageDimension - (tileDimension + tileDimension / 2)
    
    let createTileGrid x y tileDimension imageWidth imageHeight =
        let columns = [x..tileDimension..endPoint x tileDimension imageWidth]
        let rows = [y..tileDimension..endPoint y tileDimension imageHeight]
        List.map (fun y -> List.map (fun x -> {Top=y; Left=x; Bottom=y + tileDimension; Right=x + tileDimension}) columns) rows
        |> List.concat
    
    let create tileDimension tileMethod (image : Image) =
        let half = tileDimension / 2
        let gridTiles = createTileGrid 0 0 tileDimension image.Width image.Height
        match tileMethod with
        | Grid -> gridTiles
        | OverlappingCentered ->
            createTileGrid half half tileDimension image.Width image.Height
            |> List.append gridTiles
        | OverlappingColumns ->
            createTileGrid half 0 tileDimension image.Width image.Height
            |> List.append gridTiles
        | OverlappingRows ->
            createTileGrid 0 half tileDimension image.Width image.Height
            |> List.append gridTiles
 