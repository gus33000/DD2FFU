namespace DD2FFU
{
    internal class Constants
    {
        internal const ulong SectorSize = 0x200;

        internal static readonly string[] partitions = new string[]
        {
            "DPP",
            "MODEM_FSG",
            "MODEM_FS1",
            "MODEM_FS2",
            "MODEM_FSC",
            "DDR",
            "SEC",
            "APDP",
            "MSADP",
            "DPO",
            "SSD",
            "UEFI_BS_NV",
            "UEFI_NV",
            "UEFI_RT_NV",
            "UEFI_RT_NV_RPMB",
            "BOOTMODE",
            "LIMITS",
            "BACKUP_BS_NV",
            "BACKUP_SBL1",
            "BACKUP_SBL2",
            "BACKUP_SBL3",
            "BACKUP_PMIC",
            "BACKUP_DBI",
            "BACKUP_UEFI",
            "BACKUP_RPM",
            "BACKUP_QSEE",
            "BACKUP_QHEE",
            "BACKUP_TZ",
            "BACKUP_HYP",
            "BACKUP_WINSECAPP",
            "BACKUP_TZAPPS",
            "SVRawDump"
        };
    }
}