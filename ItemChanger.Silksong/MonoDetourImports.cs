using MonoDetour.HookGen;

[assembly: MonoDetourTargets(typeof(GameManager), GenerateControlFlowVariants = true)]
[assembly: MonoDetourTargets(typeof(HeroController), GenerateControlFlowVariants = true)]
[assembly: MonoDetourTargets(typeof(InventoryItemConditional))]