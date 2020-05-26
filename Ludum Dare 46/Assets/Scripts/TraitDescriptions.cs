public static class TraitDescriptions
{
    public static string Get(BodyPartTrait trait)
    {
        switch (trait)
        {
            case BodyPartTrait.Strength:
                return "Increase damage output (can stack).";
            case BodyPartTrait.Defense:
                return "Decrease damage received (can stack).";
            case BodyPartTrait.Sharp:
                return "Deal double damage to eyes.";
            case BodyPartTrait.Poison:
                return "Add poison ability.";
            case BodyPartTrait.Immunity:
                return "Immune from poison.";
            case BodyPartTrait.Lucky:
                return "TODO";
            case BodyPartTrait.GodPower:
                return "TODO";
            case BodyPartTrait.Runner:
                return "TODO";
            case BodyPartTrait.Imbalance:
                return "TODO";
            case BodyPartTrait.Fragile:
                return "TODO";
            case BodyPartTrait.Puny:
                return "TODO";
            default:
                return "Error!";
        }
    }
}
