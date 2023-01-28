using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace TechStoreApi.Entities;

[Table("RolesClaims")]
public class RoleClaim : IdentityRoleClaim<Guid> {}
