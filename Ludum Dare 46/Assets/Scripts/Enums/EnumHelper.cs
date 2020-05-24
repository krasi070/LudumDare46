public static class EnumHelper
{
    public static string BodyPartTypeToString(BodyPartType type)
    {
        switch (type)
        {
            case BodyPartType.LeftArm:
                return "Left Arm";
            case BodyPartType.RightArm:
                return "Right Arm";
            case BodyPartType.LeftLeg:
                return "Left Leg";
            case BodyPartType.RightLeg:
                return "Right Leg";
            case BodyPartType.LeftEye:
                return "Left Eye";
            case BodyPartType.RightEye:
                return "Right Eye";
            case BodyPartType.Tail:
                return "Tail";
            default:
                return null;
        }
    }
}