# Integer Data Structures

Implementations of collections of integer keys and generic values.

They need to support at least insert and lookup operations.

## Van Emde Boas tree

It is a tree data structure which support the following operations:

| Operation  | Time Complexity |
|---|---|
| Insert | O(log log M) |
| Lookup | O(log log M) |
| Delete | O(log log M) |
| Maximum Key | O(log log M) |
| Minimum Key | O(log log M) |
| Next Key | O(log log M) |
| Previous Key | O(log log M) |

### ProtoVanEmdeBoasTree

Is a simplefied implementation of Van Emde Boas tree with worst guarantees of complexity:

| Operation  | Time Complexity |
|---|---|
| Insert | O(log M) |
| Lookup | O(log log M) |
| Delete | O(log M) |
| Maximum Key | O(log M) |
| Minimum Key | O(log M) |
| Next Key | O(log M log log M) |
| Previous Key | O(log M log log M) |
