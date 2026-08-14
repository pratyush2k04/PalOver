# EF Core Migration Guidelines

These rules exist because of a real incident: a migration's `Up()` was hand-edited without
updating its `Down()`, which left it silently unable to roll back. It was traced, tested, and
fixed — these guidelines capture what to do differently going forward.

---

## Core rules (no exceptions)

- **Never hand-edit a generated migration's `Up()` or `Down()`.** If a migration needs to be
  different, fix the model and regenerate it.
- **Never hand-patch `AppDbContextModelSnapshot.cs`** to make it merge or compile.
- **Never leave a migration with an empty `Up()`** when you know the model changed. An empty
  migration means the snapshot was already stale before you generated it — stop and fix that
  first, don't commit the empty result.
- **Never let `Down()`'s data-restore step reference a column/property that `Up()` itself just
  removed.** If the column is going away, any data operation touching it — forward or back —
  must be raw SQL, not a typed data operation (see [Seeding data](#seeding-data) below).

---

## Before you generate a migration

1. Pull/rebase onto the latest target branch **first**. Never generate a migration against a
   stale snapshot.
2. If your branch has old, unmerged local migrations that predate the pull: revert them from
   your local DB, delete the migration files, then regenerate — **after** the pull, not before.

## When you hit a merge/snapshot conflict

The snapshot file is one large generated block and conflicts easily when two branches both add
migrations. When this happens:

1. Revert your migrations in your local DB.
2. Delete your local migration files (the ones not yet merged/shared).
3. Pull the latest from remote so the snapshot comes through clean, untouched by your changes.
4. Regenerate your migration fresh against that clean snapshot.

Do **not** try to resolve the conflict by hand-patching the snapshot so it merges or compiles —
that silently defeats EF's diffing and produces empty migrations further down the line, with no
error or warning.

## When you generate it

- Let `dotnet ef migrations add <Name>` produce it. Don't hand-write a migration as the first
  draft.
- Keep schema changes and data seeding in **separate migrations** — don't seed or update rows
  in the same migration that's also changing the shape of the table those rows live in.

## Before you commit it

- [ ] Open the generated file and sanity-check `Up()` isn't empty if you know the model changed.
- [ ] Run `dotnet ef migrations has-pending-model-changes` — should report no pending changes
      after adding your migration.
- [ ] **Test the round trip on a scratch/throwaway database:**
  ```
  dotnet ef database update                          # forward, from empty
  dotnet ef database update <previous-migration-name> # roll back one step
  dotnet ef database update                          # forward again
  ```
  All three must succeed cleanly. This single check is cheap (seconds) and is exactly what would
  have caught this bug immediately.

---

## Seeding data

Pick the mechanism based on what kind of data you're seeding — this is the part that caused the
original incident, so treat it as a decision tree, not a single rule.

### Stable reference/lookup data
(statuses, roles, categories — things unlikely to ever have their columns dropped)

- Use `.HasData(...)` in the entity's Fluent API configuration, **not** ad-hoc
  `migrationBuilder.InsertData()` calls inside a migration.
- Why: `HasData()` becomes part of the tracked model. When you later change that seed data, EF
  diffs the *old* `HasData()` against the *new* one and auto-generates a correct, matched
  `Up()`/`Down()` pair for you. You only get this safety if you keep using `HasData()` for
  further changes to that data — not one-off calls.

### One-off, demo, or business data
(or any seed touching a column that's being changed/dropped in this same change)

- Use raw SQL (`migrationBuilder.Sql(...)`) directly — never `InsertData`/`UpdateData`.
- Why: those typed operations re-resolve the column against a **live C# property every time
  they run**, forward or backward, forever — not just at the moment the migration was written.
  If that property is ever removed from the model, every past `InsertData`/`UpdateData` call
  referencing it becomes permanently broken to generate, even on a migration that's otherwise
  completely clean. Raw SQL has no such dependency.
- Put it in its own dedicated seed migration, separate from schema changes (see existing
  examples in this repo: `SeedPublishedVersionZero`, `Seeding all roles for Engagement 1`).

### Migrating data ahead of dropping a column
(e.g. "copy the value out of the old column before removing it")

- Sequence it explicitly within `Up()`: add the new column → raw SQL to copy/transform data
  from old → new → drop the old column.
- Don't write a `Down()` that pretends to restore the original data with a placeholder value.
  Dropping a column is inherently lossy — a fabricated stand-in value is misleading, not a real
  restore. Either accept that `Down()` can't fully reverse this step (say so in a comment), or
  don't provide a data-restoring `Down()` for that piece at all.

---

## Quick reference

| Situation | Do this |
|---|---|
| Starting a new migration | Pull latest first, then `dotnet ef migrations add` |
| Local migration conflicts with snapshot after pulling | Revert DB, delete your files, pull, regenerate |
| Snapshot won't merge cleanly | Never hand-patch it — see conflict steps above |
| Migration needs to change after generation | Fix the model, regenerate — never hand-edit the file |
| Seeding static reference data | `.HasData()` in entity configuration |
| Seeding one-off/demo data | Raw SQL, in its own migration |
| Data on a column you're dropping | Raw SQL only, sequenced before the drop, honest `Down()` |
| Before committing any migration | Round-trip test: forward → back → forward, on a scratch DB |
