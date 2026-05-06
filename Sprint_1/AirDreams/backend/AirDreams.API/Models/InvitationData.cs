public class InvitationData
{
    public string Email { get; set; } = string.Empty;

    public string TipoUsuario { get; set; } = string.Empty;

    public string Token { get; set; } = string.Empty;
    
    public DateTime ExpiryDate { get; set; }
}