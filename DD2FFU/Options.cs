using CommandLine;

namespace DD2FFU
{
    internal class Options
    {
        [Option('i', "img-file", HelpText = @"A path to the img file to convert *OR* a PhysicalDisk path. i.e. \\.\PhysicalDrive1 (Specifying a PhysicalDrive will make the tool eject the drive)", Required = true)]
        public string ImgFile
        {
            get; set;
        }

        [Option('f', "ffu-file", HelpText = "A path to the FFU file to output", Required = true)]
        public string FfuFile
        {
            get; set;
        }

        [Option('t', "temp", HelpText = "A path to the tool temporary directory", Required = false)]
        public string Temp
        {
            get; set;
        }

        [Option('l', "drive-letter",
            HelpText = "A drive letter that will be used during FFU generation for clean up. ie: X", Default = "B")]
        public string Drive
        {
            get; set;
        }

        [Option('e', "exclude-list", Required = false,
            HelpText = "Path to an optional partition exclude text list to use instead of the builtin one.")]
        public string Excludelist
        {
            get; set;
        }

        [Option('a', "anti-theft-version", Required = false, HelpText = "Anti theft version.", Default = "1.1")]
        public string Antitheftver
        {
            get; set;
        }

        [Option('o', "os-version", Required = false, HelpText = "Operating system version.",
            Default = "10.0.11111.0")]
        public string Osversion
        {
            get; set;
        }

        [Option('s', "oobe-selfhost", Required = false, HelpText = "Enable OOBE selfhost.", Default = false)]
        public bool Selfhost
        {
            get; set;
        }

        [Option('d', "disable-headless", Required = false, HelpText = "Disable OOBE headless.", Default = false)]
        public bool Headless
        {
            get; set;
        }

        [Option('r', "enable-recoveryffu", Required = false, HelpText = "Generates a recovery ffu with no partition skipped. Useful for clean state restore for a SPECIFIC unique device.", Default = false)]
        public bool Recovery
        {
            get; set;
        }
    }
}