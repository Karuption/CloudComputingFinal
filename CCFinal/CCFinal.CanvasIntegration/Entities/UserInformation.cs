using Microsoft.EntityFrameworkCore;

namespace CCFinal.CanvasIntegration.Entities;

[PrimaryKey(nameof(UserID))]
public class UserInformation {
    public string Token { get; set; }
    public DateTime? LastRunDateTime { get; set; }
    public Guid UserID { get; set; }
}
