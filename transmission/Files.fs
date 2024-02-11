namespace synthings.transmission

//TODO: add file type (JPG, PNG, PDF, etc.)
type File =
    { filePath: string
      sizeInBytes: int
      base64Encoded: string
    }

module File =
    let loadFile (filePath: string) =
        async {
            if System.IO.File.Exists(filePath) then
                let bytes = System.IO.File.ReadAllBytes(filePath)

                return
                    { filePath = filePath
                      sizeInBytes = Array.length bytes
                      base64Encoded = System.Convert.ToBase64String(bytes) }
            else
                return
                    { filePath = filePath
                      sizeInBytes = 0
                      base64Encoded = "" }
        }

    let loadFiles (filePaths: string list) =
        Seq.map loadFile filePaths
        |> Async.Parallel
        |> Async.RunSynchronously
        |> Array.toList

module Upload =
    open System.IO

    let upload uploadFilePath =
        if File.Exists(uploadFilePath) then
            Ok(Image.loadLocal uploadFilePath)
        else
            Error("No file exists at that path.")
