namespace TechStoreApi.DB.Common;

public enum Gender : byte
{
    Male = 1,
    Female = 2
}

public enum AccountStatuses : byte
{
    Pending = 1,
    Enabled = 2,
    Locked = 3,
    Closed = 4
}

public enum ProductCategories : byte
{
    PCs = 1,
    Laptops = 2,
    Tablets = 3,
    Mobiles = 4,
    Accessories = 5
}

public enum MessageTypes : byte
{
    Inquiry = 1,
    Suggestion = 2,
    Complaint = 3,
    Other = 4
}

public enum ReplyTypes : byte
{
    None = 0,
    Call = 1,
    SMS = 2,
    WhatsApp = 3,
    Email = 4,
}
