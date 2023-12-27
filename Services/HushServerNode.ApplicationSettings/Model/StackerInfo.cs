namespace HushServerNode.ApplicationSettingsService.Model;

public class StackerInfo 
{
    public string PublicSigningAddress { get; set; } = string.Empty;

    public string PrivateSigningAddress { get; set; } = string.Empty;
    
    public string PublicEncryptAddress { get; set; } = string.Empty;
    
    public string PrivateEncryptAddress { get; set; } = string.Empty;
}
