using System;
using System.Collections.Generic;

namespace MCLauncher.Model;

[Serializable]
public class ProfileContainer
{
    public ProfileContainer()
    {
        Profiles = new List<Profile?>();
    }

    public List<Profile?> Profiles { get; set; }
}