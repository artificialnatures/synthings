# synthings

Research into simple, flexible, and robust systems for interactive graphical programming.

- [ ] Need a mechanism to refer to entities before they have been instantiated
    - call identify before adding events to renderables
    - a "resolve" function in addition to submitProposal that resolves references like Parent or Sibling - would need unresolved and resolved Proposal types
    - an extra "sender" parameter in the submitProposal function that passes the sender Identifier to allow resolution
- [ ] Replace proposal -> Replace (entire subtree) and Refresh (just one entity)
- [ ] Test coverage
- [ ] Get console sandbox app working
- [ ] Get MAUI sandbox app working
- [ ] Remove all uneccessary failwith statements
- [ ] Refactoring
- [ ] GUI Testing

| Proposal | Action | Data |
|---|---|---|
| Initialize | Delete state and create new entity as root | initialTree |
| Add | Create new entity and insert into siblings list | tree, parent, order |
| Refresh | Update one entity (value) only | entityId, entity, replacementEntity |
| Replace | Delete existing entity (and all its children) and create new entity with same Identifier | entityId, entity, replacementTree |
| Remove | Delete existing entity | entityId, entity |

| Operation | Action | Data |
|---|---|---|
| Create | Creates a new entity | entityId, entity |
| Parent | Assigns child to parent | childId, parentId |
| Reorder | Moves an entity relative to its siblings | entityId, parentId, order, priorOrder |
| Update | Replaces an entity with a new value, leaving relations intact | entityId, entity, priorEntity |
| Orphan | Removes an entity from the relations table | entityId, parentId |
| Delete | Deletes an entity | entityId, entityToRestore |
