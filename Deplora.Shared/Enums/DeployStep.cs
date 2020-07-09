using System;
using System.Collections.Generic;
using System.Text;

namespace Deplora.Shared.Enums
{
    public enum DeployStep
    {
        Rollback = -1,
        InPreparation = 0,
        StoppingAppPool = 1,
        StoppingWebsite = 2,
        BackingUpDatabase = 3,
        BackingUpFiles = 4,
        Deploying = 5,
        StartingAppPool = 6,
        RunningSqlCommands = 7,
        StartingWebsite = 8,
        Finished = 9
    }
}
