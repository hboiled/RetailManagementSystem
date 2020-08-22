using System.Collections.Generic;
using System.Linq;

namespace RMS_DESKTOP_UI.Library.Models
{
    public class UserModel
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public Dictionary<string, string> Roles { get; set; } = new Dictionary<string, string>();
        
        // for displaying roles in the UI
        public string RoleList
        {
            get {
                return string.Join(", ", Roles.Select(x => x.Value));
            }        
        }

    }
}