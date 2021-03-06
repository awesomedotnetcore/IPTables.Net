﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using IPTables.Net.Exceptions;
using IPTables.Net.Iptables.DataTypes;
using IPTables.Net.Iptables.IpSet.Parser;
using IPTables.Net.Supporting;

namespace IPTables.Net.Iptables.IpSet
{
    /// <summary>
    /// A "entry" in an IPSet "set"
    /// </summary>
    public class IpSetEntry
    {
        #region Fields
        private IpCidr _cidr;
        private String _protocol;
        private int _port;
        private String _mac;
        private IpSetSet _set;
        #endregion


        #region Properties
        public IpCidr Cidr
        {
            get { return _cidr; }
            set { _cidr = value; }
        }

        public string Protocol
        {
            get { return _protocol; }
            set { _protocol = value; }
        }

        public int Port
        {
            get { return _port; }
            set { _port = value; }
        }

        public string Mac
        {
            get { return _mac; }
            set { _mac = value; }
        }

        public IpSetSet Set
        {
            get { return _set; }
            internal set { _set = value; }
        }
        #endregion

        #region Constructor

        public IpSetEntry(IpSetSet set, IpCidr? cidr = null, string protocol = null, int port = -1, string mac = null)
        {
            _set = set;
            _cidr = cidr.HasValue?cidr.Value:IpCidr.Any;
            _protocol = protocol;
            _port = port;
            _mac = mac;
        }

        #endregion

        public String GetKeyCommand()
        {
            List<String> parts = new List<string>();
            if (_cidr.Prefix != 0)
            {
                parts.Add(_cidr.ToString());
            }
            if (_mac != null)
            {
                parts.Add(_mac);
            }
            if (_port != -1)
            {
                String prefix = "";
                if (_protocol != null)
                {
                    prefix = _protocol + ":";
                }
                parts.Add(prefix + _port);
            }
            if (parts.Count == 0)
            {
                throw new IpTablesNetException("Invalid IpSet entry, no parts to key");
            }

            return String.Join(",", parts.ToArray());
        }

        protected bool Equals(IpSetEntry other)
        {
            return _set.Equals(other.Set) && KeyEquals(other);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((IpSetEntry) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = _cidr.GetHashCode();
                hashCode = (hashCode*397) ^ (_protocol != null ? _protocol.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ _port.GetHashCode();
                hashCode = (hashCode*397) ^ (_mac != null ? _mac.GetHashCode() : 0);
                return hashCode;
            }
        }

        public static IpSetEntry ParseFromParts(IpSetSet set, String value)
        {
            var entry = new IpSetEntry(set);
            IpSetEntryParser.ParseEntry(entry, value);
            return entry;
        }

        public static IpSetEntry Parse(String command, IpSetSets sets)
        {
            var parts = command.Split(new char[] {' '});

            if (parts.Length < 2)
            {
                return null;
            }

            IpSetEntry entry = new IpSetEntry(null);
            string[] arguments = ArgumentHelper.SplitArguments(command);
            var parser = new IpSetEntryParser(arguments, entry, sets);

            for (int i = 0; i < arguments.Length; i++)
            {
                i += parser.FeedToSkip(i);
            }

            return entry;
        }

        public bool KeyEquals(IpSetEntry other)
        {
            bool r = _cidr.Equals(other._cidr) && _port == other._port && string.Equals(_mac, other._mac);
            if (!r)
            {
                return false;
            }

            return string.Equals(_protocol, other._protocol) ||
                   (String.IsNullOrEmpty(_protocol) && String.IsNullOrEmpty(other._protocol));
        }

        public string GetFullCommand(String command = "add")
        {
            return String.Format("{0} {1} {2}", command, Set.Name, GetKeyCommand());
        }
    }
}
