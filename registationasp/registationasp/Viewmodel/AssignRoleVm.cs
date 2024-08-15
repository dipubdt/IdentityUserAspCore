namespace registationasp.Viewmodel;

public class AssignRoleVm
{

    public string SelectedRole { get; set; } = string.Empty;
    public string SelectedUser { get; set; } = string.Empty;

    public IEnumerable<string> Roles { get; set; } = new List<string>();
    public IEnumerable<string> Users { get; set; } = new List<string>();
}
