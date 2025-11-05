namespace DAL.Enum;

public enum CharacterOrderStatus
{
    Pending = 0,
    Active = 1,
    Completed = 2,
    Cancelled = 3,
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
    Pending = 0,
    Completed = 1,
    Failed = 2,
    Cancelled = 3,
    Refunded = 4
}

public enum OrderStatus
{
    Pending = 0,
    Processing = 1,
    Shipped = 2,
    Completed = 3,
    Cancelled = 4,
    Returned = 5
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
