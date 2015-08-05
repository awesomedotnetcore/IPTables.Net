﻿using System;
using System.Collections.Generic;
using System.Text;
using IPTables.Net.Iptables.Helpers;

namespace IPTables.Net.Iptables.Modules.U32
{
    public class U32Module : ModuleBase, IEquatable<U32Module>, IIpTablesModuleGod
    {
        private const String OptionBytecode = "--u32";

        public String ByteCode;

        public bool Equals(U32Module other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(ByteCode, other.ByteCode);
        }

        int IIpTablesModuleInternal.Feed(RuleParser parser, bool not)
        {
            switch (parser.GetCurrentArg())
            {
                case OptionBytecode:
                    ByteCode = parser.GetNextArg();
                    return 1;
            }

            return 0;
        }

        public bool NeedsLoading
        {
            get { return true; }
        }

        public String GetRuleString()
        {
            var sb = new StringBuilder();

            if (ByteCode != null)
            {
                sb.Append("--u32 ");
                sb.Append(ShellHelper.EscapeArguments(ByteCode));
            }

            return sb.ToString();
        }

        public static IEnumerable<String> GetOptions()
        {
            var options = new List<string>
            {
                OptionBytecode
            };
            return options;
        }

        public static ModuleEntry GetModuleEntry()
        {
            return GetModuleEntryInternal("u32", typeof(U32Module), GetOptions);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((U32Module)obj);
        }

        public override int GetHashCode()
        {
            return (ByteCode != null ? ByteCode.GetHashCode() : 0);
        }
    }
}