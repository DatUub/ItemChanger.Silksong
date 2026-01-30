## 2024-05-22 - Negative Cost Logic Vulnerability
**Vulnerability:** Game economy logic (RosaryCost) allowed negative costs, which could lead to items being free or potentially giving currency if exploited.
**Learning:** Even in simple game mods, defensive programming (clamping values) is essential to prevent logic exploits.
**Prevention:** Always validate inputs or clamp calculated values (like costs) to safe ranges (>= 0).
