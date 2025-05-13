
namespace CommandSystem.Commands.IO
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.InteropServices;

    /// <summary>
    /// The file util.
    /// </summary>
    public static class FileUnlocker
    {
        /// <summary>
        /// The rm reboot reason none.
        /// </summary>
        private const int RmRebootReasonNone = 0;

        /// <summary>
        /// The cch rm max app name.
        /// </summary>
        private const int CchRmMaxAppName = 255;

        /// <summary>
        /// The cch rm max svc name.
        /// </summary>
        private const int CchRmMaxSVCName = 63;

        /// <summary>
        /// The r m_ ap p_ type.
        /// </summary>
        private enum RM_APP_TYPE
        {
            RmUnknownApp = 0,
            RmMainWindow = 1,
            RmOtherWindow = 2,
            RmService = 3,
            RmExplorer = 4,
            RmConsole = 5,
            RmCritical = 1000
        }

        /// <summary>
        /// Find out what process(es) have a lock on the specified file.
        /// </summary>
        /// <param name="path">Path of the file.</param>
        /// <returns>Processes locking the file</returns>
        /// <remarks>See also:
        /// http://msdn.microsoft.com/en-us/library/windows/desktop/aa373661(v=vs.85).aspx
        /// http://wyupdate.googlecode.com/svn-history/r401/trunk/frmFilesInUse.cs (no copyright in code at time of viewing)
        /// </remarks>
        public static List<Process> WhoIsLocking(string path)
        {
            string key = Guid.NewGuid().ToString();
            List<Process> processes = new List<Process>();

            int res = RmStartSession(out var handle, 0, key);
            if (res != 0) throw new Exception("Could not begin restart session.  Unable to determine file locker.");

            try
            {
                const int ERROR_MORE_DATA = 234;
                uint pnProcInfo = 0,
                     lpdwRebootReasons = RmRebootReasonNone;

                string[] resources = new string[] { path }; // Just checking on one resource.

                res = RmRegisterResources(handle, (uint)resources.Length, resources, 0, null, 0, null);

                if (res != 0) throw new Exception("Could not register resource.");

                // Note: there's a race condition here -- the first call to RmGetList() returns
                // the total number of process. However, when we call RmGetList() again to get
                // the actual processes this number may have increased.
                res = RmGetList(handle, out var pnProcInfoNeeded, ref pnProcInfo, null, ref lpdwRebootReasons);

                if (res == ERROR_MORE_DATA)
                {
                    // Create an array to store the process results
                    RM_PROCESS_INFO[] processInfo = new RM_PROCESS_INFO[pnProcInfoNeeded];
                    pnProcInfo = pnProcInfoNeeded;

                    // Get the list
                    res = RmGetList(handle, out pnProcInfoNeeded, ref pnProcInfo, processInfo, ref lpdwRebootReasons);
                    if (res == 0)
                    {
                        processes = new List<Process>((int)pnProcInfo);

                        // Enumerate all of the results and add them to the 
                        // list to be returned
                        for (int i = 0; i < pnProcInfo; i++)
                        {
                            try
                            {
                                processes.Add(Process.GetProcessById(processInfo[i].Process.DWProcessId));
                            }
                            catch (ArgumentException)
                            {
                                // Catch the error in case the process is no longer running
                            }
                        }
                    }
                    else throw new Exception("Could not list processes locking resource.");
                }
                else if (res != 0) throw new Exception("Could not list processes locking resource. Failed to get size of result.");
            }
            finally
            {
                RmEndSession(handle);
            }

            return processes;
        }

        /// <summary>
        /// Registers resources to a Restart Manager session. The Restart Manager uses the list of resources
        /// registered with the session to determine which applications and services must be shut down and restarted.
        /// Resources can be identified by file names, service short names, or RM_UNIQUE_PROCESS structures that describe running applications.
        /// The RmRegisterResources function can be used by a primary or secondary installer.
        /// </summary>
        /// <param name="pSessionHandle"> A handle to an existing Restart Manager session. </param>
        /// <param name="nFiles"> The number of files being registered. </param>
        /// <param name="rgsFileNames"> An array of null-terminated strings of full filename paths. This parameter can be NULL if nFiles is 0. </param>
        /// <param name="nApplications"> The number of processes being registered. </param>
        /// <param name="rgApplications"> An array of RM_UNIQUE_PROCESS structures. This parameter can be NULL if nApplications is 0. </param>
        /// <param name="nServices"> The number of services to be registered. </param>
        /// <param name="rgsServiceNames"> An array of null-terminated strings of service short names. This parameter can be NULL if nServices is 0. </param>
        /// <returns>
        /// This is the most recent error received. The function can return one of the system error codes that are defined in Winerror.h.
        /// ERROR_SUCCESS 0 — The function completed successfully.
        /// ERROR_SEM_TIMEOUT 121 — A Restart Manager function could not obtain a Registry write mutex in the allotted time. A system restart is recommended because further use of the Restart Manager is likely to fail.
        /// ERROR_BAD_ARGUMENTS 160 — One or more arguments are not correct. This error value is returned by the Restart Manager function if a NULL pointer or 0 is passed in a parameter that requires a non-null and non-zero value.
        /// ERROR_WRITE_FAULT 29 — An operation was unable to read or write to the registry.
        /// ERROR_OUTOFMEMORY 14 — A Restart Manager operation could not complete because not enough memory was available.
        /// ERROR_INVALID_HANDLE 6 — An invalid handle was passed to the function. No Restart Manager session exists for the handle supplied.
        /// </returns>
        /// <remarks>
        /// Each call to the RmRegisterResources function performs relatively expensive write operations.
        /// Do not call this function once per file, instead group related files together into components and register these together.
        /// </remarks>
        [DllImport("rstrtmgr.dll", CharSet = CharSet.Unicode)]
        private static extern int RmRegisterResources(
            uint pSessionHandle,
            uint nFiles,
            string[] rgsFileNames,
            uint nApplications,
            [In] RM_UNIQUE_PROCESS[] rgApplications,
            uint nServices,
            string[] rgsServiceNames);

        /// <summary>
        /// Starts a new Restart Manager session. A maximum of 64 Restart Manager sessions per user session
        /// can be open on the system at the same time. When this function starts a session,
        /// it returns a session handle and session key that can be used in subsequent calls to the Restart Manager API.
        /// </summary>
        /// <param name="pSessionHandle"> A pointer to the handle of a Restart Manager session. The session handle can be passed in subsequent calls to the Restart Manager API. </param>
        /// <param name="dwSessionFlags"> Reserved. This parameter should be 0. </param>
        /// <param name="strSessionKey"> A null-terminated string that contains the session key to the new session. The string must be allocated before calling the RmStartSession function. </param>
        /// <returns>
        /// This is the most recent error received. The function can return one of the system error codes that are defined in Winerror.h.
        /// ERROR_SUCCESS 0 — The function completed successfully.
        /// ERROR_SEM_TIMEOUT 121 — A Restart Manager function could not obtain a Registry write mutex in the allotted time. A system restart is recommended because further use of the Restart Manager is likely to fail.
        /// ERROR_BAD_ARGUMENTS 160 — One or more arguments are not correct. This error value is returned by the Restart Manager function if a NULL pointer or 0 is passed in a parameter that requires a non-null and non-zero value.
        /// ERROR_MAX_SESSIONS_REACHED 353 — The maximum number of sessions has been reached.
        /// ERROR_WRITE_FAULT 29 — An operation was unable to read or write to the registry.
        /// ERROR_OUTOFMEMORY 14 — A Restart Manager operation could not complete because not enough memory was available.
        /// </returns>
        [DllImport("rstrtmgr.dll", CharSet = CharSet.Auto)]
        private static extern int RmStartSession(out uint pSessionHandle, int dwSessionFlags, string strSessionKey);

        /// <summary>
        /// Ends the Restart Manager session. This function should be called by the primary installer
        /// that has previously started the session by calling the RmStartSession function.
        /// The RmEndSession function can be called by a secondary installer that is joined
        /// to the session once no more resources need to be registered by the secondary installer.
        /// </summary>
        /// <param name="pSessionHandle"> A handle to an existing Restart Manager session.. </param>
        /// <returns>
        /// This is the most recent error received. The function can return one of the system error codes that are defined in Winerror.h.
        /// ERROR_SUCCESS 0 — The function completed successfully.
        /// ERROR_SEM_TIMEOUT 121 — A Restart Manager function could not obtain a Registry write mutex in the allotted time. A system restart is recommended because further use of the Restart Manager is likely to fail.
        /// ERROR_WRITE_FAULT 29 — An operation was unable to read or write to the registry.
        /// ERROR_OUTOFMEMORY 14 — A Restart Manager operation could not complete because not enough memory was available.
        /// ERROR_INVALID_HANDLE 6 — An invalid handle was passed to the function. No Restart Manager session exists for the handle supplied.
        /// </returns>
        [DllImport("rstrtmgr.dll")]
        private static extern int RmEndSession(uint pSessionHandle);

        /// <summary>
        /// Gets a list of all applications and services that are currently using resources
        /// that have been registered with the Restart Manager session.
        /// </summary>
        /// <param name="dwSessionHandle"> A handle to an existing Restart Manager session. </param>
        /// <param name="pnProcInfoNeeded"> A pointer to an array size necessary to receive RM_PROCESS_INFO structures required to return information for all affected applications and services. </param>
        /// <param name="pnProcInfo"> A pointer to the total number of RM_PROCESS_INFO structures in an array and number of structures filled. </param>
        /// <param name="rgAffectedApps"> An array of RM_PROCESS_INFO structures that list the applications and services using resources that have been registered with the session. </param>
        /// <param name="lpdwRebootReasons"> Pointer to location that receives a value of the RM_REBOOT_REASON enumeration that describes the reason a system restart is needed. </param>
        /// <returns>
        /// This is the most recent error received. The function can return one of the system error codes that are defined in Winerror.h.
        /// ERROR_SUCCESS 0 — The function completed successfully.
        /// ERROR_MORE_DATA 234 — This error value is returned by the RmGetList function if the rgAffectedApps buffer is too small to hold all application information in the list.
        /// ERROR_CANCELLED 1223 — The current operation is canceled by user.
        /// ERROR_SEM_TIMEOUT 121 — A Restart Manager function could not obtain a Registry write mutex in the allotted time. A system restart is recommended because further use of the Restart Manager is likely to fail.
        /// ERROR_BAD_ARGUMENTS 160 — One or more arguments are not correct. This error value is returned by the Restart Manager function if a NULL pointer or 0 is passed in a parameter that requires a non-null and non-zero value.
        /// ERROR_WRITE_FAULT 29 — An operation was unable to read or write to the registry.
        /// ERROR_OUTOFMEMORY 14 — A Restart Manager operation could not complete because not enough memory was available.
        /// ERROR_INVALID_HANDLE 6 — No Restart Manager session exists for the handle supplied.
        /// </returns>
        [DllImport("rstrtmgr.dll")]
        private static extern int RmGetList(
            uint dwSessionHandle,
            out uint pnProcInfoNeeded,
            ref uint pnProcInfo,
            [In, Out] RM_PROCESS_INFO[] rgAffectedApps,
            ref uint lpdwRebootReasons);

        /// <summary>
        /// Describes an application that is to be registered with the Restart Manager.
        /// </summary>
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        private struct RM_PROCESS_INFO
        {
            /// <summary>
            /// Contains an RM_UNIQUE_PROCESS structure that uniquely identifies the application by its PID and the time the process began.
            /// </summary>
            public readonly RM_UNIQUE_PROCESS Process;

            /// <summary>
            /// If the process is a service, this parameter returns the long name for the service. If the process is not a service,
            /// this parameter returns the user-friendly name for the application. If the process is a critical process,
            /// and the installer is run with elevated privileges, this parameter returns the name of the executable file of the critical process.
            /// If the process is a critical process, and the installer is run as a service,
            /// this parameter returns the long name of the critical process.
            /// </summary>
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = CchRmMaxAppName + 1)]
            private readonly string strAppName;

            /// <summary>
            /// Contains the Terminal Services session ID of the process. If the terminal session of the process cannot be determined,
            /// the value of this member is set to RM_INVALID_SESSION (-1). This member is not used if the process is a service or a system critical process.
            /// </summary>
            private readonly uint tsSessionId;

            /// <summary>
            /// TRUE if the application can be restarted by the Restart Manager; otherwise, FALSE.
            /// This member is always TRUE if the process is a service. This member is always FALSE if the process is a critical system process.
            /// </summary>
            [MarshalAs(UnmanagedType.Bool)]
            private readonly bool bRestartable;

            /// <summary>
            /// Contains an RM_APP_TYPE enumeration value that specifies the type of application
            /// as RmUnknownApp, RmMainWindow, RmOtherWindow, RmService, RmExplorer or RmCritical.
            /// </summary>
            private readonly RM_APP_TYPE applicationType;

            /// <summary>
            /// Contains a bit mask that describes the current status of the application. See the RM_APP_STATUS enumeration.
            /// </summary>
            private readonly uint appStatus;

            /// <summary>
            /// If the process is a service, this is the short name for the service.
            /// This member is not used if the process is not a service.
            /// </summary>
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = CchRmMaxSVCName + 1)]
            private readonly string strServiceShortName;
        }

        /// <summary>
        /// Uniquely identifies a process by its PID and the time the process began. An array of RM_UNIQUE_PROCESS structures
        /// can be passed to the RmRegisterResources function.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        private struct RM_UNIQUE_PROCESS
        {
            /// <summary>
            /// The product identifier (PID).
            /// </summary>
            public readonly int DWProcessId;

            /// <summary>
            /// The creation time of the process. The time is provided as a FILETIME structure
            /// that is returned by the lpCreationTime parameter of the GetProcessTimes function.
            /// </summary>
            private readonly System.Runtime.InteropServices.ComTypes.FILETIME processStartTime;
        }
    }
}
