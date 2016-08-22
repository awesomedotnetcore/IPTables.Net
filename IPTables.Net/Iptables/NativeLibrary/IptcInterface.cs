﻿//#define DEBUG_NATIVE_IPTCP
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Common.Logging;
using IPTables.Net.Exceptions;

namespace IPTables.Net.Iptables.NativeLibrary
{
    public class IptcInterface : IDisposable
    {
        private IntPtr _handle;
        public const String LibraryV4 = "libip4tc.so";
        public const String LibraryV6 = "libip6tc.so";
        public const String Helper = "libipthelper.so";
        public const int StringLabelLength = 32;

        public const String IPTC_LABEL_ACCEPT = "ACCEPT";
        public const String IPTC_LABEL_DROP = "DROP";
        public const String IPTC_LABEL_QUEUE = "QUEUE";
        public const String IPTC_LABEL_RETURN = "RETURN";

        /* Does this chain exist? */
        [DllImport(LibraryV4, SetLastError = true)]
        public static extern int iptc_is_chain(String chain, IntPtr handle);

        /* Take a snapshot of the rules.  Returns NULL on error. */
        [DllImport(LibraryV4, SetLastError = true)]
        public static extern IntPtr iptc_init(String tablename);

        /* Cleanup after iptc_init(). */
        [DllImport(LibraryV4, SetLastError = true)]
        public static extern void iptc_free(IntPtr h);

        /* Iterator functions to run through the chains.  Returns NULL at end. */
        [DllImport(LibraryV4, SetLastError = true)]
        public static extern IntPtr iptc_first_chain(IntPtr handle);
        [DllImport(LibraryV4, SetLastError = true)]
        public static extern IntPtr iptc_next_chain(IntPtr handle);

        /* Get first rule in the given chain: NULL for empty chain. */
        [DllImport(LibraryV4, SetLastError = true)]
        public static extern IntPtr iptc_first_rule(String chain,
                            IntPtr handle);

        /* Returns NULL when rules run out. */
        [DllImport(LibraryV4, SetLastError = true)]
        public static extern IntPtr iptc_next_rule(IntPtr prev,
                               IntPtr handle);

        /* Returns a pointer to the target name of this entry. */
        [DllImport(LibraryV4, SetLastError = true)]
        public static extern String iptc_get_target(IntPtr e,
                        IntPtr handle);

        /* Is this a built-in chain? */
        [DllImport(LibraryV4, SetLastError = true)]
        public static extern int iptc_builtin(String chain, IntPtr handle);

        /* Get the policy of a given built-in chain */
        [DllImport(LibraryV4, SetLastError = true)]
        public static extern String iptc_get_policy(String chain,
                        IntPtr counter,
                        IntPtr handle);

        /* These functions return TRUE for OK or 0 and set errno.  If errno ==
           0, it means there was a version error (ie. upgrade libiptc). */
        /* Rule numbers start at 1 for the first rule. */

        /* Insert the entry `e' in chain `chain' into position `rulenum'. */
        [DllImport(LibraryV4, SetLastError = true)]
        public static extern int iptc_insert_entry(
            [MarshalAs(UnmanagedType.LPStr, SizeConst = StringLabelLength)] String chain,
                      IntPtr e,
                      uint rulenum,
                      IntPtr handle);

        /* Atomically replace rule `rulenum' in `chain' with `e'. */
        [DllImport(LibraryV4, SetLastError = true)]
        public static extern int iptc_replace_entry([MarshalAs(UnmanagedType.LPStr, SizeConst = StringLabelLength)]
                String chain,
                       IntPtr e,
                       uint rulenum,
                       IntPtr handle);

        /* Append entry `e' to chain `chain'.  Equivalent to insert with
           rulenum = length of chain. */
        [DllImport(LibraryV4, SetLastError = true)]
        public static extern int iptc_append_entry(
            [MarshalAs(UnmanagedType.LPStr, SizeConst = StringLabelLength)]
                String chain,
                      IntPtr e,
                      IntPtr handle);

        /* Delete the first rule in `chain' which matches `e', subject to
           matchmask (array of length == origfw) */
        [DllImport(LibraryV4, SetLastError = true)]
        public static extern int iptc_delete_entry(
            [MarshalAs(UnmanagedType.LPStr, SizeConst = StringLabelLength)]
                String chain,
                      IntPtr origfw,
                      String matchmask,
                      IntPtr handle);

        /* Delete the rule in position `rulenum' in `chain'. */
        [DllImport(LibraryV4, SetLastError = true)]
        public static extern int iptc_delete_num_entry(
            [MarshalAs(UnmanagedType.LPStr, SizeConst = StringLabelLength)]
                String chain,
                      uint rulenum,
                      IntPtr handle);

        /* Flushes the entries in the given chain (ie. empties chain). */
        [DllImport(LibraryV4, SetLastError = true)]
        public static extern int iptc_flush_entries(
            [MarshalAs(UnmanagedType.LPStr, SizeConst = StringLabelLength)]
                String chain,
                       IntPtr handle);

        /* Zeroes the counters in a chain. */
        [DllImport(LibraryV4, SetLastError = true)]
        public static extern int iptc_zero_entries(
            [MarshalAs(UnmanagedType.LPStr, SizeConst = StringLabelLength)]
                String chain,
                      IntPtr handle);

        /* Creates a new chain. */
        [DllImport(LibraryV4, SetLastError = true)]
        public static extern int iptc_create_chain(
            [MarshalAs(UnmanagedType.LPStr, SizeConst = StringLabelLength)]
                String chain,
                      IntPtr handle);

        /* Deletes a chain. */
        [DllImport(LibraryV4, SetLastError = true)]
        static extern int iptc_delete_chain(
            [MarshalAs(UnmanagedType.LPStr, SizeConst = StringLabelLength)]
                String chain,
                      IntPtr handle);

        /* Renames a chain. */
        [DllImport(LibraryV4, SetLastError = true)]
        public static extern int iptc_rename_chain(
            [MarshalAs(UnmanagedType.LPStr, SizeConst = StringLabelLength)]
                String chain,
                      IntPtr handle);

        /* Sets the policy on a built-in chain. */
        [DllImport(LibraryV4, SetLastError = true)]
        public static extern int iptc_set_policy(
            [MarshalAs(UnmanagedType.LPStr, SizeConst = StringLabelLength)]
                String chain,
            [MarshalAs(UnmanagedType.LPStr, SizeConst = StringLabelLength)]
                String chainPolicy,
                    IntPtr counters,
                    IntPtr handle);

        /* Get the number of references to this chain */
        [DllImport(LibraryV4, SetLastError = true)]
        public static extern int iptc_get_references(IntPtr references,
                [MarshalAs(UnmanagedType.LPStr, SizeConst = StringLabelLength)]
                String chain,
                    IntPtr handle);

        /* read packet and byte counters for a specific rule */
        [DllImport(LibraryV4, SetLastError = true)]
        public static extern IntPtr iptc_read_counter(
            [MarshalAs(UnmanagedType.LPStr, SizeConst = StringLabelLength)]
                String chain,
                               uint rulenum,
                               IntPtr handle);

        /* zero packet and byte counters for a specific rule */
        [DllImport(LibraryV4, SetLastError = true)]
        public static extern int iptc_zero_counter(
            [MarshalAs(UnmanagedType.LPStr, SizeConst = StringLabelLength)]
                String chain,
                      uint rulenum,
                      IntPtr handle);

        /* set packet and byte counters for a specific rule */
        [DllImport(LibraryV4, SetLastError = true)]
        public static extern int iptc_set_counter(
            [MarshalAs(UnmanagedType.LPStr, SizeConst = StringLabelLength)]
                String chain,
                     uint rulenum,
                     IntPtr counters,
                     IntPtr handle);

        /* Makes the actual changes. */
        [DllImport(LibraryV4, SetLastError = true)]
        public static extern int iptc_commit(IntPtr handle);

        /* Translates errno numbers into more human-readable form than strerror. */
        [DllImport(LibraryV4, SetLastError = true)]
        public static extern IntPtr iptc_strerror(int err);

        /* Does this chain exist? */
        [DllImport(LibraryV6, SetLastError = true)]
        public static extern int ip6tc_is_chain(String chain, IntPtr handle);

        /* Take a snapshot of the rules.  Returns NULL on error. */
        [DllImport(LibraryV6, SetLastError = true)]
        public static extern IntPtr ip6tc_init(String tablename);

        /* Cleanup after iptc_init(). */
        [DllImport(LibraryV6, SetLastError = true)]
        public static extern void ip6tc_free(IntPtr h);

        /* Iterator functions to run through the chains.  Returns NULL at end. */
        [DllImport(LibraryV6, SetLastError = true)]
        public static extern IntPtr ip6tc_first_chain(IntPtr handle);
        [DllImport(LibraryV6, SetLastError = true)]
        public static extern IntPtr ip6tc_next_chain(IntPtr handle);

        /* Get first rule in the given chain: NULL for empty chain. */
        [DllImport(LibraryV6, SetLastError = true)]
        public static extern IntPtr ip6tc_first_rule(String chain,
                            IntPtr handle);

        /* Returns NULL when rules run out. */
        [DllImport(LibraryV6, SetLastError = true)]
        public static extern IntPtr ip6tc_next_rule(IntPtr prev,
                               IntPtr handle);

        /* Returns a pointer to the target name of this entry. */
        [DllImport(LibraryV6, SetLastError = true)]
        public static extern String ip6tc_get_target(IntPtr e,
                        IntPtr handle);

        /* Is this a built-in chain? */
        [DllImport(LibraryV6, SetLastError = true)]
        public static extern int ip6tc_builtin(String chain, IntPtr handle);

        /* Get the policy of a given built-in chain */
        [DllImport(LibraryV6, SetLastError = true)]
        public static extern String ip6tc_get_policy(String chain,
                        IntPtr counter,
                        IntPtr handle);

        /* These functions return TRUE for OK or 0 and set errno.  If errno ==
           0, it means there was a version error (ie. upgrade libiptc). */
        /* Rule numbers start at 1 for the first rule. */

        /* Insert the entry `e' in chain `chain' into position `rulenum'. */
        [DllImport(LibraryV6, SetLastError = true)]
        public static extern int ip6tc_insert_entry(
            [MarshalAs(UnmanagedType.LPStr, SizeConst = StringLabelLength)] String chain,
                      IntPtr e,
                      uint rulenum,
                      IntPtr handle);

        /* Atomically replace rule `rulenum' in `chain' with `e'. */
        [DllImport(LibraryV6, SetLastError = true)]
        public static extern int ip6tc_replace_entry([MarshalAs(UnmanagedType.LPStr, SizeConst = StringLabelLength)]
                String chain,
                       IntPtr e,
                       uint rulenum,
                       IntPtr handle);

        /* Append entry `e' to chain `chain'.  Equivalent to insert with
           rulenum = length of chain. */
        [DllImport(LibraryV6, SetLastError = true)]
        public static extern int ip6tc_append_entry(
            [MarshalAs(UnmanagedType.LPStr, SizeConst = StringLabelLength)]
                String chain,
                      IntPtr e,
                      IntPtr handle);

        /* Delete the first rule in `chain' which matches `e', subject to
           matchmask (array of length == origfw) */
        [DllImport(LibraryV6, SetLastError = true)]
        public static extern int ip6tc_delete_entry(
            [MarshalAs(UnmanagedType.LPStr, SizeConst = StringLabelLength)]
                String chain,
                      IntPtr origfw,
                      String matchmask,
                      IntPtr handle);

        /* Delete the rule in position `rulenum' in `chain'. */
        [DllImport(LibraryV6, SetLastError = true)]
        public static extern int ip6tc_delete_num_entry(
            [MarshalAs(UnmanagedType.LPStr, SizeConst = StringLabelLength)]
                String chain,
                      uint rulenum,
                      IntPtr handle);

        /* Flushes the entries in the given chain (ie. empties chain). */
        [DllImport(LibraryV6, SetLastError = true)]
        public static extern int ip6tc_flush_entries(
            [MarshalAs(UnmanagedType.LPStr, SizeConst = StringLabelLength)]
                String chain,
                       IntPtr handle);

        /* Zeroes the counters in a chain. */
        [DllImport(LibraryV6, SetLastError = true)]
        public static extern int ip6tc_zero_entries(
            [MarshalAs(UnmanagedType.LPStr, SizeConst = StringLabelLength)]
                String chain,
                      IntPtr handle);

        /* Creates a new chain. */
        [DllImport(LibraryV6, SetLastError = true)]
        public static extern int ip6tc_create_chain(
            [MarshalAs(UnmanagedType.LPStr, SizeConst = StringLabelLength)]
                String chain,
                      IntPtr handle);

        /* Deletes a chain. */
        [DllImport(LibraryV6, SetLastError = true)]
        static extern int ip6tc_delete_chain(
            [MarshalAs(UnmanagedType.LPStr, SizeConst = StringLabelLength)]
                String chain,
                      IntPtr handle);

        /* Renames a chain. */
        [DllImport(LibraryV6, SetLastError = true)]
        public static extern int ip6tc_rename_chain(
            [MarshalAs(UnmanagedType.LPStr, SizeConst = StringLabelLength)]
                String chain,
                      IntPtr handle);

        /* Sets the policy on a built-in chain. */
        [DllImport(LibraryV6, SetLastError = true)]
        public static extern int ip6tc_set_policy(
            [MarshalAs(UnmanagedType.LPStr, SizeConst = StringLabelLength)]
                String chain,
            [MarshalAs(UnmanagedType.LPStr, SizeConst = StringLabelLength)]
                String chainPolicy,
                    IntPtr counters,
                    IntPtr handle);

        /* Get the number of references to this chain */
        [DllImport(LibraryV6, SetLastError = true)]
        public static extern int ip6tc_get_references(IntPtr references,
                [MarshalAs(UnmanagedType.LPStr, SizeConst = StringLabelLength)]
                String chain,
                    IntPtr handle);

        /* read packet and byte counters for a specific rule */
        [DllImport(LibraryV6, SetLastError = true)]
        public static extern IntPtr ip6tc_read_counter(
            [MarshalAs(UnmanagedType.LPStr, SizeConst = StringLabelLength)]
                String chain,
                               uint rulenum,
                               IntPtr handle);

        /* zero packet and byte counters for a specific rule */
        [DllImport(LibraryV6, SetLastError = true)]
        public static extern int ip6tc_zero_counter(
            [MarshalAs(UnmanagedType.LPStr, SizeConst = StringLabelLength)]
                String chain,
                      uint rulenum,
                      IntPtr handle);

        /* set packet and byte counters for a specific rule */
        [DllImport(LibraryV6, SetLastError = true)]
        public static extern int ip6tc_set_counter(
            [MarshalAs(UnmanagedType.LPStr, SizeConst = StringLabelLength)]
                String chain,
                     uint rulenum,
                     IntPtr counters,
                     IntPtr handle);

        /* Makes the actual changes. */
        [DllImport(LibraryV6, SetLastError = true)]
        public static extern int ip6tc_commit(IntPtr handle);

        /* Translates errno numbers into more human-readable form than strerror. */
        [DllImport(LibraryV6, SetLastError = true)]
        public static extern IntPtr ip6tc_strerror(int err);

        [DllImport(Helper, SetLastError = true)]
        public static extern IntPtr output_rule4(IntPtr e, IntPtr h, String chain, int counters);

        [DllImport(Helper, SetLastError = true)]
        public static extern IntPtr output_rule6(IntPtr e, IntPtr h, String chain, int counters);

        [DllImport(Helper, SetLastError = true)]
        public static extern int execute_command4(String command, IntPtr h);

        [DllImport(Helper, SetLastError = true)]
        public static extern int execute_command6(String command, IntPtr h);

        [DllImport(Helper, SetLastError = true)]
        public static extern int init_helper4();

        [DllImport(Helper, SetLastError = true)]
        public static extern int init_helper6();

        [DllImport(Helper, SetLastError = true)]
        static extern IntPtr init_handle4(String table);

        [DllImport(Helper, SetLastError = true)]
        static extern IntPtr init_handle6(String table);

        [DllImport(Helper)]
        static extern IntPtr last_error();

        [DllImport(Helper)]
        static extern IntPtr ipth_bpf_compile([MarshalAs(UnmanagedType.LPStr)]String dltname, [MarshalAs(UnmanagedType.LPStr)]String code, int length);
        [DllImport(Helper)]
        static extern void ipth_free(IntPtr ptr);

        public static String BpfCompile(String dltName, String code, int programBufLen)
        {
            IntPtr ptr = ipth_bpf_compile(dltName, code, programBufLen);
            if (ptr == IntPtr.Zero)
            {
                return null;
            }
            String str = Marshal.PtrToStringAnsi(ptr);
            ipth_free(ptr);
            return str;
        }

        public String LastError()
        {
            return Marshal.PtrToStringAnsi(last_error());
        }

        private static int _helperInit = 0;

        public static bool DllExists(out String msg)
        {
            try
            {
                Marshal.PrelinkAll(typeof (IptcInterface));
            }
            catch (DllNotFoundException ex)
            {
                msg = ex.Message;
                return false;
            }
            msg = null;
            return true;
        }

        public static bool DllExists()
        {
            String msg;
            return DllExists(out msg);
        }

        public IptcInterface(String table, int ipVersion, ILog log = null)
        {
            _ipVersion = ipVersion;
            logger = log;
            if (_helperInit != 0)
            {
                throw new IpTablesNetException("Can't initialize another IptcInterface before disposing of the last");
            }
            else
                {
                int res;
                if (ipVersion == 4)
                {
                    res = init_helper4();
                }
                else
                {
                    res = init_helper6();
                }
                if (res < 0)
                {
                    throw new IpTablesNetException("Failed to initialize the helper / xtables");
                }
                _helperInit = ipVersion;
            }
            OpenTable(table);
        }

        ~IptcInterface()
        {
            Dispose();
        }

        public void Dispose()
        {
            _helperInit = 0;
            if (_handle != IntPtr.Zero)
            {
                Free(); 
            }
        }

        private List<String> _debugEntries = new List<string>();
        private ILog logger;
        private int _ipVersion;

        private void DebugEntry(string message)
        {
            if (logger != null)
            {
                _debugEntries.Add(message);
            }
        }

        private void RequireHandle()
        {
            if (_handle == IntPtr.Zero)
            {
                throw new IpTablesNetException("No IP Table currently open");
            }
        }

        private void Free()
        {
            RequireHandle();
            if (_ipVersion == 4)
            {
                iptc_free(_handle);
            }
            else
            {
                ip6tc_free(_handle);
            }
            _handle = IntPtr.Zero;
        }

        public void OpenTable(String table)
        {
            if (_handle != IntPtr.Zero)
            {
                throw new IpTablesNetException("A table is already open, commit or discard first");
            }
            if (_ipVersion == 4)
            {
                _handle = init_handle4(table);
            }
            else
            {
                _handle = init_handle6(table);
            }
            if (_handle == IntPtr.Zero)
            {
                throw new IpTablesNetException(String.Format("Failed to open table \"{0}\", error: {1}", table, LastError()));
            }
        }

        public List<IntPtr> GetRules(String chain)
        {
            RequireHandle();
            List<IntPtr> ret = new List<IntPtr>();
            IntPtr rule;
            if (_ipVersion == 4)
            {
                rule = iptc_first_rule(chain, _handle);
            }
            else
            {
                rule = ip6tc_first_rule(chain, _handle);
            }
            while (rule != IntPtr.Zero)
            {
                ret.Add(rule);
                if (_ipVersion == 4)
                {
                    rule = iptc_next_rule(rule, _handle);
                }
                else
                {
                    rule = ip6tc_next_rule(rule, _handle);
                }
            }
            return ret;
        }


        public List<string> GetChains()
        {
            RequireHandle();
            List<string> ret = new List<string>();
            IntPtr chain;
            if (_ipVersion == 4)
            {
                chain = iptc_first_chain(_handle);
            }
            else
            {
                chain = ip6tc_first_chain(_handle);
            }
            while (chain != IntPtr.Zero)
            {
                ret.Add(Marshal.PtrToStringAnsi(chain));
                if (_ipVersion == 4)
                {
                    chain = iptc_next_chain(_handle);
                }
                else
                {
                    chain = ip6tc_next_chain(_handle);
                }
            }
            return ret;
        }

        public int GetLastError()
        {
            return Marshal.GetLastWin32Error();
        }

        public String GetErrorString()
        {
            int lastError = GetLastError();
            IntPtr error;
            if (_ipVersion == 4)
            {
                error = iptc_strerror(lastError);
            }
            else
            {
                error = ip6tc_strerror(lastError);
            }
            return String.Format("({0}) {1}",lastError,Marshal.PtrToStringAnsi(error));
        }


        public String GetRuleString(String chain, IntPtr rule, bool counters = false)
        {
            RequireHandle();

            IntPtr ptr;
            if (_ipVersion == 4)
            {
                ptr = output_rule4(rule, _handle, chain, counters ? 1 : 0);
            }
            else
            {
                ptr = output_rule6(rule, _handle, chain, counters ? 1 : 0);
            }
            if (ptr == IntPtr.Zero)
            {
                throw new IpTablesNetException("IPTCH Error: " + LastError().Trim());
            }
            return Marshal.PtrToStringAnsi(ptr);
        }

        /// <summary>
        /// Insert a rule
        /// </summary>
        /// <param name="chain"></param>
        /// <param name="entry"></param>
        /// <param name="at"></param>
        /// <returns></returns>
        public bool Insert(String chain, IntPtr entry, uint at)
        {
            RequireHandle();
            if (_ipVersion == 4)
            {
                return iptc_insert_entry(chain, entry, at, _handle) == 1;
            }
            return ip6tc_insert_entry(chain, entry, at, _handle) == 1;
        }

        /// <summary>
        /// Execute an IPTables command (add, remove, delete insert)
        /// </summary>
        /// <param name="command"></param>
        /// <returns>returns 1 for sucess, error code otherwise</returns>
        public int ExecuteCommand(string command)
        {
            DebugEntry(command);
            RequireHandle();
            int ptr;
            if (_ipVersion == 4)
            {
                ptr = execute_command4(command, _handle);
            }
            else
            {
                ptr = execute_command6(command, _handle);
            }

            if (ptr == 0)
            {
                throw new IpTablesNetException("IPTCH Error: " + LastError() + " with command: " + command);
            }

            return ptr;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>if sucessful</returns>
        public bool Commit()
        {
            RequireHandle();

            if (logger != null)
            {
                foreach (var c in _debugEntries)
                {
                    logger.InfoFormat("IPTables Update: {0}", c);
                }
                _debugEntries.Clear();
            }

            bool status;

            if (_ipVersion == 4)
            {
                status = iptc_commit(_handle) == 1;
            }
            else
            {
                status = ip6tc_commit(_handle) == 1;
            }
            if (!status)
            {
                Free();
            }
            else
            {
                //Commit includes free
                _handle = IntPtr.Zero;
            }
            return status;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="chainName"></param>
        /// <returns>if chain exists</returns>
        public bool HasChain(string chainName)
        {
            RequireHandle();
            if (_ipVersion == 4)
            {
                return iptc_is_chain(chainName, _handle) == 1;
            }

            return ip6tc_is_chain(chainName, _handle) == 1;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="chainName"></param>
        /// <returns>if sucessful</returns>
        public bool AddChain(string chainName)
        {
            RequireHandle();
            if (_ipVersion == 4)
            {
                return iptc_create_chain(chainName, _handle) == 1;
            }
            return ip6tc_create_chain(chainName, _handle) == 1;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="chainName"></param>
        /// <returns>if sucessful</returns>
        public bool DeleteChain(string chainName)
        {
            RequireHandle();
            if (_ipVersion == 4)
            {
                return iptc_delete_chain(chainName, _handle) == 1;
            }
            return ip6tc_delete_chain(chainName, _handle) == 1;
        }
    }
}
