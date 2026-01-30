# Bolt's Journal

## 2024-05-22 - [Understanding the Environment]
**Learning:** The build environment for this project is currently broken due to missing local dependencies (`Benchwarp`, `MMHOOK`, `SilksongPath.props`).
**Action:** Reliance on static analysis and code patterns is crucial. Verification via `dotnet build` is not possible, so extra care with syntax and logic is required. I should look for obvious algorithmic inefficiencies or standard Unity/C# performance anti-patterns.

## 2024-05-22 - [Avoid String Lookups for Known Fields]
**Learning:** The codebase contained usage of `PlayerData.instance.GetInt(nameof(PlayerData.geo))`. Using `nameof` proves the field is known at compile time, yet the code used a slower string-lookup method. This is a performance anti-pattern.
**Action:** Always prefer direct field/property access when the member is accessible. Only use `GetBool`/`GetInt` with strings for dynamic access (e.g., variable names determined at runtime).
