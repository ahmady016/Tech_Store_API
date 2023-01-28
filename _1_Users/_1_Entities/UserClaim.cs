using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace TechStoreApi.Entities;

[Table("UsersClaims")]
public class UserClaim : IdentityUserClaim<Guid> {}
