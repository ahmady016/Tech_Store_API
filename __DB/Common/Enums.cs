namespace TechStoreApi.DB.Common;

public enum Gender : byte
{
    Male = 1,
    Female = 2
}

public enum ProductCategories : byte
{
    PCs = 1,
    Laptops = 2,
    Tablets = 3,
    Mobiles = 4,
    Accessories = 5
}

public enum ContactTypes : byte
{
    Phone = 1,
    Fax = 2,
    Mobile = 3,
}

public enum MessageTypes : byte
{
    Inquiry = 1,
    Suggestion = 2,
    Complaint = 3,
    Other = 4
}
