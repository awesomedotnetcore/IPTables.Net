﻿using System;
using System.Net;

namespace IPTables.Net.Iptables.DataTypes
{
    public struct IPPortOrRange
    {
        private readonly IPAddress _lowerAddress;
        private readonly IPAddress _upperAddress;
        private PortOrRange _port;

        public IPPortOrRange(IPAddress lowerAddress, IPAddress upperAddress, PortOrRange port)
        {
            _lowerAddress = lowerAddress;
            _upperAddress = upperAddress;
            _port = port;
        }

        public IPPortOrRange(IPAddress lowerAddress, IPAddress upperAddress)
        {
            _lowerAddress = lowerAddress;
            _upperAddress = upperAddress;
            _port = PortOrRange.Any;
        }

        public IPPortOrRange(IPAddress lowerAddress, PortOrRange port)
        {
            _upperAddress = _lowerAddress = lowerAddress;
            _port = port;
        }

        public IPPortOrRange(IPAddress lowerAddress)
        {
            _upperAddress = _lowerAddress = lowerAddress;
            _port = PortOrRange.Any;
        }

        public IPAddress LowerAddress
        {
            get { return _lowerAddress; }
        }

        public IPAddress UpperAddress
        {
            get { return _upperAddress; }
        }

        private String PortStringRepresentation()
        {
            if (_port.LowerPort == 0 && _port.UpperPort == 0)
            {
                return "";
            }

            return _port.ToString();
        }

        public override String ToString()
        {
            if (LowerAddress.Equals(UpperAddress))
            {
                return LowerAddress + ":" + PortStringRepresentation();
            }

            return String.Format("{0}-{1}:{2}", LowerAddress, UpperAddress, PortStringRepresentation());
        }

        public static IPPortOrRange Parse(string getNextArg)
        {
            string[] split = getNextArg.Split(new[] {':'});
            if (split.Length == 0)
            {
                throw new Exception("Error");
            }

            string[] splitIp = split[0].Split(new[] {'-'});

            IPAddress lowerIp = IPAddress.Parse(splitIp[0]);
            IPAddress upperIp;
            if (splitIp.Length == 1)
            {
                upperIp = lowerIp;
            }
            else
            {
                upperIp = IPAddress.Parse(splitIp[1]);
            }

            if (split.Length == 1)
            {
                return new IPPortOrRange(lowerIp, upperIp);
            }
            return new IPPortOrRange(lowerIp, upperIp, PortOrRange.Parse(split[1]));
        }
    }
}