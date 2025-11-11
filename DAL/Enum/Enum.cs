namespace DAL.Enum;

public enum CharacterOrderStatus
{
    Pending = 1,
    Completed = 3
}

public enum UserCharacterStatus
{
    Inactive = 0,
    Active = 1,
}

public enum CharacterPackageStatus
{
    Inactive = 0,
    Active = 1,
    Archived = 2
}

public enum PaymentStatus
{
    Pending = 1,
    Completed = 2,
    Failed = 3,
    Cancelled = 4,
    Refunded = 5
}

public enum OrderStatus
{
    Pending = 1,
    Processing = 2,
    Shipping = 3,
    Completed = 4
}

public enum OwnedDollStatus
{
    Inactive = 0,
    Active = 1,
}

public enum DollCharacterLinkStatus
{
    Unbound = 0,
    Bound = 1
}

public enum UserStatus
{
    Inactive = 0,
    Active = 1,
}
