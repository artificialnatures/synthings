namespace synthings.transmission

type StateRepresentation =
    | ApplicationContainer
    | Cursor
    | Window of Window
    | VerticalStack
    | Canvas
    | Transform of (float * float) * StateRepresentation
    | Text of string
    | Button of string * Proposal<StateRepresentation>
    | Image of Image
    | File of File
    | FilePicker of string list
    | Wait
and Window =
    { title: string }