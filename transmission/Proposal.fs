namespace synthings.transmission

type InitializeProposal<'entity> =
    {
        initialTree : Tree<'entity>
    }

type AddProposal<'entity> =
    {
        parent : EntityReference
        order : InsertionOrder
        entityToAdd : Tree<'entity>
    }

type ReplaceProposal<'entity> =
    {
        entityToReplace : EntityReference
        replacement : Tree<'entity>
    }

type RemoveProposal =
    {
        entityToRemove : EntityReference
    }

type Proposal<'entity> =
    | Initialize of InitializeProposal<'entity>
    | Add of AddProposal<'entity>
    | Replace of ReplaceProposal<'entity>
    | Remove of RemoveProposal
