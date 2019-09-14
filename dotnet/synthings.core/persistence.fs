namespace synthings.core

module persistence =
    open System
    
    type ParameterDefinition =
        {
            Name : string;
            Type : Type;
        }
    
    type BehaviorDefinition =
        {
            Name : string;
            Parameters : ParameterDefinition list
        }
    
    type MachineDefinition =
        {
            Name : string;
            Id : Guid;
            Behaviors : BehaviorDefinition list
        }
    
    type ConnectionDefinition =
        {
            Id : Guid;
            Upstream : Guid;
            Downstream : Guid
        }
    
    type GraphDefinition =
        {
            Id : Guid;
            Name : string;
            Machines : MachineDefinition list;
            Connections : ConnectionDefinition list
        }
