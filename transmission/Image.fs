namespace synthings.transmission

type Image =
    | DotNetBot
    | Local of string

module Image =
    let blank size color =
        let width, height = size
        let red, green, blue = color
        let backgroundColor = SixLabors.ImageSharp.PixelFormats.Rgb24((byte)red, (byte)green, (byte)blue)
        new SixLabors.ImageSharp.Image<SixLabors.ImageSharp.PixelFormats.Rgb24>(SixLabors.ImageSharp.Configuration.Default, width, height, backgroundColor)

    let loadLocal (filePath : string) =
        if System.IO.File.Exists filePath then
            try
                Ok(SixLabors.ImageSharp.Image.Load<SixLabors.ImageSharp.PixelFormats.Rgb24>(filePath))
            with
                | imageLoadingException -> Error(imageLoadingException.Message)
        else Error("File not found.")
    
    let toStream (image : SixLabors.ImageSharp.Image) : System.IO.Stream =
        let stream = new System.IO.MemoryStream()
        SixLabors.ImageSharp.ImageExtensions.SaveAsJpeg(image, stream)
        stream.Seek(0L, System.IO.SeekOrigin.Begin) |> ignore
        stream
    
    let toByteArray (image : SixLabors.ImageSharp.Image) =
        use stream = toStream image
        let bytes = Array.zeroCreate<byte> (int stream.Length)
        stream.Read(bytes, 0, int stream.Length) |> ignore
        bytes