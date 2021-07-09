using CommandSystem;
using Exiled.Permissions.Extensions;
using System;

namespace MurderMystery.Commands.General
{
    public class SetConfig : ICommand
    {
        public string Command => "setconfig";

        public string[] Aliases => new string[] { "cfg", "scfg" };

        public string Description => "Allows setting of murder mystery config values.";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission("mm.config"))
            {
                response = "You don't have permission to change config values.";
                return false;
            }

			response = "This command is not implemented.";
			return false;

            string[] args = arguments.Array;

            /*if (args.Length < 4)
            {
                response = "You must provide a value.";
                return false;
            }

            switch (args[2].ToLower())
            {
                case "murdererpercent":
                case "mpercent":
                case "mperc":
                    if (float.TryParse(args[3], out float percent))
                    {
                        percent /= 100f;

                        if (percent < 100f && percent > 0f)
                        {
                            if (percent + MurderMystery.Singleton.Config.DetectivePercentage <= 1f)
                            {
                               MurderMystery.Singleton.Config.MurdererPercentage = percent;
                               response = "Murderer percentage has been set to: " + (percent *= 100f);
                               return true;
                            }
                            else
                            {
                                response = "The total between murderer and detective percentage is too high if set to this number! Lower the detective percentage first!";
                                return false;
                            }
                        }
                        else
                        {
                            response = "The percentage must be greater than zero and less than 100.";
                            return false;
                        }
                    }
                    else if (args[3].ToLower() == "default")
                    {
                        if (Config.DefaultMurdererPercentage + MurderMystery.Singleton.Config.DetectivePercentage <= 1f)
                        {
                            MurderMystery.Singleton.Config.MurdererPercentage = Config.DefaultMurdererPercentage;
                            response = "Murderer percentage has been set to: " + (DefaultMurdererPercentage *= 100f);
                            return true;
                        }
                        else
                        {
                            response = "The total between murderer and detective percentage is too high if set to this number! Lower the detective percentage first!";
                            return false;
                        }
                    }
                    else
                    {
                        response = "The provided value could not be parsed!";
                        return false;
                    }
                    return true;
                case "detectivepercent":
                case "dpercent":
                case "dperc":
                    return true;
                case "equipmenttime":
                case "eqtime":
                    return true;

            }

            response = "The murder mystery gamemode has been forcefully ended.";
            return true;*/
        }
    }
}
