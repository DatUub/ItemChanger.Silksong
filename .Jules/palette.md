## 2024-05-22 - Handling Unimplemented UI Costs
**Learning:** Found `NotImplementedException` in UI-related cost logic (`PDBoolCost`). This not only breaks functionality but crashes the game.
**Action:** Always check for `NotImplementedException` in UI/Display methods. A simple fallback string ("Requires {0}") is better than a crash.
